using ioDeviceEmulator.Client.Models;
using System;

namespace ioDeviceEmulator.Tests
{
    [TestClass]
    public class BarrierTests
    {
        [TestMethod]
        public void StartActivated_ShouldRaiseBarrierStateChangedEvent()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(200);
            BarrierStateChangedEventArgs? eventArgs = null;
            barrier.BarrierStateChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            // Act
            barrier.Start.Activated = true;

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(BarrierState.Opening, eventArgs.NewState);
            Assert.AreEqual(BarrierState.Opening, barrier.GetBarrierState());
        }

        [TestMethod]
        public void StartActivated_ShouldNotRaiseBarrierStateChangedEventIfOpened()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(200);
            barrier.SetStateToOpened();
            BarrierStateChangedEventArgs? eventArgs = null;
            barrier.BarrierStateChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            // Act
            barrier.Start.Activated = true;

            // Assert
            Assert.IsNull(eventArgs);
            Assert.AreEqual(BarrierState.Opened, barrier.GetBarrierState());
        }

        [TestMethod]
        public void StopActivatedAfterStart_ShouldRaiseBarrierStateChangedEvent()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(200);
            BarrierStateChangedEventArgs? eventArgs = null;
            barrier.BarrierStateChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            // Act
            barrier.Start.Activated = true;
            barrier.Stop.Activated = true;

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(BarrierState.Closing, eventArgs.NewState);
            Assert.AreEqual(BarrierState.Closing, barrier.GetBarrierState());
        }

        [TestMethod]
        public void StopActivated_ShouldNotRaiseBarrierStateChangedEventIfClosed()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(200);
            barrier.SetStateToClosed();
            BarrierStateChangedEventArgs? eventArgs = null;
            barrier.BarrierStateChanged += (sender, args) =>
            {
                eventArgs = args;
            };
         
            // Act
            barrier.Stop.Activated = true;

            // Assert
            Assert.IsNull(eventArgs);
            Assert.AreEqual(BarrierState.Closed, barrier.GetBarrierState());
        }


        [TestMethod]
        public async Task SetStateToOpened_ShouldStartClosingAfterAutoClosingTime()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(10);
            barrier.Start.Activated = false;
            barrier.Start2.Activated = false;

            // Act
            barrier.SetStateToOpened();
            await Task.Delay(100);

            // Assert
            Assert.AreEqual(BarrierState.Closing, barrier.GetBarrierState());
        }

        [TestMethod]
        public async Task SetStateToOpened_ShouldKeepItOpenedIfStartActivated()
        {
            // Arrange
            var barrier = new Client.Models.Barrier(10);
            barrier.Start.Activated = true;
            barrier.Start2.Activated = false;

            // Act
            barrier.SetStateToOpened();
            await Task.Delay(100);

            // Assert
            Assert.AreEqual(BarrierState.Opened, barrier.GetBarrierState());
        }

        //[TestMethod]
        //public async Task SetStateToOpened_ShouldKeepItOpenedIfStartActivated()
        //{
        //    // Arrange
        //    var barrier = new Client.Models.Barrier(10);
        //    barrier.Start.Activated = true;
        //    barrier.Start2.Activated = false;
        //    barrier.SetStateToOpened();

        //    // Act

        //    barrier.Start.Activated = true;
        //    await Task.Delay(100);

        //    // Assert
        //    Assert.AreEqual(BarrierState.Closing, barrier.GetBarrierState());

        //    // Act

        //    // Assert
        //}

    }
}