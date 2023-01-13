using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Garage3.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string PersNo { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public List<Vehicle>? Vehicles { get; set; }

        public List<Session>? Sessions { get; set; }

    }
}
