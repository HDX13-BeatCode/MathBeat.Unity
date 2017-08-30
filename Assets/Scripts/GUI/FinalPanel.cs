using UnityEngine;
using UnityEngine.UI;
using MathBeat.Game;
using MathBeat.Core;
using System.Collections;

namespace MathBeat.GUI
{
    public class FinalPanel : MonoBehaviour
    {
        public Text
            FinalScore,
            HighScore,
            PerfectCount,
            GreatCount,
            GoodCount,
            BadCount,
            MissCount,
            CorrectCount,
            WrongCount;

        public Scoring Scoring;

        // Use this for initialization
        void Start()
        {
            Scoring = FindObjectOfType<Scoring>();
        }

        public void SetTexts()
        {
            FinalScore.text = Scoring.Score.ToString();
            HighScore.text = Scoring.HighScore.ToString();
            PerfectCount.text = Scoring.HitCounters[Scoring.HitType.Perfect].ToString();
            GreatCount.text = Scoring.HitCounters[Scoring.HitType.Great].ToString();
            GoodCount.text = Scoring.HitCounters[Scoring.HitType.Good].ToString();
            BadCount.text = Scoring.HitCounters[Scoring.HitType.Bad].ToString();
            MissCount.text = Scoring.HitCounters[Scoring.HitType.Miss].ToString();
            CorrectCount.text = Scoring.HitCounters[Scoring.HitType.Correct].ToString();
            WrongCount.text = Scoring.HitCounters[Scoring.HitType.Wrong].ToString();
        }
    } 
}
