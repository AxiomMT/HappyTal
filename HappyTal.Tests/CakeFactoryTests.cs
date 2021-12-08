using Microsoft.VisualStudio.TestTools.UnitTesting;
using HappyTal.Models;
using System;
using System.Threading.Tasks;

namespace HappyTal.Tests
{
    [TestClass]
    class CakeFactoryTests
    {
        static CakeFactory factory;

        #region Init Methods
        [ClassInitialize]
        public static void InitializeFactory(TestContext context)
        {
            factory = new CakeFactory();

            Cake.PrepareLowDurationBound = 5;
            Cake.PrepareTopDurationBound = 5;
            Cake.CuissonDuration = 10;
            Cake.EmballageDuration = 2;
        }

        [TestInitialize]
        public void RunFactory()
        {
            factory.Run();
        }

        [TestCleanup]
        public void StopFactory()
        {
            factory.Stop();
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void Preparation_After3Iterations_MakeMaxPreparationNumberx3()
        {
            Task.Delay(TimeSpan.FromSeconds(Cake.PrepareTopDurationBound * 3)).Wait();
            Assert.AreEqual(9, factory.PreparationNumber);
        }

        #endregion


    }
}
