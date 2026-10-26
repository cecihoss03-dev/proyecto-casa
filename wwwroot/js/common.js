/* ==========================================================================
   Proyecto C.A.S.A. — utilidades compartidas por todas las vistas
   --------------------------------------------------------------------------
   Aquí vive lo único que querrás tocar al conectar con tu backend:
     1. CASA.API_BASE   → URL base de tu API
     2. CASA.endpoints  → ruta de cada controlador
   ========================================================================== */
(function () {
  'use strict';

  const CASA = {};
  window.CASA = CASA;

  /* ---------- 1. Configuración ------------------------------------------ */
  CASA.API_BASE = 'http://localhost:5000';

  CASA.endpoints = {
    mascotas:             '/api/Mascotas',
    reportes:             '/api/Reportes',
    tiposReporte:         '/api/TipoReporte',
    denuncias:            '/api/DenunciasAbandonos',
    solicitudes:          '/api/SolicitudesAdopcion',
    adopciones:           '/api/Adopciones',
    establecimientos:     '/api/Establecimientos',
    tiposEstablecimiento: '/api/TipoEstableciminetos',
    distritos:            '/api/Distritos',
    rescatistas:          '/api/Rescatistas',
    authLogin:            '/api/Auth/login',
    authRegister:         '/api/Auth/register'
  };

  CASA.SUCRE = { lat: -19.0333, lng: -65.2627, zoom: 13 };

  /* ---------- 2. Texto seguro y formato --------------------------------- */
  const ESC = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
  CASA.esc = (v) => String(v ?? '').replace(/[&<>"']/g, (c) => ESC[c]);

  CASA.g = function (obj, ...names) {
    if (obj == null || typeof obj !== 'object') return undefined;
    const keys = Object.keys(obj);
    for (const name of names) {
      const hit = keys.find((k) => k.toLowerCase() === name.toLowerCase());
      if (hit !== undefined && obj[hit] != null) return obj[hit];
    }
    return undefined;
  };

  CASA.fmtDate = function (v, withTime) {
    if (!v) return '—';
    const d = new Date(v);
    if (isNaN(d)) return CASA.esc(v);
    const opts = withTime
      ? { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' }
      : { day: '2-digit', month: 'short', year: 'numeric' };
    return d.toLocaleDateString('es-BO', opts);
  };

  const strip = (s) => String(s ?? '').normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase().trim();
  CASA.strip = strip;

  CASA.imgUrl = function (v) {
    const s = String(v ?? '').trim();
    if (!s) return '';
    if (/^data:image\/(png|jpe?g|gif|webp|avif);base64,[a-z0-9+/=]+$/i.test(s)) return s;
    if (/^https?:\/\//i.test(s)) return s;
    if (/^[a-z][a-z0-9+.-]*:/i.test(s)) return '';
    return CASA.API_BASE + (s.startsWith('/') ? s : '/' + s);
  };

  CASA.estadoClass = function (estado) {
    const s = strip(estado);
    if (!s) return 'is-neutral';
    if (/(no disponible|rechaz|cancel|inactiv|cerrad|clausur|fallec)/.test(s)) return 'is-bad';
    if (/(adoptad|resuelt|finaliz|complet|atendid|entregad)/.test(s)) return 'is-done';
    if (/(disponible|aprob|activ|abiert|en adopcion|libre)/.test(s)) return 'is-ok';
    if (/(pendiente|proceso|rescate|revision|espera|perdid|abandon|reportad|ocupad)/.test(s)) return 'is-wait';
    return 'is-neutral';
  };
  CASA.badge = (estado) => `<span class="casa-badge ${CASA.estadoClass(estado)}">${CASA.esc(estado || 'Sin estado')}</span>`;

  CASA.especieEmoji = function (especie) {
    const s = strip(especie);
    if (/(perr|can|dog)/.test(s)) return '🐶';
    if (/(gat|fel|cat)/.test(s)) return '🐱';
    if (/(conej)/.test(s)) return '🐰';
    if (/(ave|pajar|loro)/.test(s)) return '🐦';
    return '🐾';
  };

  const COLORES = {
    rojo: '#E5384F', naranja: '#FF7A00', amarillo: '#F6C21A', verde: '#2E9E5B',
    azul: '#0B47C9', celeste: '#3DA5F4', celeste_claro: '#8FD0FF', violeta: '#7A4FD6', morado: '#7A4FD6',
    rosa: '#F26FA8', rosado: '#F26FA8', marron: '#8B5A2B', cafe: '#8B5A2B',
    negro: '#1B1F2A', blanco: '#FFFFFF', gris: '#8A94A8', turquesa: '#18B8B0', celestes: '#3DA5F4'
  };
  CASA.cintaColor = function (v) {
    const raw = String(v ?? '').trim();
    if (!raw) return '#FF7A00';
    const k = strip(raw).replace(/\s+/g, '_');
    if (COLORES[k]) return COLORES[k];
    if (/^#[0-9a-f]{3,8}$/i.test(raw)) return raw;
    if (/^[a-z0-9#(),.%\s-]+$/i.test(raw) && window.CSS && CSS.supports('color', raw)) return raw;
    return '#FF7A00';
  };

  /* ---------- 3. Geografía --------------------------------------------- */
  CASA.parseLatLng = function (v) {
    if (v == null || v === '') return null;
    let a, b;
    if (typeof v === 'object') {
      a = Number(CASA.g(v, 'lat', 'latitud', 'latitude'));
      b = Number(CASA.g(v, 'lng', 'lon', 'long', 'longitud', 'longitude'));
    } else {
      const s = String(v).trim();
      if (s.startsWith('{') || s.startsWith('[')) {
        try { return CASA.parseLatLng(JSON.parse(s)); } catch (_) { return null; }
      }
      const m = s.match(/(-?\d+(?:\.\d+)?)\s*[,;\s]\s*(-?\d+(?:\.\d+)?)/);
      if (!m) return null;
      a = Number(m[1]); b = Number(m[2]);
    }
    if (Array.isArray(v)) { a = Number(v[0]); b = Number(v[1]); }
    if (!isFinite(a) || !isFinite(b) || Math.abs(a) > 90 || Math.abs(b) > 180) return null;
    return [a, b];
  };

  CASA.parsePolygon = function (v) {
    if (v == null || v === '') return null;
    let data = v;
    if (typeof v === 'string') {
      const s = v.trim();
      if (s.startsWith('{') || s.startsWith('[')) {
        try { data = JSON.parse(s); } catch (_) { return null; }
      } else {
        data = s.split(/[;|\n]+/).map((p) => p.split(/[,\s]+/).filter(Boolean).map(Number));
      }
    }
    const geom = (d) => {
      if (!d || typeof d !== 'object' || Array.isArray(d)) return null;
      if (d.type === 'FeatureCollection') return geom(d.features && d.features[0]);
      if (d.type === 'Feature') return geom(d.geometry);
      if (d.type === 'Polygon') return d.coordinates[0].map(([x, y]) => [y, x]);
      if (d.type === 'MultiPolygon') return d.coordinates[0][0].map(([x, y]) => [y, x]);
      return null;
    };
    const fromGeo = geom(data);
    if (fromGeo) return fromGeo;

    if (!Array.isArray(data)) return null;
    let ring = data;
    while (Array.isArray(ring) && Array.isArray(ring[0]) && Array.isArray(ring[0][0])) ring = ring[0];
    let pts = ring
      .map((p) => (Array.isArray(p) ? [Number(p[0]), Number(p[1])]
        : (p && typeof p === 'object' ? [Number(CASA.g(p, 'lat', 'latitud')), Number(CASA.g(p, 'lng', 'lon', 'longitud'))] : [NaN, NaN])))
      .filter((p) => isFinite(p[0]) && isFinite(p[1]));
    if (pts.length < 3) return null;
    if (Math.abs(pts[0][0]) > 40 && Math.abs(pts[0][1]) <= 40) pts = pts.map(([x, y]) => [y, x]);
    return pts;
  };

  /* ---------- 4. Cliente de la API ------------------------------------- */
  class ApiError extends Error {
    constructor(message, status, data) { super(message); this.name = 'ApiError'; this.status = status; this.data = data; }
  }
  CASA.ApiError = ApiError;

  function explain(data, fallback) {
    if (!data) return fallback;
    if (typeof data === 'string') return data.slice(0, 300);
    if (data.errors && typeof data.errors === 'object') {
      const list = Object.entries(data.errors).flatMap(([k, v]) => [].concat(v).map((m) => `${k}: ${m}`));
      if (list.length) return list.join(' · ');
    }
    return data.detail || data.title || data.message || fallback;
  }

  async function request(path, options = {}) {
    const url = /^https?:\/\//i.test(path) ? path : CASA.API_BASE + path;
    const headers = { Accept: 'application/json', ...(options.headers || {}) };
    if (options.body !== undefined) headers['Content-Type'] = 'application/json';
    let res;
    try {
      res = await fetch(url, {
        method: options.method || 'GET',
        headers,
        body: options.body !== undefined ? JSON.stringify(options.body) : undefined
      });
    } catch (e) {
      throw new ApiError(`No se pudo conectar con la API (${CASA.API_BASE}).`, 0, null);
    }
    const text = await res.text();
    let data = null;
    if (text) { try { data = JSON.parse(text); } catch (_) { data = text; } }
    if (!res.ok) throw new ApiError(explain(data, `Error ${res.status} ${res.statusText}`), res.status, data);
    return data;
  }

  CASA.list = function (data) {
    if (Array.isArray(data)) return data;
    if (data && typeof data === 'object') {
      for (const k of ['$values', 'items', 'data', 'value', 'results']) if (Array.isArray(data[k])) return data[k];
    }
    return [];
  };

  CASA.api = {
    get:  (path) => request(path),
    list: async (path) => CASA.list(await request(path)),
    listSoft: async (path) => { try { return CASA.list(await request(path)); } catch (_) { return []; } },
    post: (path, body) => request(path, { method: 'POST', body }),
    put:  (path, body) => request(path, { method: 'PUT', body }),
    del:  (path) => request(path, { method: 'DELETE' })
  };

  CASA.byId = (items) => Object.fromEntries(CASA.list(items).map((x) => [CASA.g(x, 'id'), x]));

  /* ---------- 5. Autenticación y Sesión --------------------------------- */
  CASA.auth = {
    async login(email, password, remember) {
      const response = await fetch(`${CASA.API_BASE}${CASA.endpoints.authLogin}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.message || 'Correo o contraseña incorrectos');
      }

      const data = await response.json();
      localStorage.setItem('casa_token', data.token);
      localStorage.setItem('casa_user', JSON.stringify(data.user));
      return data;
    },

    async register(userData) {
      const response = await fetch(`${CASA.API_BASE}${CASA.endpoints.authRegister}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData)
      });

      if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        const msg = error.errors 
          ? Object.values(error.errors).flat().join(' ') 
          : (error.message || 'Error al crear la cuenta');
        throw new Error(msg);
      }

      return await response.json();
    },

    isLoggedIn() {
      return !!localStorage.getItem('casa_token');
    },

    user() {
      const userStr = localStorage.getItem('casa_user');
      return userStr ? JSON.parse(userStr) : null;
    },

    logout() {
      localStorage.removeItem('casa_token');
      localStorage.removeItem('casa_user');
      window.location.href = 'login.html';
    },

    isStaff() {
      const user = this.user();
      return user && user.roles && (user.roles.includes('Admin') || user.roles.includes('CEMZOO'));
    },

    safeNext(url) {
      return url && url.startsWith('/') && !url.includes('//') ? url : 'index.html';
    }
  };

  /* ---------- 6. Interfaz: estados y avisos ----------------------------- */
  CASA.ui = {
    skeletons(el, n = 6, colClass = 'col-12 col-md-6 col-xl-4') {
      el.innerHTML = Array.from({ length: n }, () => `<div class="${colClass}"><div class="casa-skeleton" aria-hidden="true"></div></div>`).join('');
      el.setAttribute('aria-busy', 'true');
    },
    done(el) { el.removeAttribute('aria-busy'); },
    empty(el, emoji, title, msg, wrapClass = 'col-12') {
      el.innerHTML = `<div class="${wrapClass}"><div class="casa-state"><span class="casa-state__emoji" aria-hidden="true">${emoji}</span><h3>${CASA.esc(title)}</h3><p>${CASA.esc(msg)}</p></div></div>`;
      el.removeAttribute('aria-busy');
    },
    error(el, err, endpoint, wrapClass = 'col-12') {
      const offline = err instanceof ApiError && err.status === 0;
      const title = offline ? 'No logramos conectar con el servidor' : 'La API respondió con un error';
      const hint = offline
        ? `Revisa que el backend esté corriendo en <code>${CASA.esc(CASA.API_BASE)}</code> y que tenga CORS habilitado.`
        : CASA.esc(err.message || 'Error desconocido');
      el.innerHTML = `<div class="${wrapClass}"><div class="casa-state casa-state--error" role="alert"><span class="casa-state__emoji" aria-hidden="true">🐕‍🦺</span><h3>${title}</h3><p>${hint}</p>${endpoint ? `<p class="small mb-0">Ruta: <code>${CASA.esc(endpoint)}</code></p>` : ''}</div></div>`;
      el.removeAttribute('aria-busy');
    },
    toast(message, kind = '') {
      let zone = document.querySelector('.casa-toast-zone');
      if (!zone) {
        zone = document.createElement('div');
        zone.className = 'casa-toast-zone';
        zone.setAttribute('role', 'status');
        zone.setAttribute('aria-live', 'polite');
        document.body.appendChild(zone);
      }
      const t = document.createElement('div');
      t.className = 'casa-toast ' + (kind ? 'is-' + kind : '');
      t.textContent = message;
      zone.appendChild(t);
      setTimeout(() => t.remove(), 5200);
    }
  };

  CASA.districtHtml = function (distrito) {
    if (!distrito) return '';
    const color = CASA.cintaColor(CASA.g(distrito, 'colorCinta'));
    return `<span class="pet-card__district"><span class="ribbon-dot" style="--ribbon:${CASA.esc(color)}"></span>${CASA.esc(CASA.g(distrito, 'nombre') || 'Distrito')}</span>`;
  };

  CASA.petCard = function (m, distritosById, opts = {}) {
    const g = CASA.g, esc = CASA.esc;
    const id = g(m, 'id');
    const nombre = g(m, 'nombre') || 'Sin nombre';
    const especie = g(m, 'especie');
    const edad = g(m, 'edad');
    const tam = g(m, 'tamaño', 'tamano');
    const color = g(m, 'color');
    const car = g(m, 'caracteristica', 'características');
    const foto = CASA.imgUrl(g(m, 'foto'));
    const estado = g(m, 'estado');
    const distrito = (distritosById || {})[g(m, 'distritoId')] || g(m, 'distritos', 'distrito');
    const ribbon = CASA.cintaColor(distrito && g(distrito, 'colorCinta'));
    const disponible = CASA.estadoClass(estado) === 'is-ok';
    const traits = [
      [CASA.especieEmoji(especie), especie],
      ['🎂', edad],
      ['📏', tam],
      ['🎨', color]
    ].filter(([, t]) => t).map(([e, t]) => `<span class="pet-trait"><span aria-hidden="true">${e}</span>${esc(t)}</span>`).join('');

    return `
      <article class="pet-card" style="--ribbon:${esc(ribbon)}">
        <div class="pet-card__photo">
          <div class="pet-card__fallback" aria-hidden="true">${CASA.especieEmoji(especie)}</div>
          ${foto ? `<img src="${esc(foto)}" alt="Foto de ${esc(nombre)}" loading="lazy" data-fallback">` : ''}
          <span class="pet-card__status">${CASA.badge(estado)}</span>
          <span class="pet-card__ribbon"></span>
        </div>
        <div class="pet-card__body">
          <h3 class="pet-tag">${esc(nombre)}</h3>
          <div class="pet-card__meta">${traits}</div>
          <p class="pet-card__desc">${esc(car || 'Aún no tiene una descripción.')}</p>
          <div class="pet-card__foot">
            ${distrito ? CASA.districtHtml(distrito) : '<span></span>'}
            ${opts.adopt
              ? `<button type="button" class="btn-casa btn-casa-sm" data-adopt="${esc(id)}" data-nombre="${esc(nombre)}" ${disponible ? '' : 'disabled'}>Quiero adoptar</button>`
              : ''}
          </div>
        </div>
      </article>`;
  };

  document.addEventListener('error', (e) => {
    const t = e.target;
    if (t && t.tagName === 'IMG' && t.hasAttribute('data-fallback')) t.remove();
  }, true);

  CASA.stars = function (n) {
    const v = Math.max(0, Math.min(5, Math.round(Number(n) || 0)));
    return `<span class="stars" role="img" aria-label="${(Number(n) || 0).toFixed(1)} de 5">${'★'.repeat(v)}<span class="off">${'★'.repeat(5 - v)}</span></span>`;
  };

  /* ---------- 7. Menú y pie de página ----------------------------------- */
  const NAV = [
    ['index',            'index.html',            'Inicio'],
    ['mascotas',         'mascotas.html',         'Mascotas'],
    ['reportes',         'reportes.html',         'Reportes'],
    ['adopciones',       'adopciones.html',       'Adopciones'],
    ['establecimientos', 'establecimientos.html', 'Veterinarias y albergues'],
    ['mapa',             'mapa.html',             'Mapa']
  ];

  function mountLayout() {
    const active = document.body.dataset.page || '';
    const navHost = document.getElementById('casa-nav');
    const footHost = document.getElementById('casa-footer');

    if (navHost) {
      navHost.innerHTML = `
        <a class="visually-hidden-focusable btn-casa btn-casa-sm position-absolute m-2" href="#contenido">Saltar al contenido</a>
        <nav class="navbar navbar-expand-lg casa-nav sticky-top" aria-label="Navegación principal">
          <div class="container">
            <a class="casa-brand" href="index.html" aria-label="Proyecto C.A.S.A., inicio">
              <span class="casa-brand__logo" aria-hidden="true">🐾</span>
              <span>C.A.S.A.<small>Rescate y adopción · Sucre</small></span>
            </a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#casaMenu"
                    aria-controls="casaMenu" aria-expanded="false" aria-label="Abrir menú">
              <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse justify-content-end" id="casaMenu">
              <ul class="navbar-nav align-items-lg-center gap-lg-1 pt-3 pt-lg-0">
                ${NAV.map(([key, href, label]) => `
                  <li class="nav-item"><a class="nav-link ${key === active ? 'active' : ''}" href="${href}" ${key === active ? 'aria-current="page"' : ''}>${label}</a></li>`).join('')}
              </ul>
            </div>
          </div>
        </nav>`;
    }

    if (footHost) {
      footHost.innerHTML = `
        <footer class="casa-footer mt-5">
          <div class="container d-flex flex-column flex-md-row justify-content-between gap-3">
            <div>
              <a class="casa-brand" href="index.html"><span class="casa-brand__logo" aria-hidden="true">🐾</span><span>C.A.S.A.</span></a>
              <p class="mt-2 mb-0 small">Rescate, adopción y gestión animal.<br>Sucre, Chuquisaca · Bolivia</p>
            </div>
            <ul class="list-unstyled small mb-0">
              ${NAV.slice(1).map(([, href, label]) => `<li><a href="${href}">${label}</a></li>`).join('')}
            </ul>
          </div>
        </footer>`;
    }
  }

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', mountLayout);
  else mountLayout();
})();