using CarSimulator.Server.Models;
using DataLogicLibrary.DTO;

namespace CarSimulator.Server.Models.ViewModels
{
    public class SimulationViewModel
    {

        public bool IsRunning { get; set; } = false;

        public int SelectedAction { get; set; }

        public Car Car { get; set; }

        public Driver Driver { get; set; }

        public StatusDTO CurrentStatus { get; set; }

        public string CarStatusMessage { get; set; }

        public string DriverStatusMessage { get; set; }

        public string CurrentActionMessage { get; set; }



    }
}
