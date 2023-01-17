using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Garage3.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Pers.Number")]
        [RegularExpression(@"^(\d{4})(\d{2})(\d{2})-(\d{4})$",
            ErrorMessage = "Must be in format YYYYMMDD-XXXX.")]
        public string PersNo { get; set; } = string.Empty;

        [Required]
        [DisplayName("First Name")]
        [StringLength(18)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Last Name")]
        [StringLength(18)]
        public string LastName { get; set; } = string.Empty;

        public List<Vehicle>? Vehicles { get; set; }

        public List<Session>? Sessions { get; set; }

    }
}
