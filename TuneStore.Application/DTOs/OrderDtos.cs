using System.ComponentModel.DataAnnotations;

namespace TuneStore.Application.DTOs
{
    public class OrderCreateDto
    {
        [Required] public int CustomerId { get; set; }
        [Required] public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        [Required] public int AlbumId { get; set; }
        [Range(1, int.MaxValue)] public int Quantity { get; set; }
    }
}
