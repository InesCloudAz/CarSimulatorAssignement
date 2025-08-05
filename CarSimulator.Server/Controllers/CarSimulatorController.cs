using APIServiceLibrary.Services;
using CarSimulator.Server.Factories;
using CarSimulator.Server.Models.ViewModels;
using DataLogicLibrary.Factories;
using DataLogicLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarSimulator.Server.Controllers
{
    public class CarSimulatorController : Controller
    {

        public CarSimulatorController(IAPIService apiService, ISimulationLogicService simulationLogicService, ICarFactory carFactory,
                                      IDriverFactory driverFactory, IStatusFactory statusFactory, IStatusMessageService statusMessageService)
        {
            _apiService = apiService;
            _simulationLogicService = simulationLogicService;
            _carFactory = carFactory;
            _driverFactory = driverFactory;
            _statusFactory = statusFactory;
            _statusMessageService = statusMessageService;
        }

        private readonly IAPIService _apiService;
        private readonly ISimulationLogicService _simulationLogicService;
        private readonly ICarFactory _carFactory;
        private readonly IDriverFactory _driverFactory;
        private readonly IStatusFactory _statusFactory;
        private readonly IStatusMessageService _statusMessageService;



        public async Task<IActionResult> Index(SimulationViewModel viewModel)
        {
            if (viewModel.SelectedAction == 7)
            {
                return RedirectToAction("Index");
            }

            if (viewModel.Driver == null)
            {
                var resultDTO = await _apiService.GetOneDriver();
                var driver = _driverFactory.CreateDriver(resultDTO);
                viewModel.Driver = driver;
            }

            if (viewModel.IsRunning)
            {
                if (viewModel.Car != null)
                {
                    viewModel.CurrentStatus = _simulationLogicService.DecreaseStatusValues(viewModel.SelectedAction, viewModel.CurrentStatus);
                    viewModel.CurrentStatus = _simulationLogicService.PerformAction(viewModel.SelectedAction, viewModel.CurrentStatus);
                }

                else
                {
                    viewModel.Car = _carFactory.CreateCar();
                    viewModel.CurrentStatus = _statusFactory.CreateStatus();
                }

                viewModel.CurrentActionMessage = _statusMessageService.GetCurrentActionMessage(viewModel.SelectedAction, viewModel.CurrentStatus.GasValue, viewModel.Driver.First)!;
                viewModel.DriverStatusMessage = _statusMessageService.GetDriverStatusMessage(viewModel.CurrentStatus.EnergyValue, viewModel.Driver.First)!;
                viewModel.CarStatusMessage = _statusMessageService.GetCarStatusMessage(viewModel.CurrentStatus.GasValue)!;
            }

            ModelState.Clear();
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult PerformAction(SimulationViewModel viewModel)
        {



            return View("Index", viewModel);
        }

    }
}
