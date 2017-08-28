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
        /// Artist/producer/whatever
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Speed in beats per minute
        /// </summary>
        public float BPM { get; set; }

        /// <summary>
        /// Value of offset (in seconds)
        /// </summary>
        public float Offset { get; set; }

        /// <summary>
        /// File names of the track
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Beat map data list/array
        /// </summary>
        public BeatMapData[] MapData { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Title, Artist);
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

    [Serializable]
    public class InvalidLengthException : Exception
    {
        public InvalidLengthException() : base("Invalid track length was set!") { }
        protected InvalidLengthException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
