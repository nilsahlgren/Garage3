namespace Garage3.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        public string RegNo { get; set; } = string.Empty;

        public string VehicleTypeName { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty; 

        public string Model { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int NoOfWheels { get; set; }

        public Session? Session { get; set; }
    }
}
