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

        public List<Cake> CakesToPrepare, CakesToBake, CakesToPack;     // Queue lists of cakes waiting for their corresopnding stage

        // "Real-time" counting the number of Cakes going through each step
        public int PreparationNumber { get; set; }
        public int CuissonNumber { get; set; } 
        public int EmballageNumber { get; set; } 
        public int ReadyNumber { get; set; }

        // To prevent undesired parallel stages side-effect when handling the Cakes lists, we add an enum condition. 
        // e.g: The CakesToBake are both filled during the Preparing stage and emptied in the Baking stage. We help this from happening "at the same time" 
        private Stage allowedStage;

        public bool IsStarted = false; // True when the first stage process really starts. Triggered by the display features.

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiates CakeFactory object and initiates its properties
        /// </summary>
        public CakeFactory()
        {
            CakesToPrepare = new List<Cake>();
            CakesToBake = new List<Cake>();
            CakesToPack = new List<Cake>();

            prepaToken = new CancellationTokenSource();
            cuissonToken = new CancellationTokenSource();
            embToken = new CancellationTokenSource();

            prepaTask = new Task(() => Preparation(), prepaToken.Token);
            cuissonTask = new Task(() => Cuisson(), cuissonToken.Token);
            embTask = new Task(() => Emballage(), embToken.Token);

            PreparationNumber = CuissonNumber = EmballageNumber = ReadyNumber = 0;

            allowedStage = Stage.Preparation;
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
                if (allowedStage == Stage.Preparation)
                {
                    preparationTasks = new List<Task>();

                    // Filling the CakesToPrepare list with the allowed number of cakes
                    for (int i = 0; i < maxSimultaneousPreparation; i++) CakesToPrepare.Add(new Cake());
                    foreach (Cake cake in CakesToPrepare)
                    {
                        preparationTasks.Add(cake.PreparationAsync());
                        PreparationNumber++;
                    }

                    if (!IsStarted) IsStarted = true;

                    CakesToBake.AddRange(CakesToPrepare);
                    allowedStage = Stage.Cuisson;

                    Task.WaitAll(preparationTasks.ToArray());

                    CakesToPrepare.Clear();
                    PreparationNumber -= preparationTasks.Count;
                }
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
                if (allowedStage == Stage.Cuisson)
                {
                    cuissonTasks = new List<Task>();
                    foreach (Cake cake in CakesToBake.Take(maxSimultaneousCuisson))
                    {
                        cuissonTasks.Add(cake.CuissonAsync());
                        CuissonNumber++;
                    }

                    CakesToPack = CakesToBake.Take(cuissonTasks.Count).ToList();
                    allowedStage = Stage.Emballage;

                    Task.WaitAll(cuissonTasks.ToArray());

                    CakesToBake = CakesToBake.Skip(cuissonTasks.Count).ToList();
                    CuissonNumber -= cuissonTasks.Count;
                }
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
                if (allowedStage == Stage.Emballage)
                {
                    embTasks = new List<Task>();
                    foreach (Cake cake in CakesToPack.Take(maxSimultaneousEmballage))
                    {
                        embTasks.Add(cake.EmballageAsync());
                        EmballageNumber++;
                    }

                    CakesToPack = CakesToPack.Skip(embTasks.Count).ToList();
                    allowedStage = Stage.Preparation;

                    Task.WaitAll(embTasks.ToArray());

                    ReadyNumber += EmballageNumber;
                    EmballageNumber -= embTasks.Count;
                }
            }
        }
        #endregion
    }

    public enum StageState { Running, Stopped }
    public enum Stage { Preparation, Cuisson, Emballage }
}
