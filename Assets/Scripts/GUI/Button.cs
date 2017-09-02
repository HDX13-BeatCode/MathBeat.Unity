using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MathBeat.GUI
{ 
    public class Button: MonoBehaviour, IPointerDownHandler
    {
        public int AnswerID;
        Game.Trigger TriggerBox;

        //public Image Image;
        //public float FadeDuration = .1f;
        //public Color NormalColor;
        //public Color PressedColor;

        /// <summary>
        /// Getting things ready...
        /// </summary>
        private void Awake()
        {
            TriggerBox = FindObjectOfType<Game.Trigger>();
        }

        private void OnEnable()
        {
            TriggerBox.MainScreen.Pause += Pause_GameButton;
        }

        private void OnDisable()
        {
            TriggerBox.MainScreen.Pause -= Pause_GameButton;
        }

        private void Pause_GameButton(bool isPaused)
        {
            gameObject.SetActive(!isPaused);
        }

        public void OnPointerDown(PointerEventData args)
        {
            Debug.LogFormat("Button #{0} was pressed!", AnswerID);
            TriggerBox.OnTrigger(AnswerID);
        }

    }
}
