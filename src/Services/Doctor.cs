using covidSim.Models;
using Microsoft.Extensions.DependencyInjection;


namespace covidSim.Services
{
    public class Doctor : Person
    {
        public Doctor(int id, int homeId, CityMap map, PersonHealthStatus healthStatus) : base(id, homeId, map, healthStatus)
        {
            this.PersonType = PersonType.Doctor;
        }
    }
}