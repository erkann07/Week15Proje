namespace TuneStore.Domain.Entities
{
    public enum UserRole { Admin = 1, Customer = 2 }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!; // DataProtected
        public UserRole Role { get; set; } = UserRole.Customer;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!;
        public ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();
    }

    public class OrderAlbum
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class MaintenanceWindow
    {
        public int Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsEnabled { get; set; }
        public string? Message { get; set; }
    }

    public class RequestLog
    {
        public int Id { get; set; }
        public DateTime RequestedAtUtc { get; set; }
        public string Path { get; set; } = null!;
        public string Method { get; set; } = null!;
        public int? UserId { get; set; }
        public string? Ip { get; set; }
    }
}
