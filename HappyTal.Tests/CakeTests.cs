using Microsoft.VisualStudio.TestTools.UnitTesting;
using HappyTal.Models;
using System.Threading.Tasks;
using System;

namespace HappyTal.Tests
{
    [TestClass]
    public class CakeTests
    {
        private static Cake cake;
        
        // Initializing the cake's properties
        static int prepLowBound = 5;
        static int prepTopBound = 5;           // We constraint the duration to a fixed value instead of a random one we know the value to be checked
        static int cuissonDuration = 10;
        static int embDuration = 2;

        [ClassInitialize]
        public static void InitializeCake(TestContext context)
        {
            cake = new Cake(prepLowBound, prepTopBound, cuissonDuration, embDuration);

            Assert.AreEqual(prepLowBound, cake.PrepareLowDurationBound);
            Assert.AreEqual(prepTopBound, cake.PrepareTopDurationBound);
            Assert.AreEqual(cuissonDuration, cake.CuissonDuration);
            Assert.AreEqual(embDuration, cake.EmballageDuration);
        }

        [TestMethod]
        public void PreparationAsync_lookedBeforeItsTerm_DoesNotReturnPreparedState()
        {
            Task<State> preparationTask = cake.PreparationAsync();
            int waitTime = new Random().Next(0, prepLowBound);  // Wait time obviously that expected
            Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();

            State checkedState = cake.State;

            Assert.AreNotEqual(State.Prepared, checkedState);
        }

        [TestMethod]
        public void PreparationAsync_lookedBetweenLowAndTopDurationBounds_ReturnsPreparedState()
        {
            Task<State> preparationTask = cake.PreparationAsync();
            int waitTime = new Random().Next(prepLowBound, prepTopBound);  // Wait time obviously that expected
            Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();

            State checkedState = cake.State;

            Assert.AreEqual(State.Prepared, checkedState);
        }

        [TestMethod]
        public void CuissonAsync_lookedBeforeItsTerm_DoesNotReturnBakedState()
        {
            Task<State> preparationTask = cake.CuissonAsync();
            int waitTime = new Random().Next(0, cuissonDuration);  
            Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();

            State checkedState = cake.State;

            Assert.AreNotEqual(State.Baked, checkedState);
        }

        [TestMethod]
        public void CuissonAsync_lookedBetweenLowAndTopDurationBounds_ReturnsBakedState()
        {
            Task<State> preparationTask = cake.CuissonAsync();
            Task.Delay(TimeSpan.FromSeconds(cuissonDuration)).Wait();

            State checkedState = cake.State;

            Assert.AreEqual(State.Baked, checkedState);
        }

        [TestMethod]
        public void EmballageAsync_lookedBeforeItsTerm_DoesNotReturnPackedState()
        {
            Task<State> preparationTask = cake.EmballageAsync();
            int waitTime = new Random().Next(0, embDuration);
            Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();

            State checkedState = cake.State;

            Assert.AreNotEqual(State.Packed, checkedState);
        }

        [TestMethod]
        public void EmballageAsync_lookedBetweenLowAndTopDurationBounds_ReturnsBakedState()
        {
            Task<State> preparationTask = cake.EmballageAsync();
            Task.Delay(TimeSpan.FromSeconds(embDuration)).Wait();

            State checkedState = cake.State;

            Assert.AreEqual(State.Packed, checkedState);
        }
    }
}
