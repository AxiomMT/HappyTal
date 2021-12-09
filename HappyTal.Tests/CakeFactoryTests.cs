using Microsoft.VisualStudio.TestTools.UnitTesting;
using HappyTal.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace HappyTal.Tests
{
    [TestClass]
    public class CakeFactoryTests
    {
        static CakeFactory factory;

        private int maxSimultaneousPreparation = 3;
        private int maxSimultaneousCuisson = 5;
        private int maxSimultaneousEmballage = 2;

        #region Init Methods
        [ClassInitialize]
        public static void InitializeFactory(TestContext context)
        {
            Cake.PrepareLowDurationBound = 5;
            Cake.PrepareTopDurationBound = 5;
            Cake.CuissonDuration = 10;
            Cake.EmballageDuration = 2;
        }

        [TestInitialize]
        public void RunFactory()
        {
            factory = new CakeFactory();
        }

        [TestCleanup]
        public void StopFactory()
        {
            factory.Stop();
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void Preparation_After2IterationsAnd2Seconds_MakeMaxPreparationNumberx2PreparedCakes()
        {
            // N.B: We take 2 seconds margin because the stages duration infact last more than the delay we asked for

            Task.Run(factory.Preparation);
            Task.Delay(TimeSpan.FromSeconds(2 * Cake.PrepareTopDurationBound + 2)).Wait();
            Assert.AreEqual(maxSimultaneousPreparation * 2, factory.Cakes.Count(c => c.State == State.Prepared));
        }

        [TestMethod]
        public void Cuisson_WithEnoughPreparedCakes_After2IterationsAnd2Seconds_MakeMaxCuissonNumberx2BakedCakes()
        {
            // N.B: We take 2 seconds margin because the stages duration infact last more than the delay we asked for
            for (int i = 0; i < maxSimultaneousCuisson * 2 + 2; i++)
            {
                factory.Cakes.Add(new Cake() { State = State.Prepared });
            }
            Task.Run(factory.Cuisson);
            Task.Delay(TimeSpan.FromSeconds(2 * Cake.CuissonDuration + 2)).Wait();
            Assert.AreEqual(maxSimultaneousCuisson * 2, factory.Cakes.Count(c => c.State == State.Baked));
        }

        [TestMethod]
        public void Emballage_WithEnoughBakedCakes_After2IterationsAnd2Seconds_MakeMaxEmballageNumberx2PackedCakes()
        {
            // N.B: We take 2 seconds margin because the stages duration infact last more than the delay we asked for
            for (int i = 0; i < maxSimultaneousEmballage * 2 + 2; i++)
            {
                factory.Cakes.Add(new Cake() { State = State.Baked });
            }
            Task.Run(factory.Emballage);
            Task.Delay(TimeSpan.FromSeconds(2 * Cake.EmballageDuration + 2)).Wait();
            Assert.AreEqual(maxSimultaneousEmballage * 2, factory.Cakes.Count(c => c.State == State.Packed));
        }

        #endregion


    }
}
