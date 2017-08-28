using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace MathBeat.Game.BeatSync
{
    public interface IPinger
    {
        /// <summary>
        /// What to do when the object is enabled
        /// </summary>
        void OnEnable();
        /// <summary>
        /// What to do when the object is disabled
        /// </summary>
        void OnDisable();
        /// <summary>
        /// Prepare to ping
        /// </summary>
        /// <param name="syncTime">Init time from dspTime + StartDelay></param>
        void Ping(double syncTime);
        /// <summary>
        /// A coroutine to ping the observers
        /// </summary>
        /// <returns></returns>
        IEnumerator PingAsync();
    }
}
