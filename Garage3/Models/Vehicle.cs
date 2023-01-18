using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        [Required]
        [DisplayName("Registration number")]
        [RegularExpression(@"^[A-Za-z]{3}(\d{3})$", 
            ErrorMessage = "Must be in format ABC123.")]
        public string RegNo { get; set; } = string.Empty;

        [DisplayName("Vehicle type")]
        public string VehicleTypeName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Vehicle brand")]
        [StringLength(18)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [DisplayName("Vehicle model")]
        [StringLength(18)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [DisplayName("Vehicle color")]
        [StringLength(12)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [DisplayName("Number of wheels")]
        [Range(0, 12)]
        public int NoOfWheels { get; set; }

        public Session? Session { get; set; }
    }
}
