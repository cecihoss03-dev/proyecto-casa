using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudadadano_Usuarios_UsuarioId",
                table: "Ciudadadano");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Usuarios_usuariosId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Rescatista_Usuarios_UsuariosId",
                table: "Rescatista");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Distrito_DistritosId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Rol_RolesId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_publicacion_PublicacionesId",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Usuario");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_RolesId",
                table: "Usuario",
                newName: "IX_Usuario_RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_PublicacionesId",
                table: "Usuario",
                newName: "IX_Usuario_PublicacionesId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_DistritosId",
                table: "Usuario",
                newName: "IX_Usuario_DistritosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudadadano_Usuario_UsuarioId",
                table: "Ciudadadano",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Usuario_usuariosId",
                table: "Comentario",
                column: "usuariosId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rescatista_Usuario_UsuariosId",
                table: "Rescatista",
                column: "UsuariosId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Distrito_DistritosId",
                table: "Usuario",
                column: "DistritosId",
                principalTable: "Distrito",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Rol_RolesId",
                table: "Usuario",
                column: "RolesId",
                principalTable: "Rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_publicacion_PublicacionesId",
                table: "Usuario",
                column: "PublicacionesId",
                principalTable: "publicacion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudadadano_Usuario_UsuarioId",
                table: "Ciudadadano");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Usuario_usuariosId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Rescatista_Usuario_UsuariosId",
                table: "Rescatista");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Distrito_DistritosId",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Rol_RolesId",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_publicacion_PublicacionesId",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_RolesId",
                table: "Usuarios",
                newName: "IX_Usuarios_RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_PublicacionesId",
                table: "Usuarios",
                newName: "IX_Usuarios_PublicacionesId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_DistritosId",
                table: "Usuarios",
                newName: "IX_Usuarios_DistritosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudadadano_Usuarios_UsuarioId",
                table: "Ciudadadano",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Usuarios_usuariosId",
                table: "Comentario",
                column: "usuariosId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rescatista_Usuarios_UsuariosId",
                table: "Rescatista",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Distrito_DistritosId",
                table: "Usuarios",
                column: "DistritosId",
                principalTable: "Distrito",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Rol_RolesId",
                table: "Usuarios",
                column: "RolesId",
                principalTable: "Rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_publicacion_PublicacionesId",
                table: "Usuarios",
                column: "PublicacionesId",
                principalTable: "publicacion",
                principalColumn: "Id");
        }
    }
}
