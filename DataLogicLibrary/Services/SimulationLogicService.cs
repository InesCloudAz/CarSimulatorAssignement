using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataLogicLibrary.DirectionStrategies.Interfaces;
using DataLogicLibrary.DTO;
using DataLogicLibrary.Infrastructure.Enums;
using DataLogicLibrary.Services.Interfaces;

namespace DataLogicLibrary.Services
{
    public class SimulationLogicService : ISimulationLogicService
    {
        public delegate IDirectionStrategy DirectionStrategyResolver(MovementAction movementAction);

        public SimulationLogicService(IDirectionContext directionContext, DirectionStrategyResolver directionStrategyResolver)
        {
            _directionContext = directionContext;
            _turnLeftStrategy = directionStrategyResolver(MovementAction.Left);
            _turnRightStrategy = directionStrategyResolver(MovementAction.Right);
            _driveForwardStrategy = directionStrategyResolver(MovementAction.Forward);
            _reverseStrategy = directionStrategyResolver(MovementAction.Backward);
        }

        private readonly IDirectionContext _directionContext;
        private readonly IDirectionStrategy _turnLeftStrategy;
        private readonly IDirectionStrategy _turnRightStrategy;
        private readonly IDirectionStrategy _driveForwardStrategy;
        private readonly IDirectionStrategy _reverseStrategy;

        private const int MaxEnergy = 20;
        private const int MaxGas = 20;
        private const int MaxHunger = 16;   // spel över om hunger >= 16

        public StatusDTO PerformAction(int userInput, StatusDTO currentStatus)
        {

            // Om man försöker köra (1-4) men bensinen är slut
            if ((userInput == 1 || userInput == 2 || userInput == 3 || userInput == 4)
                && currentStatus.GasValue == 0)
            {
                // Bilen kan inte köra, returnera status oförändrad
                return currentStatus;
            }

            switch (userInput)
            {
                case 1: 
                    _directionContext.SetStrategy(_turnLeftStrategy);
                    break;

                case 2: 
                    _directionContext.SetStrategy(_turnRightStrategy);
                    break;

                case 3: 
                    _directionContext.SetStrategy(_driveForwardStrategy);
                    break;

                case 4: 
                    _directionContext.SetStrategy(_reverseStrategy);
                    break;

                case 5: 
                    currentStatus.EnergyValue = MaxEnergy;
                    return currentStatus;

                case 6: 
                    currentStatus.GasValue = MaxGas;
                    return currentStatus;

                case 8: 
                    currentStatus.HungerValue = 0;
                    return currentStatus;

                case 7: 
                    return currentStatus;

                default:
                    return currentStatus;
            }

            return _directionContext.ExecuteStrategy(currentStatus);
        }

        public StatusDTO DecreaseStatusValues(int userInput, StatusDTO currentStatus)
        {
            Random random = new Random();

            // minska trötthet (1-5)
            int energyDecrease = random.Next(1, 6);
            currentStatus.EnergyValue -= energyDecrease;

            // bensin minskar bara om man inte rastar
            if (userInput != 5)
            {
                int gasDecrease = random.Next(1, 6);
                currentStatus.GasValue -= gasDecrease;
            }

            // hunger ökar alltid när man gör något, utom när man äter
            if (userInput != 8)
                currentStatus.HungerValue += 2;

            // clamp values så de inte går under 0
            if (currentStatus.EnergyValue < 0)
                currentStatus.EnergyValue = 0;

            if (currentStatus.GasValue < 0)
                currentStatus.GasValue = 0;

            if (currentStatus.HungerValue > MaxHunger)
                currentStatus.HungerValue = MaxHunger;

            return currentStatus;
        }


    }
}








