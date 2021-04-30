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
        private const int InfectionRadius = 7;

        public const double DoctorsPercentage = 0.1;
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
            var illPeoples = (int)Math.Round(IllPeoplePercentage * PeopleCount);
            var doctors = (int)Math.Round(DoctorsPercentage * PeopleCount);
            var people = new List<Person>();

            for (var i = 0; i < illPeoples; i++)
            {
                people.Add(new Doctor(i,FindHome(),Map, PersonHealthStatus.Ill));
            }

            for (var i = 0; i < doctors; i++)
            {
                people.Add(new Person(i + illPeoples, FindHome(), Map, PersonHealthStatus.Healthy));
            }

            for (var i = 0; i < PeopleCount - illPeoples - doctors; i++)
            {
                people.Add(new Person(i + illPeoples + doctors, FindHome(), Map, PersonHealthStatus.Healthy));
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
                var currentTick = Interlocked.Increment(ref this.currentTick);
                CalcNextStep(currentTick);
            }

            return this;
        }

        private void CalcNextStep(int currentTick)
        {
            _lastUpdate = DateTime.Now;
            var walkingNotInfected = new List<Person>();
            var walkingInfected = new List<Person>();
            var allInfected = new List<Person>();
            var doctors = new List<Person>();
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
                    
                    else if (!(person is Doctor))
                        walkingNotInfected.Add(person);
                }
                if (person.HealthStatus == PersonHealthStatus.Ill)
                    allInfected.Add(person);
                else if (person is Doctor)
                {
                    doctors.Add(person);
                }
            }
            
            foreach (var person in personsToRemove)
                People.Remove(person);
            CheckInfections(walkingInfected, walkingNotInfected);
            CheckRecovery(doctors, allInfected);
        }

        private void CheckRecovery(List<Person> doctors, List<Person> allInfected)
        {
            foreach (var doctor in doctors)
            foreach (var infectedPerson in allInfected)
            {
                if (CanHaveInteraction(InfectionRadius, doctor, infectedPerson))
                {
                    infectedPerson.HealthStatus = PersonHealthStatus.Healthy;
                }
            }
        }
        
        private static void CheckInfections(List<Person> walkingInfected, List<Person> walkingNotInfected)
        {
            foreach (var notInfected in walkingNotInfected)
            foreach (var infected in walkingInfected)
            {
                if (CanHaveInteraction(InfectionRadius, notInfected, infected) 
                    && _random.NextBoolWithChance(1, 2))
                {
                    notInfected.HealthStatus = PersonHealthStatus.Ill;
                    break;
                    
                }
            }
        }

        private static bool CanHaveInteraction(int maxdistance, Person personA, Person personB)
        {
            
            return GetDistance(personA, personB) <= maxdistance;

        }

        private static double GetDistance(Person personA, Person personB)
        {
            return Math.Sqrt((personA.Position.X - personB.Position.X) 
                             * (personA.Position.X - personB.Position.X) +
                             (personA.Position.Y - personB.Position.Y) * 
                             (personA.Position.Y - personB.Position.Y));
        }
    }
}