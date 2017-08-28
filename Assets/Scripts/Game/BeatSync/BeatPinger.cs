using UnityEngine;
using MathBeat.Core;
using Values = MathBeat.Core.BeatValue;
using System.Collections;
using System;

namespace MathBeat.Game.BeatSync
{
    public class BeatPinger : MonoBehaviour, IPinger
    {
        [Header("Sync Options")]
        public BeatValueType BeatValue = BeatValueType.Beat;
        [Range(0, 30)]
        public float OffsetTime = 0f;
        [Range(0, 500)]
        [Tooltip("Time to wait for checks (in milliseconds)")]
        public float PingDelay = 1000 / 60;
        [Range(0, 1000f)]
        public float Latency = 0f;

        [Header("Audio and Observers")]
        public AudioSource Audio;
        public AudioSync Syncer;
        public BeatObserver[] Observers;

        private double _next = 0f, _current = 0f, _period, _offset;
        private int freq;


        /// <summary>
        /// Preparing pinger
        /// </summary>
        void Awake()
        {
            // preparing variables
            if (Audio == null)
                Audio = FindObjectOfType<AudioSource>();
            Syncer = Audio.GetComponent<AudioSync>();
            float audioBpm = Syncer.BPM;
            freq = Audio.clip.frequency;

            // setting _period and _offset
            _period = (60f / audioBpm * Values.Values[(int)BeatValue] + Latency / 1000) * freq;
            _offset = OffsetTime * freq;

            // logging the details
            Log.Debug("Audio clip {1} is in {0} Hz.", freq, Audio.clip);
            Log.Debug("Period is {0} s / {1} samples", _period / freq, _period);
            Log.Debug("Offset is {0} s / {1} samples", OffsetTime, freq);
        }

        /// <summary>
        /// Subscribes Ping to AudioStart event
        /// </summary>
        public void OnEnable()
        {
            Syncer.AudioStart += Ping;
        }

        /// <summary>
        /// Unsubscribes Ping from AudioStart event
        /// </summary>
        public void OnDisable()
        {
            Syncer.AudioStart -= Ping;
        }

        /// <summary>
        /// Starts the ping coroutine
        /// </summary>
        /// <param name="syncTime">Init time</param>
        public void Ping(double syncTime)
        {
            _next = (syncTime * freq);
            //StartCoroutine(PingAsync());
        }

        private void Update()
        {
            // Sets the current position in samples
            // with the position of the currently playing audio.
            // This makes it independent from the
            // game's frame rate, which is important.
            _current = AudioSettings.dspTime * freq;

            // Compares the current sample with
            // the next sample + offset
            if (_current >= (_next + _offset))
            {
                foreach (var observer in Observers)
                    observer.Ping();
                _next += _period;
            }
        }

        public IEnumerator PingAsync()
        {
            // keep the coroutine playing...
            while (Audio.isPlaying)
            {
                // Sets the current position in samples
                // with the position of the currently playing audio.
                // This makes it independent from the
                // game's frame rate, which is important.
                _current = AudioSettings.dspTime * freq;

                // Compares the current sample with
                // the next sample + offset
                if (_current >= (_next + _offset))
                {
                    foreach (var observer in Observers)
                        observer.Ping();
                    _next += _period;
                }

                // Wait for PingDelay ms before sending another ping
                yield return new WaitForSeconds(PingDelay / 1000f);
            }
        }
    }
}
