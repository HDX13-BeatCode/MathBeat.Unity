using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MathBeat.Game
{
    public class Destroyer : MonoBehaviour
    {
        public ObjectPool NoteRecycler, QuestionRecycler;
        public Scoring ScoreSystem;
        [SerializeField]
        private GameObject beatBlock;
        private Queue<GameObject> blocks;
        // Use this for initialization
        void Start()
        {
            ScoreSystem = FindObjectOfType<Scoring>();
            blocks = new Queue<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            while(blocks.Count > 0)
            {
                switch (blocks.Peek().tag)
                {
                    case "Note":
                        NoteRecycler.ReturnObject(blocks.Dequeue());
                        break;
                    case "Question":
                        QuestionRecycler.ReturnObject(blocks.Dequeue());
                        break;
                    default:
                        Destroy(blocks.Dequeue());
                        break;
                }
                ScoreSystem.Respond(Scoring.CODE_MISS);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Note") || collision.gameObject.CompareTag("Question"))
                blocks.Enqueue(collision.gameObject);
        }
    }
}
