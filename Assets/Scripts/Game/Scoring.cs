using UnityEngine;
using UnityEngine.UI;
using MathBeat.Core;
using System.Collections;
using System.Collections.Generic;

namespace MathBeat.Game
{
    public class Scoring : MonoBehaviour
    {
        [Header("Score")]
        public int Score = 0;
        public int HighScore = 0;
        public int Chain = 0;
        public float Multiplier = 1.0f;

        [Header("Rewards")]
        [Tooltip("Score when correct answer is given")]
        public int CorrectPoints = 500;
        [Tooltip("Score when hitting a note beat")]
        public int NotePoints = 100;
        [Tooltip("Score when a beat is missed or wrong answer is given")]
        public int WrongPoints = 0;

        [Header("Text Views")]
        public Text ScoreText;
        public Text MultiplierText,
                    HighScoreText,
                    HitStatusText,
                    ChainStatusText,
                    ChainText;

        public UIController StatusBox;

        public Dictionary<HitType, int> HitCounters;

        private const string highScorePrefsFormat = "HighScore[{0}]";
        private Main main;
        private HitType hitStatus = HitType.Miss;
        private Coroutine coroutine;
        private bool coroutineRunning = false;

        public const int CODE_HIT = 0;
        public const int CODE_MISS = -1;
        public const int CODE_OFF = -2;
        public const int CODE_WRONG = -3;

        private void Awake()
        {
            main = FindObjectOfType<Main>();
        }

        // Use this for initialization
        void Start()
        {
            HitCounters = new Dictionary<HitType, int>();
            HitCounters.Add(HitType.Wrong, 0);
            HitCounters.Add(HitType.Bad, 0);
            HitCounters.Add(HitType.Miss, 0);
            HitCounters.Add(HitType.Perfect, 0);
            HitCounters.Add(HitType.Great, 0);
            HitCounters.Add(HitType.Good, 0);
            HitCounters.Add(HitType.Correct, 0);

            Score = 0;
            Multiplier = 1.0f;
        }

        // Update is called once per frame
        // But we don't need it
        // Because updating the text every frame
        // would be a pain in the ***.
        // void Update()
        // {

        // }
        // So we greened it out. /* get it? */

        public enum HitType
        {
            Wrong = -3, Bad = -2, Miss, Perfect, Great, Good, Correct
        }

        public void Respond(int errorCode, float hitPoints = 0f, string tag = "")
        {
            const float THRESHOLD_PERFECT = .5f;
            const float THRESHOLD_GREAT = 1f;
            const float THRESHOLD_GOOD = 2f;
            int _points = 0;
            int _basePoints = NotePoints;
            float hitX = 0f;

            if (tag == "Question")
                _basePoints = CorrectPoints;

            // handling combos
            if (errorCode < 0 || hitPoints > 1.5f)
            {
                Multiplier = 1.0f;
                Chain = 0;
            }
            else
            {
                if (++Chain <= 30)
                    Multiplier = 1 + Chain / 10;
            }

            switch (errorCode)
            {
                case CODE_HIT:
                    HitCounters[HitType.Correct]++;
                    if (hitPoints <= THRESHOLD_PERFECT)
                    {
                        hitX = 1f;
                        hitStatus = HitType.Perfect;
                    }
                    else if (hitPoints <= THRESHOLD_GREAT)
                    {
                        hitX = (THRESHOLD_GOOD - hitPoints) / (THRESHOLD_GOOD - THRESHOLD_PERFECT);
                        hitStatus = HitType.Great;
                    }
                    else if (hitPoints <= THRESHOLD_GOOD)
                    {
                        hitX = (THRESHOLD_GOOD - hitPoints) / (THRESHOLD_GOOD - THRESHOLD_PERFECT);
                        hitStatus = HitType.Good;
                    }
                    else
                    {
                        hitX = .01f;
                        hitStatus = HitType.Bad;
                    }

                    if (hitX < 0) hitX = 0;

                    _points = Mathf.RoundToInt(_basePoints * Multiplier * hitX);
                    Log.Debug(hitStatus.ToString() + "! Got " + _points + " points!");
                    main.PlayFX("Correct");
                    break;

                case CODE_OFF:
                    Log.Debug("Miss! Multiplier reset.");
                    main.PlayFX("OffBeat");
                    break;

                case CODE_MISS:
                    Log.Debug("Miss! Multiplier reset.");
                    hitStatus = HitType.Miss;
                    main.PlayFX("Miss");
                    break;

                case CODE_WRONG:
                    Log.Debug("Wrong answer! Multiplier reset.");
                    hitStatus = HitType.Wrong;
                    HitCounters[HitType.Miss]++;
                    main.PlayFX("Wrong");
                    break;

                default:
                    break;
            }

            HitCounters[hitStatus]++;
            Score += _points;
            if (coroutineRunning)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(UpdateText());
        }

        IEnumerator UpdateText()
        {
            coroutineRunning = true;
            ScoreText.text = Score.ToString();
            MultiplierText.text = System.Math.Round(Multiplier, 2).ToString() + "x";
            ChainText.text = Chain.ToString();
            ChainStatusText.text = Chain > 1 ? Chain.ToString() : string.Empty;
            HitStatusText.text = hitStatus.ToString();
            StatusBox.Show();
            yield return new WaitForSeconds(1);
            StatusBox.Hide();
            coroutineRunning = false;
        }

        public void LoadHighScore(int musicId)
        {
            string key = string.Format(highScorePrefsFormat, musicId);
            HighScore = PlayerPrefs.GetInt(key);
        }

        public void SaveHighScore(int musicId)
        {
            string key = string.Format(highScorePrefsFormat, musicId);
            if (Score > HighScore)
            {
                PlayerPrefs.SetInt(key, Score);
            }
        }

    }
}
