using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garage3.Models
{
    public class VehicleViewModel
    {
        //Ägare, Medlemskap, Fordonstyp, RegNum och ParkTid som minimum.
        [DisplayName("Vehicle Type")]
        public string VehicleTypeName { get; set; } = string.Empty;

        [DisplayName("Reg. No.")]
        public string RegNo { get; set;} = string.Empty;


        [DisplayName("Time Parked (dd.hh:mm)")]
        [DisplayFormat(DataFormatString = "{0:dd\\.hh\\:mm}")]
        public TimeSpan ParkTime { get; set; }

        [DisplayName("Parking Spaces")]
        public string ParkingSpaces { get; set; }

        public string Owner { get; set; } = string.Empty;

        [DisplayName("Membership Level")]
        public string MembershipLevel { get; set; } = "Basic";

    }
}
