using System;

namespace covidSim.Utils
{
    public static class RandomExtensions
    {
        public static bool NextBoolWithChance(this Random random, int chance, int possibilities) => 
            random.Next(0, possibilities) < chance;
    }
}