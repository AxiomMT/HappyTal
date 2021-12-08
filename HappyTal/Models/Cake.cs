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
        public State State { get; set; }
        #endregion

        #region Constructors
        public Cake()
        {
            State = State.Born;
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
            int lowDurationBound = 5;
            int topDurationBound = 8;
            int randomDuration = new Random().Next(lowDurationBound, topDurationBound + 1);     // The MaxValue parameter is exclusive
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
            int duration = 10;

            Task.Delay(TimeSpan.FromSeconds(duration)).Wait();
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
            int duration = 2;

            Task.Delay(TimeSpan.FromSeconds(duration)).Wait();
            State = State.Packed;
            return State;
        }
        #endregion
    }

    // The cake's state
    public enum State { Born, Prepared, Baked, Packed }
}
