using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using MathBeat.Core;
using GameLog = MathBeat.Core.Log;
using System;

namespace MathBeat.Game
{
    public class Main : MonoBehaviour
    {
        #region Fields
        #region DEBUG
        [Header("DEBUG")]
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        private float lastNoteTime = 0;
        public DebugPanel DebugPanel;

#endif
        private bool isTest = true;
        [SerializeField]
        private float position = 0f;

        [SerializeField]
        private int[] TestTrackBeats = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        [SerializeField]
        private float testOffset = 1;
        
        const int ms = 1000;
        #endregion DEBUG

        #region GameplayData
        [Header("Gameplay Data")]
        /// <summary>
        /// The position of the current active note 
        /// </summary>
        public int CurrentBeat = 0;

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
        public MusicData MusicData;

        public QuizSystem QuizData;

        public int Difficulty = 1;

        public int Speed = 4;

        public float Latency = 0f;

        /// <summary>
        /// Time needed for a block to fall
        /// </summary>
        private float FallTime;

        /// <summary>
        /// The music player
        /// </summary>
        public AudioSource Music;

        /// <summary>
        /// The music clip to be played
        /// </summary>
        private AudioClip music2Play;

        public AudioSource FXPlayer;

        public Text FinalScoreText;

        #endregion GameplayData

        #region Others
        /// <summary>
        /// Contains the data for the game
        /// </summary>
        [Header("Others")]
        public GameHandler GameData;

        /// <summary>
        /// A recycler for note blocks
        /// </summary>
        public ObjectPool cache;

        public static Animator anim;

        public GameObject GameOverScreen;

        public GameObject PausePanel;

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

        /// <summary>
        /// To check if the game is running
        /// </summary>
        public bool IsRunning;

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
                Core.Log.Debug("Preparing data at " + DateTime.Now);
                GameData = FindObjectOfType<GameHandler>(); // if this fails, go to catch line
                music2Play = FindObjectOfType<AudioClip>();
                
                // preparing data
                MusicData = GameData.GetCurrentMusic();
                QuizData = GameData.QuizSystem;
                FallTime = 16f / GameData.GameSpeed;
                Difficulty = GameData.Difficulty;
                Speed = GameData.GameSpeed;
                isTest = false;

                LoadAudio();
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
                MusicData = new MusicData() { Title = "TestTrack", Artist = "<unknown/>", // dat html/xml!
                    BPM = 120, Offset = testOffset, TrackBeats = TestTrackBeats };
                QuizData.Quiz = GameHandler.LoadJSON<Quiz>("Quiz"); //epic one-liner
                FallTime = 16f / Speed;
            }
            #endregion
            #region finally
            finally
            {
                //these lines will be executed no matter what
                ScoreSystem = FindObjectOfType<Scoring>();
                cache = FindObjectOfType<ObjectPool>();
                anim = GetComponent<Animator>();

                // preparing latency
                lateStart = MusicData.Offset > FallTime;
            }
            #endregion
        }

        #region BasicBehaviours
        ///<summary>
        ///Use this for initialization
        ///</summary>
        void Start()
        {
            // playing music
            Play();
        }

        #region EventHandling
        private void OnEnable()
        {
            Pause += Pause_Menu;
        }
        private void OnDisable()
        {
            Pause -= Pause_Menu;
        }
        private void Pause_Menu(bool isPaused)
        {
            GameLog.Debug("Game paused = " + isPaused.ToString());
            if (isPaused)
            {
                IsPlaying = false;
                IsRunning = false;
                Music.Pause();
                PausePanel.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                PausePanel.SetActive(false);
                Music.UnPause();
                IsPlaying = true;
                IsRunning = true;
            }
        }
        #endregion

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
                
                if (IsPlaying)
                {
                    position += Time.deltaTime;

                    if (position >= Music.clip.length || CurrentBeat >= MusicData.TrackBeats.Length)
                    {
                        //end the game
                        Debug.Log("The game has ended.");
                        gameOver = true;
                        IsPlaying = false;
                        IsRunning = false;
                        StopCoroutine(MakeBeat());
                        StartCoroutine(ShowGameOverScreen());
                    }
                }  
            } 
        }

        /////<summary>
        /////Update is called in a fixed interval
        /////</summary> 
        //private void FixedUpdate()
        //{
        //    if (!gameOver)
        //    {
                
        //    }
        //}
        #endregion

        /// <summary>
        /// Plays the music in delay
        /// </summary>
        /// <returns>WaitForSeconds</returns>
        void Play()
        {
            StartCoroutine(MakeBeat());
            float delay = lateStart ? 0 : (FallTime - MusicData.Offset);
            Music.PlayDelayed(delay);
            IsPlaying = true;
        }

        public void PlayFX(string fxName)
        {
            //FXPlayer.PlayOneShot(Resources.Load<AudioClip>(gameData.RawPath + "FX_" + fxName));
        }

        /// <summary>
        /// An async task to spawn notes
        /// </summary>
        /// <returns></returns>
        IEnumerator MakeBeat()
        {
            #region Preparations
            //set the beat speed
            BPM = MusicData.GameBpm;
            float speed = 240f / BPM; // (60 sec / BPM * 4 beats)
            float latency = Latency / ms;
            float delay = speed - latency;

            // check if the game is running
            if (!IsRunning)
            {
                if (lateStart)
                    yield return new WaitForSeconds(MusicData.Offset - FallTime);
                else
                    yield return null;

                IsRunning = true;
            }
            #endregion

            #region Spawn
            //var spawnPoint = new Vector2(0, 8f);
            while (CurrentBeat < MusicData.TrackBeats.Length)
            {
                bool makeNote;
                try
                {
                    if (MusicData.TrackBeats[CurrentBeat++] == 0)
                        makeNote = false;
                    else
                        makeNote = true;

                    if (makeNote)
                    {
                        var noteObj = cache.GetObject();
                        noteObj.GetComponent<NoteBlock>().LoadQuestion(Difficulty);
                        GameLog.Debug("Note #{0} generated at {1} s.\nDelay: {2}",
                            CurrentBeat, position, position - lastNoteTime);
                        lastNoteTime = position;
                    }
                }
                catch (Exception)
                {
                    //Debug.LogWarning("No more beats detected.");
                    //// end the game
                }
                // wait for a beat before starting over
                yield return new WaitForSeconds(delay);
            }
            #endregion
        }

        /// <summary>
        /// Show the game over screen
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowGameOverScreen()
        {
            yield return new WaitForSeconds(2 * FallTime);
            if(!isTest)
                ScoreSystem.SaveHighScore(GameData.SelectedMusic);
            GameOverScreen.GetComponent<UIController>().Show();
            FinalScoreText.text = "Skor akhir: " + ScoreSystem.Score.ToString();
        }

        /// <summary>
        /// Go back to main menu
        /// </summary>
        public void ReturnToMenu()
        {
            if (!isTest)
            {
                UnloadAudio();
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

        /// <summary>
        /// Send a <see cref="GameObject"/> back to <see cref="ObjectPool"/>  
        /// </summary>
        /// <param name="obj"></param>
        public void DisposeBlock(GameObject obj)
        {
            cache.ReturnObject(obj);
        }

        /// <summary>
        /// Unload the audio from memory
        /// </summary>
        void UnloadAudio()
        {
            music2Play.UnloadAudioData();
        }

        /// <summary>
        /// Load the audio to memory
        /// </summary>
        void LoadAudio()
        {
            GameLog.Debug("Loading audio at " + DateTime.Now);
            // load the audio file
            // note: ignore extension
            music2Play = Resources.Load<AudioClip>(GameHandler.RAW_PATH + MusicData.FileNameNoExt);
            switch (music2Play.loadState)
            {
                case AudioDataLoadState.Unloaded:
                    GameLog.Debug(music2Play.ToString() + "is not loaded.");
                    break;
                case AudioDataLoadState.Loading:
                    GameLog.Debug(music2Play.ToString() + "is loading.");
                    break;
                case AudioDataLoadState.Loaded:
                    GameLog.Debug(music2Play.ToString() + "is loaded.");
                    break;
                case AudioDataLoadState.Failed:
                    Debug.LogError(music2Play.ToString() + "failed to load.");
                    break;
                default:
                    break;
            }
            GameLog.Debug("Loading {0}...", music2Play);
            if (music2Play.LoadAudioData())
                Music.clip = music2Play;
            else
                GameLog.Error("Unable to load {0}!", music2Play);
        }

        //void PauseGame(bool pause)
        //{
        //    GameLog.DebugLog("Game paused = " + pause.ToString());
        //    switch (pause)
        //    {
        //        case true:
        //            Music.Pause();
        //            PausePanel.Show();
        //            break;
        //        case false:
        //            Time.timeScale = 1;
        //            PausePanel.Hide();
        //            break;
        //        default:
        //            break;
        //    }
        //}
        //
        //public void OnGamePaused()
        //{
        //    Time.timeScale = 0;
        //}
        //public void OnGameUnpaused()
        //{
        //    Time.timeScale = 1;
        //}
    }
}