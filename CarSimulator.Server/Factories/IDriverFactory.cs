using APIServiceLibrary.DTO;
using CarSimulator.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulator.Server.Factories
{
    public interface IDriverFactory
    {
        Driver CreateDriver(ResultsDTO resultDTO);
    }
}
