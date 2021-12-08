using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappyTal.Models
{
    public class Cake
    {
        // N.B: We consider each cake making stage (Preparation, Cuisson, Emballage) as an individual task that can be awaited.

        #region Properties
        //------ Duration properties
        int PrepareLowDurationBound = 5;
        int PrepareTopDurationBound = 8;
        int CuissonDuration = 10;
        int EmballageDuration = 2;

        public State State { get; set; }
        #endregion

        #region Constructors
        public Cake()
        {
            State = State.Born;
        }

        public Cake(int prepareLowDurationBound, int prepareTopDurationBound, int cuissonDuration, int emballageDuration) : this()
        {
            PrepareLowDurationBound = prepareLowDurationBound;
            PrepareTopDurationBound = prepareTopDurationBound;
            CuissonDuration = cuissonDuration;
            EmballageDuration = emballageDuration;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prepares the cake (first stage)
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task<State> PreparationAsync()
        {
            return await Task.Run(() => Preparation());
        }

        /// <summary>
        /// Preparation Stage
        /// </summary>
        /// <returns>The State of the Cake that went through the stage</returns>
        private State Preparation()
        {
            int randomDuration = new Random().Next(PrepareLowDurationBound, PrepareTopDurationBound + 1);     // The MaxValue parameter is exclusive
            Task.Delay(TimeSpan.FromSeconds(randomDuration)).Wait();
            State = State.Prepared;
            return State;
        }

        /// <summary>
        /// Bakes the cake
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task<State> CuissonAsync()
        {
            return await Task.Run(() => Cuisson());
        }

        /// <summary>
        /// Cuisson Stage
        /// </summary>
        /// <returns>The State of the Cake that went through the stage</returns>
        private State Cuisson()
        {
            Task.Delay(TimeSpan.FromSeconds(CuissonDuration)).Wait();
            State = State.Baked;
            return State;
        }

        /// <summary>
        /// Packs the cake
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task<State> EmballageAsync()
        {
            return await Task.Run(() => Emballage());
        }

        /// <summary>
        /// Emballage Stage
        /// </summary>
        /// <returns>The State of the Cake that went through the stage</returns>
        private State Emballage()
        {
            Task.Delay(TimeSpan.FromSeconds(EmballageDuration)).Wait();
            State = State.Packed;
            return State;
        }
        #endregion
    }

    // The cake's state
    public enum State { Born, Prepared, Baked, Packed }
}
