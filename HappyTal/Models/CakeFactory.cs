using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HappyTal.Models
{
    public class CakeFactory
    {
        #region Properties
        private int maxSimultaneousPreparation = 3;
        private int maxSimultaneousCuisson = 5;
        private int maxSimultaneousEmballage = 2;

        // The cake's creation taks will run indefinitely until we manually stop them. To do so, Cancellation tokens will be needed
        private Task prepaTask, cuissonTask, embTask;
        private CancellationTokenSource prepaToken, cuissonToken, embToken;
        private StageState prepaState, cuissonState, embState;

        public List<Cake> Cakes;                    

        // "Real-time" counting the number of Cakes going through each step
        public int PreparationNumber { get; set; }
        public int CuissonNumber { get; set; } 
        public int EmballageNumber { get; set; } 
        public int ReadyNumber { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates CakeFactory object and initiates its properties
        /// </summary>
        public CakeFactory()
        {
            Cakes = new List<Cake>();

            prepaToken = new CancellationTokenSource();
            cuissonToken = new CancellationTokenSource();
            embToken = new CancellationTokenSource();

            prepaTask = new Task(() => Preparation(), prepaToken.Token);
            cuissonTask = new Task(() => Cuisson(), cuissonToken.Token);
            embTask = new Task(() => Emballage(), embToken.Token);

            PreparationNumber = CuissonNumber = EmballageNumber = ReadyNumber = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the cake's creation process
        /// </summary>
        public void Run()
        {
            prepaTask.Start();
            prepaState = StageState.Running;

            cuissonTask.Start();
            cuissonState = StageState.Running;

            embTask.Start();
            embState = StageState.Running;
        }

        /// <summary>
        /// Stops the cake's creation process
        /// </summary>
        public void Stop()
        {
            prepaState = StageState.Stopped;
            prepaToken.Cancel();
            prepaToken.Dispose();

            cuissonState = StageState.Stopped;
            cuissonToken.Cancel();
            cuissonToken.Dispose();

            embState = StageState.Stopped;
            embToken.Cancel();
            embToken.Dispose();
        }

        /// <summary>
        /// Prepares the Cakes
        /// </summary>
        private void Preparation()
        {
            List<Task> preparationTasks;
            while (prepaState == StageState.Running)
            {
                preparationTasks = new List<Task>();

                // Filling the CakesToPrepare list with the allowed number of cakes
                for (int i = 0; i < maxSimultaneousPreparation; i++)
                {
                    Cakes.Add(new Cake());
                }

                IEnumerable<Cake> prepaCakes = Cakes.Where(c => c.State == State.Born).ToList();        // queue list of the cakes to prepare
                foreach (Cake cake in prepaCakes)
                {
                    preparationTasks.Add(cake.PreparationAsync());
                    PreparationNumber++;
                }
                Task.WaitAll(preparationTasks.ToArray());

                PreparationNumber -= preparationTasks.Count;
            }
        }

        /// <summary>
        /// Bakes the Cakes
        /// </summary>
        private void Cuisson()
        {
            List<Task> cuissonTasks;
            while (cuissonState == StageState.Running)
            {
                cuissonTasks = new List<Task>();
                IEnumerable<Cake> cuissonCakes = Cakes.Where(c => c.State == State.Prepared).ToList();  // queue list of the cakes to bake
                foreach (Cake cake in cuissonCakes.Take(maxSimultaneousCuisson))                        // limiting baking to the maximum allowed
                {
                    cuissonTasks.Add(cake.CuissonAsync());
                    CuissonNumber++;
                }
                Task.WaitAll(cuissonTasks.ToArray());

                CuissonNumber -= cuissonTasks.Count;
            }
        }

        /// <summary>
        /// Packs the Cakes
        /// </summary>
        private void Emballage()
        {
            List<Task> embTasks;
            while (embState == StageState.Running)
            {
                embTasks = new List<Task>();
                IEnumerable<Cake> embCakes = Cakes.Where(c => c.State == State.Baked).ToList();         // queue list of the cakes to bake
                foreach (Cake cake in embCakes.Take(maxSimultaneousEmballage))                          // limiting packing to the maximum allowed
                {
                    embTasks.Add(cake.EmballageAsync());
                    EmballageNumber++;
                }
                Task.WaitAll(embTasks.ToArray());

                ReadyNumber += EmballageNumber;
                EmballageNumber -= embTasks.Count;
            }
        }
        #endregion
    }

    public enum StageState { Running, Stopped }
    public enum Stage { Preparation, Cuisson, Emballage }
}
