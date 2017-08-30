using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MathBeat.Core;
using GameLog = MathBeat.Core.Log;
using MathBeat.Game.BeatSync;
using System;

namespace MathBeat.Game
{
    /// <summary>
    /// Handles the main game system, like data, score, etc
    /// </summary>
    public class Main : MonoBehaviour
    {
        #region Fields
        #region DEBUG
        [Header("DEBUG")]
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //private float lastNoteTime = 0;
        // public DebugPanel DebugPanel;

#endif
        private bool isTest = true;
        //[SerializeField]
        //private float position = 0f;

        [SerializeField]
        private float testOffset = 1;

        const int ms = 1000;

        [SerializeField]
        private BeatMapData testMap = new BeatMapData()
        {
            Difficulty = 1,
            BeatSnap = (int)BeatValueType.Beat,
            BeatMap = new string[] { "1130 1120", "1130 1120", "1131 1121", "1131 1121" }
        };

        #endregion DEBUG

        #region GameplayData
        [Header("Gameplay Data")]

        /// <summary>
        /// The BPM of the track, 
        /// may not be the actual tempo
        /// </summary>
        public float BPM;

        /// <summary>
        /// Condition to respawn another note
        /// </summary>
        public bool ResetTimer = true;

        public Scoring ScoreSystem;

        /// <summary>
        /// The data of currently playing music
        /// </summary>
        public MusicData NowPlaying;

        public QuizSystem QuizData;

        [Range(1,3)]
        public int QuestionDifficulty = 1;

        [Range(0,2)]
        public int GameDifficulty = 0;

        [Range(4, 16)]
        public int Speed = 4;

        /// <summary>
        /// Time needed for a block to fall
        /// </summary>
        private float FallTime;

        /// <summary>
        /// The music player
        /// </summary>
        public AudioSource Music;

        /// <summary>
        /// <see cref="Music"/>.<see cref="AudioSync"/>
        /// </summary>
        public AudioSync Syncer;

        /// <summary>
        /// The music clip to be played
        /// </summary>
        private AudioClip music2Play;

        public Text FinalScoreText;

        #endregion GameplayData

        #region System
        /// <summary>
        /// Contains the data for the game
        /// </summary>
        [Header("Others")]
        public GameHandler GameData;

        public BlockSpawner Spawner;

        public GameObject GameOverScreen;

        public GameObject PausePanel;

        public BeatPinger Pinger;

        public GUI.FinalPanel FinalPanel;
        // Events
        /// <summary>
        /// What to do when the game is paused?
        /// </summary>
        /// <param name="isPaused"></param>
        public delegate void OnPause(bool isPaused);

        /// <summary>
        /// What to do when the game is paused?
        /// </summary>
        public event OnPause Pause;

        public bool IsPlaying;

        /// <summary>
        /// Checks if the starts late from the music
        /// (MusicData.Offset > FallTime)
        /// </summary>
        bool lateStart;

        bool gameOver = false;
        #endregion Others
        #endregion Fields

        /// <summary>
        /// Called when the scene is active
        /// </summary>
        private void Awake()
        {
            GameLog.Debug(ToString() + " has awaken at " + DateTime.Now);
            #region FromStart
            try
            {
                // preparing objects
                GameLog.Debug("Preparing data at " + DateTime.Now);
                GameData = FindObjectOfType<GameHandler>(); // if this fails, go to catch line
                music2Play = FindObjectOfType<AudioClip>();

                // preparing data
                NowPlaying = GameData.GetCurrentMusic();
                QuizData = GameData.QuizSystem;
                Speed = GameData.GameSpeed;
                GameDifficulty = GameData.GameDifficulty;
                QuestionDifficulty = GameData.QuestionDifficulty;
                isTest = false;

                music2Play = GameHandler.LoadAudio(NowPlaying.FileName);
                Music.clip = music2Play;
            }
            #endregion
            #region TestTrack
            catch (Exception)
            {
                // this will be a test script
                // so you don't need to start
                // all the way from the beginning
                // PS: I've set Taffy as default, so
                // no more FileName on MusicData
                isTest = true;
                GameDifficulty = 0; // because I only have one BeatMapData: testMap
                NowPlaying = new MusicData()
                {
                    Title = "TestTrack",
                    Artist = "<unknown/>", // dat html/xml!
                    BPM = 120,
                    Offset = testOffset,
                    MapData = new List<BeatMapData>() { testMap }
                };
                QuizData.Quiz = GameHandler.LoadJSON<List<Quiz>>("Quiz");

            }
            #endregion
            #region Finally
            finally
            {
                //these lines will be executed no matter what
                Syncer = Music.GetComponent<AudioSync>();
                Pinger = Music.GetComponent<BeatPinger>();
                ScoreSystem = FindObjectOfType<Scoring>();
                Spawner = FindObjectOfType<BlockSpawner>();
                FinalPanel = GameOverScreen.GetComponent<GUI.FinalPanel>();
                Spawner.MapData = NowPlaying.MapData[GameDifficulty].Maps;
                Spawner.Speed = Speed;
                if (!isTest)
                    ScoreSystem.HighScore = GameHandler.LoadHighScore(NowPlaying.Title);

                // preparing latency
                Pinger.BeatValue = NowPlaying.MapData[GameDifficulty].Snap;
                Syncer.BPM = NowPlaying.BPM;
                lateStart = NowPlaying.Offset > 0;
                FallTime = (Spawner.Position - Spawner.TriggerPoint) / Speed;
                Spawner.FallTime = FallTime;
                Syncer.TimeOffset = FallTime;

                if (lateStart)
                    Pinger.OffsetTime = NowPlaying.Offset;
                else
                    Syncer.TimeOffset += -NowPlaying.Offset;
            }
            #endregion
        }

        #region BasicBehaviours
        ///<summary>
        ///Use this for initialization
        ///</summary>
        void Start()
        {
            IsPlaying = true;
        }

        ///<summary>
        ///Update is called once per frame
        ///</summary> 
        void Update()
        {
            if (!gameOver)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Pause(IsPlaying);
                }
            }
        }

        #region EventHandling
        private void OnEnable()
        {
            Pause += Pause_Menu;
            Spawner.GameOver += ShowGameOverScreen;
        }
        private void OnDisable()
        {
            Pause -= Pause_Menu;
            Spawner.GameOver += ShowGameOverScreen;
        }

        /// <summary>
        /// Shows the pause menu
        /// </summary>
        /// <param name="isPaused">Is the game paused now?</param>
        private void Pause_Menu(bool isPaused)
        {
            GameLog.Debug("Game paused = " + isPaused.ToString());
            if (isPaused)
            {
                PausePanel.SetActive(true);
                IsPlaying = false;
                Music.Pause();
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                PausePanel.SetActive(false);
                Music.UnPause();
                IsPlaying = true;
            }
        }

        /// <summary>
        /// Show the game over screen
        /// </summary>
        /// <returns></returns>
        void ShowGameOverScreen()
        {
            //yield return new WaitForSeconds(2 * FallTime);
            if (!isTest)
                GameHandler.SaveHighScore(NowPlaying.Title, ScoreSystem.Score);
            GameOverScreen.GetComponent<UIController>().Show();
            FinalPanel.SetTexts();
            
            
        }

        #endregion
        #endregion

        public void PlayFX(string fxName)
        {
            //FXPlayer.PlayOneShot(Resources.Load<AudioClip>(gameData.RawPath + "FX_" + fxName));
        }

        /// <summary>
        /// Go back to main menu
        /// </summary>
        public void ReturnToMenu()
        {
            if (!isTest)
            {
                GameHandler.UnloadAudio(Music.clip);
                GUI.Initiate.Fade("Menu", Color.white, 0.7f);
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

        }

    }
}


