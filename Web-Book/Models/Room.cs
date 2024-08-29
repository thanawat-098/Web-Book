using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web_Book.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [StringLength(50)]
        public string RoomType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [JsonPropertyName("roomImages")]
        public virtual ICollection<RoomImage>? RoomImages { get; set; }


    }
}
