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
        


        public StatusDTO PerformAction(int userInput, StatusDTO currentStatus)
        {
            if (userInput == 6)
                return currentStatus;

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
                    currentStatus.EnergyValue = 20;
                    return currentStatus;
                case 6:
                    currentStatus.GasValue = 20;
                    return currentStatus;

            }

            currentStatus = _directionContext.ExecuteStrategy(currentStatus);
            return currentStatus;

        }

        public StatusDTO DecreaseStatusValues(int userInput, StatusDTO currentStatus)
        {

            Random random = new Random();
            int energyDecrease = random.Next(1, 6);
            int gasDecrease = random.Next(1, 6);

            currentStatus.EnergyValue -= energyDecrease;

            var driverIsRestingValue = 5;

            if (userInput != driverIsRestingValue)
                currentStatus.GasValue -= gasDecrease;

            if (currentStatus.EnergyValue < 0)
                currentStatus.EnergyValue = 0;

            if (currentStatus.GasValue < 0)
                currentStatus.GasValue = 0;

            return currentStatus;

        }

 


    }
}
