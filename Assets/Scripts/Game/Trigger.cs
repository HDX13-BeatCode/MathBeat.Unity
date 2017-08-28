using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathBeat.Core;

namespace MathBeat.Game
{
    public class Trigger : MonoBehaviour
    {
        /// <summary>
        /// Score system
        /// </summary>
        public Scoring ScoreSystem;

        /// <summary>
        /// Keyboard key mapping for PC/Web
        /// </summary>
        public KeyCode KeyPressed;

        //public Main MainScreen;
        public ObjectPool Recycler;

        /// <summary>
        /// Answer ID code (0/1/2/3)
        /// </summary>
        public int AnswerID;

        /// <summary>
        /// Checks if the trigger is activated,
        /// either with a key or a touch.
        /// </summary>
        private bool isTriggered = false;

        /// <summary>
        /// The beat in contact with the trigger
        /// </summary>
        private GameObject currentBeat;

        private Queue<GameObject> beatQueue;

        /// <summary>
        /// The center of the trigger area (define manually)
        /// </summary>
        [SerializeField]
        private float triggerCenter = -6.5f;

        const string TAG = "Question";

        // Use this for initialization
        void Start()
        {
            ScoreSystem = FindObjectOfType<Scoring>();
            Recycler = FindObjectOfType<ObjectPool>();
            triggerCenter = FindObjectOfType<BlockSpawner>().TriggerPoint;
            beatQueue = new Queue<GameObject>();
            currentBeat = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (isTriggered)
            {
                if (currentBeat != null)
                {
                    if (currentBeat.CompareTag("Question"))
                    {
                        CheckAnswer(currentBeat);
                        Recycler.ReturnObject(currentBeat);
                        //currentBeat = null;
                    }    
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D obj)
        {
            Log.Debug("Hit now!");
            if (obj.gameObject.CompareTag("Question"))
            {
                beatQueue.Enqueue(obj.gameObject);
                currentBeat = beatQueue.Peek();
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Debug.Break();
            Log.Debug("Block is out of trigger!");
            
            if (beatQueue.Count > 0)
            {
                beatQueue.Dequeue();
                if (beatQueue.Count > 0)
                    currentBeat = beatQueue.Peek();
            }
            else
                currentBeat = null;
            isTriggered = false;
        }

        private void CheckAnswer(GameObject block)
        {
            // make sure it's off
            isTriggered = false;
            Log.Debug("The selected answer ID is " + AnswerID.ToString());
            // check for answer
            if (IsCorrect(block.GetComponent<NoteBlock>(), AnswerID))
            {
                SendHit();
            }
            else
            {
                ScoreSystem.Respond(Scoring.CODE_WRONG);
            }  
        }

        public bool IsCorrect(NoteBlock answer, int ansId)
        {
            return answer.CurrentQuestion.IsCorrect(answer[ansId]);
        }

        public void OnTrigger(int id)
        {
            AnswerID = id;
            if(currentBeat != null)
                isTriggered = true;
        }

        private void SendHit()
        {
            float delta = Mathf.Abs(triggerCenter - currentBeat.transform.position.y);
            ScoreSystem.Respond(Scoring.CODE_HIT, delta, TAG);
        }
    }

}