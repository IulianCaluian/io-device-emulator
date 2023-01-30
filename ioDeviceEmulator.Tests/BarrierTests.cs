using ioDeviceEmulator.Client.Models;

namespace ioDeviceEmulator.Tests
{
    [TestClass]
    public class BarrierTests
    {
        [TestMethod]
        public async Task StartActivated_ShouldChangeDegreeTo90()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(200);

            // Act
            barrier.Start.Activated = true;
            await Task.Delay(1000);

            // Assert

        }
    }
}