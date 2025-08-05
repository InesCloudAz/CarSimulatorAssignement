using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLogicLibrary.Services.Interfaces
{
    public interface IStatusMessageService
    {
        string? GetCurrentActionMessage(int userInput, int gasValue, string driverName);
        string? GetDriverStatusMessage(int value, string driverName);
        string? GetCarStatusMessage(int value);


    }
}
