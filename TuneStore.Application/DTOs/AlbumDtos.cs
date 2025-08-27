using System.ComponentModel.DataAnnotations;

namespace TuneStore.Application.DTOs
{
    public class AlbumCreateUpdateDto
    {
        [Required, StringLength(200)] public string Title { get; set; } = null!;
        [Required, StringLength(120)] public string Artist { get; set; } = null!;
        [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
        [Range(0, int.MaxValue)] public int StockQuantity { get; set; }
    }
}
