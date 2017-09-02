using UnityEngine;
using UnityEngine.UI;
using MathBeat.Core;
using System.Collections;

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
        public float CorrectPoints = 100;
        public float WrongPoints = 0;

        [Header("Text Views")]
        public Text ScoreText;
        public Text MultiplierText, 
                    HighScoreText, 
                    HitStatusText,
                    ChainStatusText,
                    ChainText;

        public UIController StatusBox;

        private const string HighScorePrefsFormat = "HighScore[{0}]";
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

        enum HitType
        {
            Bad = -2, Miss, Perfect, Great, Good
        }

        public void Respond(int errorCode, float hitPoints = 0f)
        {
            const float THRESHOLD_PERFECT = .2f;
            const float THRESHOLD_GREAT = .75f;
            const float THRESHOLD_GOOD = 1.85f;
            int _points = 0;
            float hitX = 0f;

            

            // handling combos
            if(errorCode < 0 || hitPoints > 1.5f)
            {
                Multiplier = 1.0f;
                Chain = 0;
                hitStatus = HitType.Miss;
            }
            else
            {
                if (++Chain <= 30)
                    Multiplier = 1 + Chain / 10;
            }

            switch (errorCode)
            {
                case CODE_HIT:
                    if (hitPoints <= THRESHOLD_PERFECT)
                    {
                        hitX = 1f;
                        hitStatus = HitType.Perfect;
                    }
                    else if (hitPoints <= THRESHOLD_GREAT)
                    {
                        hitX = (THRESHOLD_GOOD - hitPoints) / THRESHOLD_GOOD;
                        hitStatus = HitType.Great;
                    }
                    else if (hitPoints <= THRESHOLD_GOOD)
                    {
                        hitX = (THRESHOLD_GOOD - hitPoints) / THRESHOLD_GOOD;
                        hitStatus = HitType.Good;
                    }
                    else
                    {
                        hitX = (THRESHOLD_GOOD - hitPoints) / THRESHOLD_GOOD;
                        hitStatus = HitType.Bad;
                    }

                    if (hitX < 0) hitX = 0;

                    _points = Mathf.RoundToInt(CorrectPoints * Multiplier * hitX);
                    Log.Debug(hitStatus.ToString() + "! Got " + _points + " points!");
                    main.PlayFX("Correct");
                    break;

                case CODE_OFF:
                    Log.Debug("Miss! Multiplier reset.");
                    main.PlayFX("OffBeat");
                    break;

                case CODE_MISS:
                    Log.Debug("Miss! Multiplier reset.");
                    main.PlayFX("Miss");
                    break;

                case CODE_WRONG:
                    Log.Debug("Wrong answer! Multiplier reset.");
                    main.PlayFX("Wrong");
                    break;

                default:
                    break;
            }
            
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
            ChainStatusText.text = ChainText.text = Chain.ToString();
            HitStatusText.text = hitStatus.ToString();
            StatusBox.Show();
            yield return new WaitForSeconds(1);
            StatusBox.Hide();
            coroutineRunning = false;
        }

        public void LoadHighScore(int musicId)
        {
            string key = string.Format(HighScorePrefsFormat, musicId);
            HighScore = PlayerPrefs.GetInt(key);
        }

        public void SaveHighScore(int musicId)
        {
            string key = string.Format(HighScorePrefsFormat, musicId);
            if(Score > HighScore)
            {
                PlayerPrefs.SetInt(key, Score);
            }
        }

    } 
}
