using UnityEngine;
using System.Collections;
using MathBeat.Core;

namespace MathBeat.Game.BeatSync
{
    /// <summary>
    /// Observes the ping from <see cref="BeatPinger"/>
    /// </summary>
    public class BeatObserver : MonoBehaviour
    {
        private bool isActive = false;
        /// <summary>
        /// Checks if the observer receives ping
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        /// <summary>
        /// Time to let the ping active (in milliseconds)
        /// </summary>
        [Range(0, 500)]
        [Tooltip("Time to let the ping active (in milliseconds)")]
        public float WaitTime = 1000 / 30;

        /// <summary>
        /// Sends a ping signal to the observer
        /// </summary>
        /// <returns></returns>
        public void Ping()
        {
            isActive = true;
            StartCoroutine(ClearPing());
        }

        IEnumerator ClearPing()
        {
            yield return new WaitForSeconds(WaitTime / 1000f);
            isActive = false;
        }

        public IEnumerator Wait()
        {
            while (!IsActive)
                yield return null;
        }
    }
}
