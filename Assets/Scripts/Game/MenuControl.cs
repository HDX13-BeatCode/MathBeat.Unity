using UnityEngine;
using UnityEngine.UI;
using UButton = UnityEngine.UI.Button;
using MathBeat.GUI;
using MathBeat.Core;
using System.Collections.Generic;


namespace MathBeat.Game
{
    public class MenuControl : MonoBehaviour
    {
        public UIController MainMenu, SelectMusic, MusicDifficulty;
        public GameHandler GameData; 
        public AudioSource BGAudio;
        public Slider SliderDifficulty, SliderSpeed;
        public Text TitleArtist, QuestionDifficulty, GameSpeed;
        public ObjectPool ButtonRecycler;

        private List<GameObject> buttons;

        private void Awake()
        {
            SelectMusic.gameObject.SetActive(false);
            MusicDifficulty.gameObject.SetActive(false);
        }

        private void Start()
        {
            GameData = GameHandler.Game;
            buttons = new List<GameObject>();
            SliderDifficulty.value = PlayerPrefs.GetInt("QuizDifficulty");
            SliderSpeed.value = PlayerPrefs.GetInt("GameSpeed");
            UpdateText();
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

        public void SetDifficulty(string title)
        {
            GameData.SelectedMusic = GameData.MusicList[title];
            MusicDifficulty.Show();
            ShowDifficultyButtons();
            TitleArtist.text = GameData.SelectedMusic.ToString();
            if (BGAudio.isPlaying)
                BGAudio.Stop();
            BGAudio.PlayOneShot(GameHandler.LoadAudio(GameData.SelectedMusic.FileName));
        }

        public void StartGame()
        {
            GameData.QuestionDifficulty = (int)SliderDifficulty.value;
            GameData.GameSpeed = (int)SliderSpeed.value;
            Initiate.Fade("Main", Color.black, 1f, true);
        }


        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QuizDifficulty", Mathf.RoundToInt(SliderDifficulty.value));
            PlayerPrefs.SetInt("GameSpeed", Mathf.RoundToInt(SliderSpeed.value));
        }

        public void UpdateText()
        {
            QuestionDifficulty.text = "Kesulitan Soal: ";
            switch ((int)SliderDifficulty.value)
            {
                case 1:
                    QuestionDifficulty.text += "Mudah";
                    break;
                case 2:
                    QuestionDifficulty.text += "Sedang";
                    break;
                case 3:
                    QuestionDifficulty.text += "Sulit";
                    break;
                default:
                    QuestionDifficulty.text += "";
                    break;
            }

            GameSpeed.text = (SliderSpeed.value / 4).ToString() + "x";
        }

        private void ShowDifficultyButtons()
        {
            RemoveDifficultyButtons();

            foreach (var mapData in GameData.SelectedMusic.MapData)
            {
                GameObject obj = ButtonRecycler.GetObject();
                obj.GetComponentInChildren<Text>().text = mapData.Difficulty.ToString();
                obj.GetComponent<DifficultyButton>().DifficultyIndex = buttons.Count;
                buttons.Add(obj);
            }
            
        }

        private void RemoveDifficultyButtons()
        {
            while(buttons.Count > 0)
            {
                ButtonRecycler.ReturnObject(buttons[0]);
                buttons.RemoveAt(0);
            }
        }
    } 
}
