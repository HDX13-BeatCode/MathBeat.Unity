using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MathBeat.GUI
{
    public class Button : MonoBehaviour, IPointerDownHandler
    {
        public int AnswerID;
        Game.Trigger trigbox;
        [SerializeField]
        Game.NoteTrigger trigcircle;
        Game.Main main;

        //public Image Image;
        //public float FadeDuration = .1f;
        //public Color NormalColor;
        //public Color PressedColor;

        /// <summary>
        /// Getting things ready...
        /// </summary>
        private void Awake()
        {
            trigbox = FindObjectOfType<Game.Trigger>();
            main = FindObjectOfType<Game.Main>();
        }

        private void OnEnable()
        {
            main.Pause += Pause_GameButton;
            main.Spawner.GameOver += Spawner_GameOver;
        }

        private void Spawner_GameOver()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            main.Pause -= Pause_GameButton;
            main.Spawner.GameOver -= Spawner_GameOver;
        }

        private void Pause_GameButton(bool isPaused)
        {
            enabled = !isPaused;
        }

        public void OnPointerDown(PointerEventData args)
        {
            Debug.LogFormat("Button #{0} was pressed!", AnswerID);
            trigbox.OnTrigger(AnswerID);
            trigcircle.OnTrigger();
        }

    }
}
