using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Garage3.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^(\d{8})-(\d{4})$",
            ErrorMessage = "Characters are not allowed.")]
        public string PersNo { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public List<Vehicle>? Vehicles { get; set; }

        public List<Session>? Sessions { get; set; }

    }
}
