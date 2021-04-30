using System;
using System.Collections.Generic;
using System.Linq;
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
            var walkingNotInfected = new List<Person>();
            var walkingInfected = new List<Person>();
            foreach (var person in People)
            {
                person.CalcNextStep();
                if (person.state == PersonState.Walking)
                {
                    if (person.HealthStatus == PersonHealthStatus.Ill)
                    {
                        walkingInfected.Add(person);
                    }
                    else
                    {
                        walkingNotInfected.Add(person);
                    }
                }
            }
            Console.WriteLine(walkingInfected.Count);
            CheckInfections(walkingInfected, walkingNotInfected);

        }

        private void CheckInfections(List<Person> walkingInfected, List<Person> walkingNotInfected)
        {
            foreach (var notInfected in walkingNotInfected)
            {
                foreach (var infected in walkingInfected)
                {
                    var distance = Math.Sqrt((notInfected.Position.X - infected.Position.X) 
                                             * (notInfected.Position.X - infected.Position.X) +
                                             (notInfected.Position.Y - infected.Position.Y) * 
                                             (notInfected.Position.Y - infected.Position.Y));
                    if (distance <= 7)
                    {
                        if (_random.Next(0, 2) == 1)
                        {
                            notInfected.HealthStatus = PersonHealthStatus.Ill;
                            break;
                        }
                    }
                }
            }
        }
    }
}