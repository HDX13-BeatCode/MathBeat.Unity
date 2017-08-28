using UnityEngine;
using System.Collections;

namespace MathBeat.Game
{
    public class Destroyer : MonoBehaviour
    {
        public ObjectPool NoteRecycler, QuestionRecycler;
        public Scoring ScoreSystem;
        [SerializeField]
        bool destroy = false;
        private GameObject beatBlock;
        // Use this for initialization
        void Start()
        {
            ScoreSystem = FindObjectOfType<Scoring>();
        }

        // Update is called once per frame
        void Update()
        {
            if (destroy)
            {
#if UNITY_EDITOR
                //Debug.Break();
#endif
                switch (beatBlock.tag)
                {
                    case "Note":
                        NoteRecycler.ReturnObject(beatBlock);
                        break;
                    case "Question":
                        QuestionRecycler.ReturnObject(beatBlock);
                        break;
                    default:
                        Destroy(beatBlock);
                        break;
                }
                ScoreSystem.Respond(Scoring.CODE_MISS);
                destroy = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
#if UNITY_EDITOR
            //Debug.Break();
#endif
            destroy = true;
            if (collision.gameObject.CompareTag("Note") || collision.gameObject.CompareTag("Question"))
                beatBlock = collision.gameObject;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            beatBlock = null;
            destroy = false;
        }
    }
}
