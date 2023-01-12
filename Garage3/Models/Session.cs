namespace Garage3.Models
{
    public class Session
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        public int VehicleId { get; set; }

        public List<ParkingSpace> ParkingSpaces { get; set; }

        public DateTime TimeOfArrival { get; set; }

        public DateTime TimeOfDeparture { get; set; }

        public double Price { get; set; }
    }
}
