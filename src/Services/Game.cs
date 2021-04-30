using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing.Internal;
using covidSim.Models;

namespace covidSim.Services
{
    public class Game
    {
        public List<Person> People;
        public CityMap Map;
        private DateTime _lastUpdate;

        private static Game _gameInstance;
        private static Random _random = new Random();

        public const double IllPeoplePercentage = 0.05;
        public const int PeopleCount = 320;
        public const int FieldWidth = 1000;
        public const int FieldHeight = 500;
        public const int MaxPeopleInHouse = 10;

        private Game()
        {
            Map = new CityMap();
            People = CreatePopulation();
            _lastUpdate = DateTime.Now;
        }

        public static Game Instance => _gameInstance ?? (_gameInstance = new Game());

        private List<Person> CreatePopulation()
        {
            var illPeoples = Math.Round(IllPeoplePercentage * PeopleCount);

            return Enumerable
                .Range(0, PeopleCount)
                .Select(index => new Person(index, FindHome(), Map,
                    illPeoples-- > 0 ? PersonHealthStatus.Ill : PersonHealthStatus.Healthy))
                .ToList();
        }

        public static void Restart()
        {
            _gameInstance = new Game();
        }

        private int FindHome()
        {
            while (true)
            {
                var homeId = _random.Next(CityMap.HouseAmount);

                if (Map.Houses[homeId].ResidentCount < MaxPeopleInHouse)
                {
                    Map.Houses[homeId].ResidentCount++;
                    return homeId;
                }
            }
            
        }

        public Game GetNextState()
        {
            var diff = (DateTime.Now - _lastUpdate).TotalMilliseconds;
            if (diff >= 1000)
            {
                CalcNextStep();
            }

            return this;
        }

        private void CalcNextStep()
        {
            _lastUpdate = DateTime.Now;
            foreach (var person in People)
            {
                person.CalcNextStep();
            }
        }
    }
}