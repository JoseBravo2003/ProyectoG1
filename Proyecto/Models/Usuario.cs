namespace Proyecto.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string clave { get; set; } // 'clave' en minúscula

        public string Role { get; set; }

       // Propiedades adicionales para el historial de compras
       public List<PurchaseHistory> PurchaseHistory { get; set; }
    }

    public class PurchaseHistory
    {
        public int Id { get; set; }
        public string PaymentType { get; set; }
        public string MaskedCardNumber { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderId { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        

        // Clave externa para relacionar con Usuario
        public int UserId { get; set; }

        // Propiedad de navegación
        public Usuario Usuario { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }  // ID de la tabla
        public string PaymentType { get; set; }  // Tipo de pago (Card o Cash)
        public string CardOwner { get; set; }  // Nombre del titular de la tarjeta
        public string CardNumber { get; set; }  // Número de la tarjeta (enmascarado)
        public string CVV { get; set; }  // CVV
        public string ExpirationDate { get; set; }  // Fecha de expiración (MM/YY)
        public string DeliveryAddress { get; set; }  // Dirección de entrega
    }
}

