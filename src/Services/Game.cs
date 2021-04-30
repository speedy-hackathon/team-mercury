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
        public const double DoctorsPercentage = 0.1;
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
            var illPeoples = (int)Math.Round(IllPeoplePercentage * PeopleCount);
            var doctors = (int) Math.Round(DoctorsPercentage * PeopleCount);
            
            var people = new List<Person>();
            for (var i = 0; i < illPeoples; i++)
            {
                people.Add(new Person(i, FindHome(), Map, PersonHealthStatus.Ill));
            }
            
            for (var i = 0; i < doctors; i++)
            {
                people.Add(new Doctor(illPeoples + i, FindHome(), Map, PersonHealthStatus.Healthy));
            }

            for (var i = 0; i < PeopleCount - illPeoples - doctors; i++)
            {
                people.Add(new Person(illPeoples + doctors + i, FindHome(), Map, PersonHealthStatus.Healthy)); 
            }

            return people;
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
            var walkingNotInfected = new List<Person>();
            var walkingInfected = new List<Person>();
            var allInfected = new List<Person>();
            var doctors = new List<Person>();
            foreach (var person in People)
            {
                person.CalcNextStep();
                if (person.state == PersonState.Walking)
                {
                    if (person.HealthStatus == PersonHealthStatus.Ill)
                    {
                        walkingInfected.Add(person);
                    }
                    else if (!(person is Doctor))
                    {
                        walkingNotInfected.Add(person);
                    }
                }
                if (person.HealthStatus == PersonHealthStatus.Ill)
                    allInfected.Add(person);
                else if (person is Doctor)
                {
                    doctors.Add(person);
                }
            }
            CheckInfections(walkingInfected, walkingNotInfected);
            CheckRecovery(doctors, allInfected);

        }

        private void CheckRecovery(List<Person> doctors, List<Person> allInfected)
        {
            foreach (var doctor in doctors)
            foreach (var infectedPerson in allInfected)
            {
                if (CanHaveInteraction(7, doctor, infectedPerson))
                {
                    infectedPerson.HealthStatus = PersonHealthStatus.Healthy;
                }
            }
        }

        private void CheckInfections(List<Person> walkingInfected, List<Person> walkingNotInfected)
        {
            foreach (var notInfected in walkingNotInfected)
            {
                foreach (var infected in walkingInfected)
                {
                    if (CanHaveInteraction(7, notInfected, infected) 
                        && _random.Next(0, 2) == 1)
                    {
                        notInfected.HealthStatus = PersonHealthStatus.Ill;
                        break;
                    }
                }
            }
        }

        private bool CanHaveInteraction(int maxdistance, Person personA, Person personB)
        {
            
            var distance = Math.Sqrt((personA.Position.X - personB.Position.X) 
                                     * (personA.Position.X - personB.Position.X) +
                                     (personA.Position.Y - personB.Position.Y) * 
                                     (personA.Position.Y - personB.Position.Y));
            return distance <= maxdistance;

        }
    }
}