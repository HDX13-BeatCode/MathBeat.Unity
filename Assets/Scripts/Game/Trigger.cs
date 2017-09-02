using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathBeat.Core;

namespace MathBeat.Game
{
    public class Trigger : MonoBehaviour
    {
        //Activator1.Color = f44336
        //Activator2.Color = FFEB3B
        //Activator3.Color = 4CAF50
        //Activator4.Color = 2196F3

        public Scoring ScoreSystem;

        /// <summary>
        /// Keyboard key mapping for PC/Web
        /// </summary>
        public KeyCode keyPressed;

        public Main MainScreen;

        /// <summary>
        /// Answer ID code (0/1/2/3)
        /// </summary>
        public int answerID;

        /// <summary>
        /// Checks if the trigger is activated,
        /// either with a key or a touch.
        /// </summary>
        private bool isTriggered = false;

        /// <summary>
        /// The beat in contact with the trigger
        /// </summary>
        private GameObject currentBeat;

        /// <summary>
        /// The answer generated from the Core
        /// </summary>
        private string answer;

        /// <summary>
        /// The center of the trigger area (define manually)
        /// </summary>
        private float triggerCenter;

        // Use this for initialization
        void Start()
        {
            ScoreSystem = FindObjectOfType<Scoring>();
            MainScreen = FindObjectOfType<Main>();
            triggerCenter = -6.5f;
        }

        // Update is called once per frame
        void Update()
        {  
            if (isTriggered)
            {
                if (currentBeat != null)
                {
                    CheckAnswer();
                }          
            }                  
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log("Hit now!");
#endif
            if (collision.gameObject.tag == "Beat")
            {
                currentBeat = collision.gameObject;
            }
                
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Debug.Break();
            Debug.Log("Block is out of trigger!");
            currentBeat = null;
        }

        private void CheckAnswer()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //Debug.Break();
            Debug.Log("The selected answer ID is "+ answerID.ToString());
#endif
            if (IsCorrect(currentBeat.GetComponent<NoteBlock>(), answerID))
            {
                float delta = Mathf.Abs(triggerCenter - currentBeat.transform.position.y);
                ScoreSystem.Respond(Scoring.CODE_HIT, delta);
            }
            else
            {
                ScoreSystem.Respond(Scoring.CODE_WRONG);
            }
            MainScreen.DisposeBlock(currentBeat);
            isTriggered = false;
            currentBeat = null;
        }

        public bool IsCorrect(NoteBlock answer, int ansId)
        {
            return answer.CurrentQuestion.IsCorrect(answer[ansId]);
        }

        public void OnTrigger(int id)
        { 
            answerID = id;
            isTriggered = true; 
        }
    }

}