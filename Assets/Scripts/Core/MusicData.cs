using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathBeat.Core
{
    [Serializable]
    public class MusicData
    {
        
        /// <summary>
        /// Title of the music
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Music artist
        /// </summary>
        public string Artist { get; set; }

        private float _bpm;

        /// <summary>
        /// Music speed/BPM
        /// </summary>
        public float BPM
        {
            get
            {
                if (Properties.Contains(MusicProperties.HalfSpeed) && Properties.Contains(MusicProperties.DoubleSpeed))
                    return _bpm;
                else if (Properties.Contains(MusicProperties.HalfSpeed))
                    return _bpm / 2;
                else if (Properties.Contains(MusicProperties.DoubleSpeed))
                    return _bpm * 2;
                else return _bpm;
            }
            set
            {
                _bpm = value;
            }
        }

        internal float GameBpm { get { return _bpm; } }
        /// <summary>
        /// Playback offset
        /// </summary>
        public float Offset { get; set; }
        
        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File name without extension
        /// </summary>
        public string FileNameNoExt { get
        {
                return FileName.Substring(0, FileName.LastIndexOf('.'));
        } }

        /// <summary>
        /// The high score of the current track
        /// </summary>
        /// Use UnityEngine.PlayerPrefs for this.
        public uint HighScore { get; set; }

        /// <summary>
        /// The positions to hit the beats
        /// </summary>
        public int[] TrackBeats { get; set; }

        /// <summary>
        /// Properties to hit the beats
        /// </summary>
        public enum MusicProperties { HalfSpeed, DoubleSpeed }

        /// <summary>
        /// A write-only properties in format of "hd..."
        /// </summary>
        public string properties { private get; set; }

        /// <summary>
        /// Properties for the music
        /// </summary>
        /// 
        /// An example of this is on Fade (lite).
        /// It has a 2/4 bar, and it can be manipulated
        /// by doubling the BPM (in the BPM properties)
        /// from 90 to 180
        /// and then use the Half Speed properties
        /// so it can be considered that the music
        /// has 90 BPM, while the game thinks it's 180.
        public MusicProperties[] Properties
        {
            get
            {
                List<MusicProperties> output = new List<MusicProperties>();
                foreach (char opt in properties.ToCharArray())
                {
                    switch (opt)
                    {
                        case 'h':
                            output.Add(MusicProperties.HalfSpeed);
                            break;
                        case 'd':
                            output.Add(MusicProperties.DoubleSpeed);
                            break;
                        default:
                            break;
                    }
                }
                return output.ToArray();
            }
        }
    }

    [Serializable]
    public class MusicList
    {
        public List<MusicData> MusicData { get; set; }
        public MusicList()
        {
            MusicData = new List<MusicData>();
        }
    }
}
