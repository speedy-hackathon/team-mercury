using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using covidSim.Models;

namespace covidSim.Services
{
    public class Game
    {
        public HashSet<Person> People;
        public CityMap Map;
        public Statistic Statistic;
        private DateTime _lastUpdate;
        private int currentTick;

        private static Game _gameInstance;
        private static Random _random = new Random();

        public const double IllPeoplePercentage = 0.05;
        public const int PeopleCount = 320;
        public const int FieldWidth = 1000;
        public const int FieldHeight = 500;
        public const int MaxPeopleInHouse = 10;

        private Game()
        {
            Statistic = new Statistic();
            Map = new CityMap();
            People = CreatePopulation().ToHashSet();
            _lastUpdate = DateTime.Now;
        }

        public static Game Instance => _gameInstance ?? (_gameInstance = new Game());

        private IEnumerable<Person> CreatePopulation()
        {
            var illPeoples = Math.Round(IllPeoplePercentage * PeopleCount);

            return Enumerable
                .Range(0, PeopleCount)
                .Select(index => new Person(index, FindHome(), Map,
                    illPeoples-- > 0 ? PersonHealthStatus.Ill : PersonHealthStatus.Healthy));
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
                var currentTick = Interlocked.Increment(ref this.currentTick);
                CalcNextStep(currentTick);
            }
            CalcStatistic();
            return this;
        }

        private void CalcNextStep(int currentTick)
        {
            _lastUpdate = DateTime.Now;
            var walkingNotInfected = new List<Person>();
            var walkingInfected = new List<Person>();
            var personsToRemove = new List<Person>();
            foreach (var person in People)
            {
                person.CalcNextStep(currentTick);

                if (person.ShouldRemove(currentTick))
                {
                    personsToRemove.Add(person);
                    continue;
                }

                if (person.state == PersonState.Walking)
                {
                    if (person.HealthStatus == PersonHealthStatus.Ill)
                        walkingInfected.Add(person);
                    else if(person.HealthStatus != PersonHealthStatus.Recovered)
                        walkingNotInfected.Add(person);
                }
            }

            foreach (var person in personsToRemove)
                People.Remove(person);
            CheckInfections(walkingInfected, walkingNotInfected);
        }

        private static void CheckInfections(List<Person> walkingInfected, List<Person> walkingNotInfected)
        {
            foreach (var notInfected in walkingNotInfected)
            {
                foreach (var infected in walkingInfected)
                {
                    var distance = Math.Sqrt((notInfected.Position.X - infected.Position.X) 
                                             * (notInfected.Position.X - infected.Position.X) +
                                             (notInfected.Position.Y - infected.Position.Y) * 
                                             (notInfected.Position.Y - infected.Position.Y));
                    if (distance <= 7 && _random.Next(0, 2) == 1)
                    {
                        notInfected.HealthStatus = PersonHealthStatus.Ill;
                        break;
                    }
                }
            }
        }

        private void CalcStatistic()
        {
            Statistic.Reset();
            foreach (var person in People)
            {
                switch (person.HealthStatus)
                {
                    case PersonHealthStatus.Healthy:
                        Statistic.Healthy++;
                        break;
                    case PersonHealthStatus.Ill:
                        Statistic.Ill++;
                        break;
                    case PersonHealthStatus.Dead:
                        Statistic.Dead++;
                        break;
                    case PersonHealthStatus.Recovered:
                        Statistic.Recovered++;
                        break;
                }
            }
        }
    }
}