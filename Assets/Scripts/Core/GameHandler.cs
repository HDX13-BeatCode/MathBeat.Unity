﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MathBeat.Core
{
    public class GameHandler : MonoBehaviour
    {
        public static GameHandler Game;

        /// <summary>
        /// Quiz database for questions and answers
        /// </summary>
        public QuizSystem QuizSystem;

        /// <summary>
        /// List of music in the game
        /// </summary>
        public MusicList MusicList;

        public Log GameLogger;

        public int GameDifficulty;
        public int QuestionDifficulty;
        public int GameSpeed;

        #region CONSTS
        /// <summary>
        /// /Resources/
        /// </summary>
        [HideInInspector]
        public const string RES_PATH = "Resources/";

        /// <summary>
        /// /Resources/Raw/ (for audio files)
        /// </summary>
        [HideInInspector]
        public const string RAW_PATH = "Raw/";

        /// <summary>
        /// /Resources/Values/
        /// </summary>
        [HideInInspector]
        const string JSON_PATH = "Values/";

        /// <summary>
        /// Quiz.json
        /// </summary>
        [HideInInspector]
        const string JSON_QUIZ = "Quiz";

        /// <summary>
        /// MusicData.json
        /// </summary>
        [HideInInspector]
        const string JSON_MUSIC = "MusicData";

        private const string highScorePrefsFormat = "HighScore[{0}]";

        #endregion
        /// <summary>
        /// The index of currently selected music
        /// </summary>
        public MusicData SelectedMusic;

        public GameHandler()
        {
            QuizSystem = new QuizSystem();
            MusicList = new MusicList();
        }

        private void Awake()
        {
            // path doesn't need "Resources/"
            //because it's loaded with Resources.Load,
            //so it's automatically runs it from the Resources folder.

            Game = this;
            Log.Debug(ToString() + " has awaken at " + DateTime.Now);
        }

        // Use this for initialization
        void Start()
        {
            GameLogger = gameObject.AddComponent<Log>();
            DontDestroyOnLoad(gameObject);
            //Load the splash screen
            SceneManager.LoadScene("Splash");
            QuizSystem.Quiz = LoadJSON<List<Quiz>>(JSON_QUIZ);
            LoadAllMusicData();
        }

        public MusicData GetCurrentMusic()
        {
            return SelectedMusic;
        }

        /// <summary>
        /// Returns <see cref="List{T}"/> from JSON file
        /// </summary>
        /// <typeparam name="T">Type to be returned</typeparam>
        /// <param name="jsonFileName">JSON file name</param>
        /// <returns>T object</returns>
        public static T LoadJSON<T>(string jsonFileName)
        {
            Log.Debug("Loading {0}...", jsonFileName);
            TextAsset json = Resources.Load<TextAsset>(JSON_PATH + jsonFileName);
            Log.Debug("Parsing {0}...", jsonFileName);
            return JsonConvert.DeserializeObject<T>(json.text);
        }

        private void LoadAllMusicData()
        {
            TextAsset[] JSONs = Resources.LoadAll<TextAsset>(JSON_PATH);
            var musicJson = JSONs.Where(text => text.text.StartsWith("/* MUSIC DATA */"));
            foreach (var json in musicJson)
            {
                MusicList.MusicData.Add(
                    JsonConvert.DeserializeObject<MusicData>(json.text)
                );
            }
        }

        /// <summary>
        /// Unload the audio from memory
        /// </summary>
        public static void UnloadAudio(AudioClip audio)
        {
            if(audio.loadState == AudioDataLoadState.Loaded)
                audio.UnloadAudioData();
        }

        /// <summary>
        /// Load the audio to memory
        /// </summary>
        public static AudioClip LoadAudio(string audioName)
        {
            AudioClip audio;
            Log.Debug("Loading audio at " + DateTime.Now);
            // load the audio file
            // note: ignore extension
            audio = Resources.Load<AudioClip>(RAW_PATH + audioName);
            Log.Debug("{0} {1}...", audio.name, audio.loadState);

            Log.Debug("Loading {0}...", audio);
            if (audio.LoadAudioData() /* is a success */)
                return audio;
            else
            {
                Log.Error("Unable to load {0}!", audio.name);
                return null;
            }     
        }

        //Update is called once per frame
        //void Update()
        //{

        //}

        public static int LoadHighScore(string title)
        {
            string key = string.Format(highScorePrefsFormat, title);
            return PlayerPrefs.GetInt(key);
        }

        public static void SaveHighScore(string title, int score)
        {
            string key = string.Format(highScorePrefsFormat, title);
            if (score > LoadHighScore(title))
            {
                PlayerPrefs.SetInt(key, score);
            }
        }

    }
}
