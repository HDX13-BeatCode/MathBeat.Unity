using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathBeat.Core
{
    [Serializable]
    public class BeatMapData
    {
        public int Difficulty { get; set; }
        public int BeatSnap { private get; set; }
        public BeatValueType Snap
        {
            get { return (BeatValueType)BeatSnap; }
        }
        public string[] BeatMap
        {
            set
            {
                _maps.SetBeatMap(value);
            }
        }
        private BeatTypeList _maps;
        public BeatTypeList Maps
        {
            get
            {
                return _maps;
            }
        }

        public BeatMapData()
        {
            _maps = new BeatTypeList();
        }
    }

    [Serializable]
    public class BeatTypeList
    {
        private List<BeatType> _list;
        const string IndexChar = "0123456789QWERTYUIOPASDFGHJKLZXCVBNM";

        public BeatTypeList()
        {
            _list = new List<BeatType>();
        }

        public void SetBeatMap(string[] mapstring)
        {
            List<BeatType> temp = new List<BeatType>();
            foreach (var item in mapstring)
            {
                foreach (var c in item)
                {
                    if (!IsIndex(c)) continue;

                    temp.Add((BeatType)Char2Index(c));
                }
            }
            _list = temp;
        }

        public BeatType this[int beatPosition]
        {
            get
            {
                return _list[beatPosition];
            }
        }

        public int Length
        {
            get
            {
                return _list.Count;
            }
        }
        private bool IsIndex(char c)
        {
            return IndexChar.Contains(c);
        }

        private int Char2Index(char c)
        {
            return IndexChar.IndexOf(c);
        }



        //public override string ToString()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    string[] beatNames = Enum.GetNames(typeof(BeatType));
        //    char[] beatCode = new char[beatNames.Length];
        //    for (int i = 0; i < beatNames.Length; i++)
        //    {
        //        beatCode[i] = beatNames[i][0];
        //    }

        //    builder.Append("");
        //    for (int i = -1; i < _list.Count; i++)
        //    {
        //        if(i == -1)
        //        {
        //            for (int j = 1; j < 10; j++)
        //                builder.Append($"{j}---");
        //            for (int j = 10; j <= 16; j++)
        //                builder.Append($"{j}--");
        //        }
        //        else
        //        {
        //            builder.Append(beatCode[(int)_list[i]]);
        //            if ((i + 1) % 4 == 0) builder.Append(' ');
        //            if ((i + 1) % 16 == 0) builder.Append('\n');
        //        }
        //        builder.Append('\n');
        //    }
        //    return builder.ToString();
        //}
    }

    [Serializable]
    public enum BeatType
    {
        None,
        Single,
        Double,
        Block
    }

    [Serializable]
    public enum BeatValueType
    {
        None,
        QuarterStep,
        QuarterDotStep,
        HalfStep,
        HalfDotStep,
        Step,
        DotStep,
        QuarterBeat = Step,
        QuarterDotBeat = DotStep,
        HalfBeat,
        HalfDotBeat,
        Beat,
        DotBeat,
        QuarterBar = Beat,
        QuarterDotBar = DotBeat,
        HalfBar,
        HalfDotBar,
        Bar,
        DotBar
    }

    [Serializable]
    public struct BeatValue
    {
        const float dotModifier = 1.5f;

        public static float[] Values =
        {
            0f,                         /* None           (00)               */
            16f,                        /* QuarterStep    (01)               */
            16f / dotModifier,          /* QuarterDotStep (02)               */
            8f,                         /* HalfStep       (03)               */
            8f / dotModifier,           /* HalfDotStep    (04)               */
            4f,                         /* Step           (05) QuarterBeat   */
            4f / dotModifier,           /* DotStep        (06) QuarterDotBeat*/
            2f,                         /* HalfBeat       (07)               */
            2f / dotModifier,           /* HalfDotBeat    (08)               */
            1f,                         /* Beat           (09) QuarterBar    */
            1f / dotModifier,           /* DotBeat        (10) QuarterDotBar */
            .5f,                        /* HalfBar        (11)               */
            .5f / dotModifier,          /* HalfDotBar     (12)               */
            .25f,                       /* Bar            (13)               */
            .25f / dotModifier,         /* DotBar         (14)               */
        };

        public float this[BeatValueType type]
        {
            get
            {
                return Values[(uint)type];
            }
        }
    }
}
