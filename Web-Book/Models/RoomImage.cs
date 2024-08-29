
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web_Book.Models
{
    public class RoomImage
    {
        [Key]
        public int ImageID { get; set; }

        [ForeignKey("Room")]
        public int RoomID { get; set; }

        public string ImagePath { get; set; }

        [JsonIgnore]
        public virtual Room Room { get; set; }
    }
}
