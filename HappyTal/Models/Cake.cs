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
        public async Task PreparationAsync()
        {
            int lowDurationBound = 5;
            int topDurationBound = 8;
            int randomDuration = new Random().Next(lowDurationBound, topDurationBound + 1);     // The MaxValue parameter is exclusive

            await Task.Delay(TimeSpan.FromSeconds(randomDuration));
            State = State.Prepared;
        }

        /// <summary>
        /// Bakes the cake
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task CuissonAsync()
        {
            int duration = 10;

            await Task.Delay(TimeSpan.FromSeconds(duration));
            State = State.Baked;
        }

        /// <summary>
        /// Packs the cake
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        public async Task EmballageAsync()
        {
            int duration = 2;

            await Task.Delay(TimeSpan.FromSeconds(duration));
            State = State.Packed;
        }
        #endregion
    }

    // The cake's state
    public enum State { Born, Prepared, Baked, Packed }
}
