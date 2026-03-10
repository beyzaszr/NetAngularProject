using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; } = "";

        [Column(TypeName = "nvarchar(250)")]
        public string Description { get; set; } = "";

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsCanceled { get; set; } = false;

        public DateTime? CanceledAt { get; set; }

        // Dosya yükleme için
        [Column(TypeName = "nvarchar(255)")]
        public string? DocumentPath { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? DocumentOriginalName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
