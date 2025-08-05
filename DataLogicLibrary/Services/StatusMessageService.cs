using DataLogicLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLogicLibrary.Services
{
    public class StatusMessageService : IStatusMessageService
    {
        public string? GetCurrentActionMessage(int userInput, int gasValue, string driverName)
        {
            if (userInput != 6 && gasValue == 0)
                return "Refuel to drive the car!";

            return userInput switch
            {
                1 => "Car turns and drives to the left",
                2 => "Car turns and drives to the right",
                3 => "Car drives forward",
                4 => "Car reverses and drives backwards",
                5 => $"{driverName} takes a rest",
                6 => "Car refuels",
                7 => "Aborted simulation",
                _ => "Car is stationary."
            };
        }

        public string? GetDriverStatusMessage(int energyValue, string driverName)
        {
            if (energyValue <= 10 && energyValue > 5)
                return $"{driverName} is getting tired, consider taking a rest.";

            if (energyValue <= 5 && energyValue >= 1)
                return $"{driverName} is struggling to stay awake!";

            if (energyValue == 0)
                return $"{driverName} has fallen asleep!!!! WATCH OUT";

            return null;
        }

        public string? GetCarStatusMessage(int gasValue)
        {
            if (gasValue <= 10 && gasValue > 5)
                return "Car is running out of gas, consider refuelling the car.";

            if (gasValue <= 5 && gasValue >= 1)
                return "Car is almost out of gas!";

            if (gasValue == 0)
                return "Car is completely out of gas! You have to refuel to drive the car!";

            return null;
        }
    }
}
