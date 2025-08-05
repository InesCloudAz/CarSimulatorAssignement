using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarSimulator.Server.Models;
using DataLogicLibrary.Infrastructure.Enums;

namespace CarSimulator.Server.Factories
{
    public class CarFactory : ICarFactory
    {
        public Car CreateCar()
        {
            return new Car()
            {
                CardinalDirection = CardinalDirection.North,
                GasValue = 20
            };
        }
    }
}
