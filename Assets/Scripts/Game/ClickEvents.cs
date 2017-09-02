using UnityEngine;
using UnityEngine.UI;
using MathBeat.GUI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MathBeat.Game
{
    public class ClickEvents : MonoBehaviour
    {
        public Core.GameHandler GameData;
        public Slider SliderDifficulty, SliderSpeed;
        public int MainSceneID;

        Animator _anim;

        private void Start()
        {
            GameData = FindObjectOfType<Core.GameHandler>();
            _anim = FindObjectOfType<Animator>();
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
            SwitchScreen(4);
        }

        public void StartGame()
        {
            GameData.Difficulty = (int)SliderDifficulty.value;
            GameData.GameSpeed = (int)SliderSpeed.value;
            Initiate.Fade("Main", Color.black, 1f, true);
        }

        public void SwitchScreen(int transIdx)
        {
            switch (transIdx)
            {
                case 1:
                    _anim.SetTrigger("Main2Select");
                    break;
                case 2:
                    _anim.SetTrigger("Main2Help");
                    break;
                case 3:
                    _anim.SetTrigger("Main2Opt");
                    break;
                case 4:
                    _anim.SetTrigger("SetDifficulty");
                    break;
                case -1:
                    _anim.SetTrigger("Select2Main");
                    break;
                case -2:
                    _anim.SetTrigger("Help2Main");
                    break;
                case -3:
                    _anim.SetTrigger("Opt2Main");
                    break;
                case -4:
                    _anim.SetTrigger("SelectBack");
                    break;
                default:
                    break;
            }
        }

        public void SwitchScreen(string animName)
        {
            _anim.SetTrigger(animName);
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QuizDifficulty", Mathf.RoundToInt(SliderDifficulty.value));
            PlayerPrefs.SetInt("GameSpeed", Mathf.RoundToInt(SliderSpeed.value));
        }
    } 
}
