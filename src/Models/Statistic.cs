namespace covidSim.Models
{
    public class Statistic
    {
        public int Healthy;
        public int Dead;
        public int Ill;
        public int Recovered;

        public void Reset()
        {
            Healthy = 0;
            Dead = 0;
            Ill = 0;
            Recovered = 0;
            
        }
    }
}