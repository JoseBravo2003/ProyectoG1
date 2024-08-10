namespace Proyecto.Models
{
    public class Comida
    {
        public int IdComida { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public string ImagenUrl { get; set; }
        public int Cantidad { get; set; }
    }
}
