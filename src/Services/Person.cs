using System;
using covidSim.Models;
using covidSim.Utils;

namespace covidSim.Services
{
    public class Person
    {
        private const int MaxDistancePerTurn = 30;
        private static Random random = new Random();
        public PersonState state = PersonState.AtHome;
        private HouseCoordinates houseCoordinates;
        private static readonly int CureTime = 45;
        private int illTick;

        public Person(int id, int homeId, CityMap map, PersonHealthStatus healthStatus)
        {
            Id = id;
            HomeId = homeId;
            HealthStatus = healthStatus;

            var homeCoords = map.Houses[homeId].Coordinates.LeftTopCorner;
            houseCoordinates = map.Houses[homeId].Coordinates;
            var x = homeCoords.X + random.Next(HouseCoordinates.Width);
            var y = homeCoords.Y + random.Next(HouseCoordinates.Height);
            Position = new Vec(x, y);
        }

        public readonly int Id;
        public readonly int HomeId;
        public Vec Position;
        public PersonType PersonType = PersonType.CommonPerson;

        public PersonHealthStatus HealthStatus { get; set; }
        private int deadAtTick = 0;

        public void CalcNextStep(int currentTick)
        {
            UpdateHealthStatus(currentTick);

            if (HealthStatus != PersonHealthStatus.Dead)
                switch (state)
                {
                    case PersonState.AtHome:
                        CalcNextStepForPersonAtHome();
                        break;
                    case PersonState.Walking:
                        CalcNextPositionForWalkingPerson();
                        break;
                    case PersonState.GoingHome:
                        CalcNextPositionForGoingHomePerson();
                        break;
                }
        }

        public bool ShouldRemove(int currentTick)
        {
            return HealthStatus == PersonHealthStatus.Dead && currentTick > deadAtTick + 10;
        }

        private void UpdateHealthStatus(int currentTick)
        {
            if (HealthStatus == PersonHealthStatus.Ill)
            {
                if (random.NextBoolWithChance(3, 100000))
                {
                    HealthStatus = PersonHealthStatus.Dead;
                    deadAtTick = currentTick;
                }

                illTick++;
                if (illTick >= CureTime) HealthStatus = PersonHealthStatus.Healthy;
            }
        }

        private void CalcNextStepForPersonAtHome()
        {
            var goingWalk = random.NextDouble() < 0.005;
            if (!goingWalk)
            {
                var x = random.Next(houseCoordinates.LeftTopCorner.X,
                    houseCoordinates.LeftTopCorner.X + HouseCoordinates.Height);
                var y = random.Next(houseCoordinates.LeftTopCorner.Y,
                    houseCoordinates.LeftTopCorner.Y + HouseCoordinates.Width);
                Position = new Vec(x, y);
                return;
            }

            state = PersonState.Walking;
            CalcNextPositionForWalkingPerson();
        }

        private void CalcNextPositionForWalkingPerson()
        {
            var xLength = random.Next(MaxDistancePerTurn);
            var yLength = MaxDistancePerTurn - xLength;
            var direction = ChooseDirection();
            var delta = new Vec(xLength * direction.X, yLength * direction.Y);
            var nextPosition = new Vec(Position.X + delta.X, Position.Y + delta.Y);

            if (isCoordInField(nextPosition))
            {
                Position = nextPosition;
            }
            else
            {
                CalcNextPositionForWalkingPerson();
            }
        }

        private void CalcNextPositionForGoingHomePerson()
        {
            var game = Game.Instance;
            var homeCoord = game.Map.Houses[HomeId].Coordinates.LeftTopCorner;
            var homeCenter = new Vec(homeCoord.X + HouseCoordinates.Width / 2,
                homeCoord.Y + HouseCoordinates.Height / 2);

            var xDiff = homeCenter.X - Position.X;
            var yDiff = homeCenter.Y - Position.Y;
            var xDistance = Math.Abs(xDiff);
            var yDistance = Math.Abs(yDiff);

            var distance = xDistance + yDistance;
            if (distance <= MaxDistancePerTurn)
            {
                Position = homeCenter;
                state = PersonState.AtHome;
                return;
            }

            var direction = new Vec(Math.Sign(xDiff), Math.Sign(yDiff));

            var xLength = Math.Min(xDistance, MaxDistancePerTurn);
            var newX = Position.X + xLength * direction.X;
            var yLength = MaxDistancePerTurn - xLength;
            var newY = Position.Y + yLength * direction.Y;
            Position = new Vec(newX, newY);
        }

        public void GoHome()
        {
            if (state != PersonState.Walking) return;

            state = PersonState.GoingHome;
            CalcNextPositionForGoingHomePerson();
        }

        public override int GetHashCode() => Id;

        private Vec ChooseDirection()
        {
            var directions = new Vec[]
            {
                new Vec(-1, -1),
                new Vec(-1, 1),
                new Vec(1, -1),
                new Vec(1, 1),
            };
            var index = random.Next(directions.Length);
            return directions[index];
        }

        private bool isCoordInField(Vec vec)
        {
            var belowZero = vec.X < 0 || vec.Y < 0;
            var beyondField = vec.X > Game.FieldWidth || vec.Y > Game.FieldHeight;

            return !(belowZero || beyondField);
        }
    }
}