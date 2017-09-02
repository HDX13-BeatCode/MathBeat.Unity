using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MathBeat.Core;

namespace MathBeat.GUI
{
    public class TrackListAdapter : BaseAdapter
    {
        [SerializeField]
        private Text txtArtistTitle;

        [SerializeField]
        private Text txtBPM;

        public void SetArtistTitle(string artist, string title)
        {
            txtArtistTitle.text = artist + " - " + title;
        }

        public void SetBpmText(float bpm)
        {
            txtBPM.text = bpm.ToString();
        }

    }
}
