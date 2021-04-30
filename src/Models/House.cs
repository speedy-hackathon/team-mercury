using System.Net.NetworkInformation;

namespace covidSim.Models
{
    public class House
    {
        public House(int id, Vec cornerCoordinates)
        {
            Id = id;
            Coordinates = new HouseCoordinates(cornerCoordinates);
        }

        public int Id;
        public HouseCoordinates Coordinates;
        public int ResidentCount = 0;

        public bool ContainsPoint(Vec point)
        {
            var dx = point.X - Coordinates.LeftTopCorner.X;
            var dy = point.Y - Coordinates.LeftTopCorner.Y;
            return (dx >= 0 && dy >= 0) && (dx - HouseCoordinates.Width <= 0 && dy - HouseCoordinates.Height <= 0);
        }
    }
}