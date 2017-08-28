using UnityEngine;
using System.Collections;

namespace MathBeat.Game.BeatSync
{
    /// <summary>
    /// Syncs the audio with the beat spawner
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioSync : MonoBehaviour
    {
        public float BPM = 120f;
        [Tooltip("Wait time to play the audio.")]
        public float StartDelay = 3f;
        [Tooltip("Ofsett time, useful to wait for a block to hit the trigger.")]
        public float TimeOffset = 0f;

        public delegate void OnAudioStart(double syncTime);
        public event OnAudioStart AudioStart;

        // Use this for initialization
        void Start()
        {
            double initTime = AudioSettings.dspTime;
            GetComponent<AudioSource>().PlayScheduled(initTime + StartDelay + TimeOffset);
            if (AudioStart != null)
            {
                Core.Log.Debug("Starting audio analysis...");
                AudioStart(initTime + StartDelay);
            }
        }

    }
}
