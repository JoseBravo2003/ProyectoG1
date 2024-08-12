namespace Proyecto.Models
{
    public class Cart
    {

        
        public int Id { get; set; }
        public int UserId { get; set; }
        public int IdComida { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Usuario Usuario { get; set; }
        public Comida Comida { get; set; }
    
}
}
