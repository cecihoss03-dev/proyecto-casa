namespace Proyecto.Models;
public class TipoEstableciminetos
{
    public int Id {get;set;}
    public string nombre {get;set;}
    public List<Establecimientos> Establecimientos {get;set;} = new ();
}