using UnityEngine;
using System.Collections.Generic;

namespace MathBeat.Game
{
    public class NoteTrigger : MonoBehaviour
    {
        GameObject currentBeat;
        //Queue<GameObject> beatQueue;
        [SerializeField]
        float triggerCenter = 0f;
        Scoring ScoreSystem;
        const string TAG = "Note";
        bool isTriggered = false;
        [SerializeField]
        ObjectPool Recycler;

        // Use this for initialization
        void Start()
        {
            ScoreSystem = FindObjectOfType<Scoring>();
            //beatQueue = new Queue<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isTriggered)
            {
                if (currentBeat != null)
                {
                    SendHit();
                    Recycler.ReturnObject(currentBeat);
                    currentBeat = null;
                }
                isTriggered = false;
            }
                 
        }

        private void OnTriggerEnter2D(Collider2D obj)
        {
            if (obj.gameObject.CompareTag(TAG))
            {
                //beatQueue.Enqueue(obj.gameObject);
                currentBeat = obj.gameObject;
            }     
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //if (beatQueue.Count > 0)
            //{
            //    beatQueue.Dequeue();
            //    if (beatQueue.Count > 0)
            //        currentBeat = beatQueue.Peek();
            //}
            //else
                currentBeat = null;
            isTriggered = false;
        }

        private void SendHit()
        {
            float delta = Mathf.Abs(triggerCenter - currentBeat.transform.position.y);
            ScoreSystem.Respond(Scoring.CODE_HIT, delta, TAG);
            isTriggered = false;
        }

        public void OnTrigger()
        {
            if (currentBeat != null)
                isTriggered = true;
        }
    }
}