using UnityEngine;
using System.Collections;

namespace MathBeat.Game
{
    public class Destroyer : MonoBehaviour
    {
        public Main MainGame;
        [SerializeField]
        bool destroy = false;
        public GameObject noteBlock;
        // Use this for initialization
        void Start()
        {
            MainGame = FindObjectOfType<Main>();
        }

        // Update is called once per frame
        void Update()
        {
            if (destroy)
            {
#if UNITY_EDITOR
                //Debug.Break();
#endif
                MainGame.DisposeBlock(noteBlock);
                MainGame.ScoreSystem.Respond(Scoring.CODE_MISS);
                destroy = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
#if UNITY_EDITOR
            //Debug.Break();
#endif
            destroy = true;
            if (collision.gameObject.CompareTag("Beat"))
                noteBlock = collision.gameObject;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            destroy = false;
        }
    } 
}
