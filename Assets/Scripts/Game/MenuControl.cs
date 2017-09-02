using UnityEngine;
using UnityEngine.UI;
using MathBeat.GUI;
using System.Collections;


namespace MathBeat.Game
{
    public class MenuControl : MonoBehaviour
    {
        public UIController MainMenu, SelectMusic, MusicDifficulty;
        public Core.GameHandler GameData;
        public Slider SliderDifficulty, SliderSpeed;

        private void Start()
        {
            GameData = FindObjectOfType<Core.GameHandler>();
            SliderDifficulty.value = PlayerPrefs.GetInt("QuizDifficulty");
            SliderSpeed.value = PlayerPrefs.GetInt("GameSpeed");
        }

        public void LoadScene(string sceneName)
        {
            Initiate.Fade(sceneName, Color.black, 1.5f);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SetDifficulty(int musicId)
        {
            GameData.SelectedMusic = musicId;
            MusicDifficulty.Show();
        }

        public void StartGame()
        {
            GameData.Difficulty = (int)SliderDifficulty.value;
            GameData.GameSpeed = (int)SliderSpeed.value;
            Initiate.Fade("Main", Color.black, 1f, true);
        }


        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QuizDifficulty", Mathf.RoundToInt(SliderDifficulty.value));
            PlayerPrefs.SetInt("GameSpeed", Mathf.RoundToInt(SliderSpeed.value));
        }

    } 
}
