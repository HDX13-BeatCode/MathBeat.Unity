using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using MathBeat.GUI;

namespace MathBeat.Game
{
    public class Loading : MonoBehaviour
    {
        public float LoadingTime = 3f;
        public Image LoadingBar;

        AudioSource _audio;
        Animator _anim;

        bool isAnimating = false;
        float progress;

        // Use this for initialization
        void Start()
        { 
            //Resetting the loading bar
            LoadingBar.fillAmount = 0f;

            //preparing components
            _audio = GetComponent<AudioSource>();
            _anim = GetComponent<Animator>();

        }

        // Update is called once per frame
        void Update()
        {
            if (!isAnimating)
            {
                _anim.Play("Splash");
                _audio.Play();
                isAnimating = true;
            }
            if(LoadingBar.fillAmount < 1)
            {
                // to make sure it loads after the audio
                if (!_audio.isPlaying)
                    LoadingBar.fillAmount += 1f / LoadingTime * Time.deltaTime;
            }
            else
            {
                Initiate.Fade("Menu", Color.black, 1f);
            }
        }

    }
}