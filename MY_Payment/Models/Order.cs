namespace MY_Payment.Models
{
    public class OrderMY
    {
        public Guid id { get; set; }
        public Guid clientId { get; set; }
        public Guid storeId { get; set; }
        public Guid shoppingCartId { get; set; }
        public Guid clientAddressId { get; set; }
        public string secuence { get; set; } = string.Empty;
        public string identification { get; set; } = string.Empty;
        public string businessName { get; set; } = string.Empty;
        public decimal amount { get; set; }
        public ClientCard? paymentMethod { get; set; }
        public Transaction? paymentStatus { get; set; }
        public string? status { get; set; }
        public DateTime creationDate { get; set; } = DateTime.UtcNow;
        public decimal deliveryFee { get; set; }
        public decimal averageTime { get; set; }
        public decimal service { get; set; }
        public decimal total { get; set; } = 0;
        public string? bookingId { get; set; }
        public string? trackingUrl { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? checkoutId { get; set; }
        public List<OrderDetailById>? details { get; set; }
        public List<OrderDeliveryStatusById>? deliveryStatus { get; set; }
        public string? shortNameStatus { get; set; }
        public string? longNameStatus { get; set; }
    }

    public class OrderDetailById
    {
        public Guid id { get; set; }
        public Guid orderId { get; set; }
        public int secuence { get; set; } = 0;
        public int quantity { get; set; } = 0;
        public decimal unitPrice { get; set; }
        public decimal price { get; set; }
        public bool hasIva { get; set; }
        public decimal iva { get; set; }
        public decimal ice { get; set; }
        public decimal discount { get; set; }
        public decimal finalPrice { get; set; }
        public Guid productId { get; set; }
        public string name { get; set; } = string.Empty;
        public string? presentation { get; set; } = string.Empty;
        public string? photo { get; set; } = string.Empty;
    }

    public class OrderDeliveryStatusById
    {
        public string status { get; set; } = string.Empty;
        public string shortName { get; set; } = string.Empty;
        public string longName { get; set; } = string.Empty;
        public DateTime currentDate { get; set; } = DateTime.UtcNow;
    }


}
