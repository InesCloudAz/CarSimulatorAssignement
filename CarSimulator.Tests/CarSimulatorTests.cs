using DataLogicLibrary.DirectionStrategies.Interfaces;
using DataLogicLibrary.DTO;
using DataLogicLibrary.Infrastructure.Enums;
using DataLogicLibrary.Services;
using System.Text.Json;

namespace CarSimulator.Tests
{

    [TestClass]
    public class RandomUserApiIntegrationTests
    {
        private readonly HttpClient _httpClient = new HttpClient();

        [TestMethod]
        public async Task RandomUserApi_ShouldReturnValidUser()
        {
            // Arrange
            var url = "https://randomuser.me/api/";

            // Act
            var response = await _httpClient.GetAsync(url);

            // Assert: API svarar OK
            Assert.IsTrue(response.IsSuccessStatusCode, "RandomUser API did not respond with success");

            var json = await response.Content.ReadAsStringAsync();

            // Försök deserialisera till JsonDocument
            using var doc = JsonDocument.Parse(json);
            Assert.IsTrue(doc.RootElement.TryGetProperty("results", out var results), "Response does not contain 'results'");

            // Kolla att det finns minst en user
            Assert.IsTrue(results.GetArrayLength() > 0, "No users returned from RandomUser API");
        }
    }
    [TestClass]
    public class SimulationLogicService_GasTests
    {
        private SimulationLogicService _service;

        // Fake context + strategy
        private class FakeDirectionContext : IDirectionContext
        {
            private IDirectionStrategy _strategy;
            public void SetStrategy(IDirectionStrategy strategy) => _strategy = strategy;
            public StatusDTO ExecuteStrategy(StatusDTO status) => status;
        }

        private class FakeDirectionStrategy : IDirectionStrategy
        {
            public StatusDTO Execute(StatusDTO currentStatus)
            {
                throw new NotImplementedException();
            }

            public StatusDTO Move(StatusDTO status) => status;
        }

        [TestInitialize]
        public void Setup()
        {
            var fakeContext = new FakeDirectionContext();
            _service = new SimulationLogicService(fakeContext, action => new FakeDirectionStrategy());
        }

        [TestMethod]
        public void Gas_ShouldDecrease_WhenDrivingForward()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 0 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.GasValue < 20, "Gas should decrease when driving forward");
        }

        [TestMethod]
        public void Gas_ShouldNotDecrease_WhenResting()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 0 };
            var result = _service.DecreaseStatusValues(5, status); // 5 = Rest

            Assert.AreEqual(20, result.GasValue, "Gas should not decrease when resting");
        }

        [TestMethod]
        public void Gas_ShouldRefillToMax_WhenRefueling()
        {
            var status = new StatusDTO { EnergyValue = 10, GasValue = 3, HungerValue = 0 };
            var result = _service.PerformAction(6, status); // 6 = Refuel

            Assert.AreEqual(20, result.GasValue, "Gas should refill to max when refueling");
        }

        [TestMethod]
        public void Gas_ShouldNotGoBelowZero()
        {
            var status = new StatusDTO { EnergyValue = 10, GasValue = 1, HungerValue = 0 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.GasValue >= 0, "Gas should never be negative");
        }


    }

    [TestClass]
    public class SimulationLogicService_EnergyTests
    {
        private SimulationLogicService _service;

        private class FakeDirectionContext : IDirectionContext
        {
            private IDirectionStrategy _strategy;
            public void SetStrategy(IDirectionStrategy strategy) => _strategy = strategy;
            public StatusDTO ExecuteStrategy(StatusDTO status) => status;
        }

        private class FakeDirectionStrategy : IDirectionStrategy
        {
            public StatusDTO Execute(StatusDTO status) => status;
        }

        [TestInitialize]
        public void Setup()
        {
            var fakeContext = new FakeDirectionContext();
            _service = new SimulationLogicService(fakeContext, action => new FakeDirectionStrategy());
        }

        [TestMethod]
        public void Hunger_ShouldIncrease_WhenDriving()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 0 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.HungerValue > 0, "Hunger should increase when performing an action");
        }

        [TestMethod]
        public void Hunger_ShouldNotIncrease_WhenEating()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 5 };
            var result = _service.DecreaseStatusValues(8, status); // 8 = Eat

            Assert.AreEqual(5, result.HungerValue, "Hunger should not increase when eating");
        }

        [TestMethod]
        public void Hunger_ShouldResetToZero_WhenEating()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 10 };
            var result = _service.PerformAction(8, status); // 8 = Eat

            Assert.AreEqual(0, result.HungerValue, "Hunger should reset to zero when eating");
        }

        [TestMethod]
        public void Hunger_ShouldClampAtMax()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 15 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.HungerValue <= 16, "Hunger should not exceed max value (16)");
        }

        [TestMethod]
        public void Energy_ShouldDecrease_WhenDrivingForward()
        {
            var status = new StatusDTO { EnergyValue = 20, GasValue = 20, HungerValue = 0 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.EnergyValue < 20, "Energy should decrease when driving forward");
        }

        [TestMethod]
        public void Energy_ShouldNotGoBelowZero()
        {
            var status = new StatusDTO { EnergyValue = 1, GasValue = 20, HungerValue = 0 };
            var result = _service.DecreaseStatusValues((int)MovementAction.Forward, status);

            Assert.IsTrue(result.EnergyValue >= 0, "Energy should never go below zero");
        }

        [TestMethod]
        public void Energy_ShouldRestoreToMax_WhenResting()
        {
            var status = new StatusDTO { EnergyValue = 5, GasValue = 20, HungerValue = 0 };
            var result = _service.PerformAction(5, status); // 5 = Rest

            Assert.AreEqual(20, result.EnergyValue, "Energy should reset to max when resting");
        }

    }

    [TestClass]
    public class SimulationLogicService_MenuTests
    {
        private SimulationLogicService _service;

        private class FakeDirectionContext : IDirectionContext
        {
            private IDirectionStrategy _strategy;
            public void SetStrategy(IDirectionStrategy strategy) => _strategy = strategy;
            public StatusDTO ExecuteStrategy(StatusDTO status) => status;
        }

        private class FakeDirectionStrategy : IDirectionStrategy
        {
            public StatusDTO Execute(StatusDTO status) => status;
        }

        [TestInitialize]
        public void Setup()
        {
            var fakeContext = new FakeDirectionContext();
            _service = new SimulationLogicService(fakeContext, action => new FakeDirectionStrategy());
        }

        [TestMethod]
        public void PerformAction_ShouldReturnUnchangedStatus_WhenExit()
        {
            var status = new StatusDTO { EnergyValue = 10, GasValue = 5, HungerValue = 3 };
            var result = _service.PerformAction(7, status); // Exit

            Assert.AreEqual(status, result, "Status should remain unchanged when exiting");
        }
        [TestMethod]
        public void PerformAction_ShouldReturnUnchangedStatus_WhenInvalidInput()
        {
            var status = new StatusDTO { EnergyValue = 10, GasValue = 5, HungerValue = 3 };
            var result = _service.PerformAction(99, status); // Invalid action

            Assert.AreEqual(status, result, "Status should remain unchanged for invalid input");
        }
        [TestMethod]
        public void Gas_ShouldNotAllowMovement_WhenEmpty()
        {
            var fakeContext = new FakeDirectionContext();
            bool strategyCalled = false;
            _service = new SimulationLogicService(fakeContext, action => new TestDirectionStrategy(() => strategyCalled = true));

            var status = new StatusDTO { EnergyValue = 10, GasValue = 0, HungerValue = 0 };
            var result = _service.PerformAction((int)MovementAction.Forward, status);

            Assert.IsFalse(strategyCalled, "Strategy should not execute when gas is empty");
        }

        private class TestDirectionStrategy : IDirectionStrategy
        {
            private readonly Action _onExecute;
            public TestDirectionStrategy(Action onExecute) => _onExecute = onExecute;
            public StatusDTO Execute(StatusDTO status)
            {
                _onExecute();
                return status;
            }
        }



    }

    [TestClass]
    public class SimulationLogicService_StrategyTests
    {
        private SimulationLogicService _service;
        private bool _strategyCalled;

        private class FakeDirectionContext : IDirectionContext
        {
            private IDirectionStrategy _strategy;
            public void SetStrategy(IDirectionStrategy strategy) => _strategy = strategy;
            public StatusDTO ExecuteStrategy(StatusDTO status) => _strategy.Execute(status);
        }

        private class TestDirectionStrategy : IDirectionStrategy
        {
            private readonly Action _onExecute;
            public TestDirectionStrategy(Action onExecute) => _onExecute = onExecute;
            public StatusDTO Execute(StatusDTO status)
            {
                _onExecute();
                return status;
            }
        }

        [TestInitialize]
        public void Setup()
        {
            _strategyCalled = false;
            var fakeContext = new FakeDirectionContext();
            _service = new SimulationLogicService(fakeContext, action => new TestDirectionStrategy(() => _strategyCalled = true));
        }

        [TestMethod]
        public void PerformAction_Left_ShouldCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(1, status);
            Assert.IsTrue(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Right_ShouldCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(2, status);
            Assert.IsTrue(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Forward_ShouldCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(3, status);
            Assert.IsTrue(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Backward_ShouldCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(4, status);
            Assert.IsTrue(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Rest_ShouldNotCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(5, status);
            Assert.IsFalse(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Refuel_ShouldNotCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(6, status);
            Assert.IsFalse(_strategyCalled);
        }

        [TestMethod]
        public void PerformAction_Eat_ShouldNotCallStrategy()
        {
            var status = new StatusDTO();
            _service.PerformAction(8, status);
            Assert.IsFalse(_strategyCalled);
        }
    }




}
