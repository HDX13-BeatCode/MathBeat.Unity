using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathBeat.Core;
using UnityEngine.UI;

namespace MathBeat.Game
{
    public class NoteBlock : MonoBehaviour
    {
        Rigidbody2D rgbd;
        Main MainGame;
        public Text QuestionText;
        public Text[] AnswerTexts;

        public float Speed;
        public int Difficulty;

        /// <summary>
        /// The question attached to the beat
        /// </summary>
        public Quiz CurrentQuestion;

        private void Awake()
        {
            MainGame = FindObjectOfType<Main>();
        }

        /// <summary>
        /// Use this for initialization
        /// </summary> 
        void Start()
        {
            //preparing Rigidbody2D to simulate fall
            rgbd = GetComponent<Rigidbody2D>();
            Speed = MainGame.Speed;
            Difficulty = MainGame.Difficulty;
            LoadQuestion(Difficulty);
        }


        public void LoadQuestion(int difficulty)
        {
            // setting the questions and answers
            // load a new question every time
            CurrentQuestion = MainGame.QuizData.PickQuiz(difficulty);

            // setting it to the texts
            QuestionText.text = CurrentQuestion.Question;
            List<string> answersList = CurrentQuestion.Answers.GetAnswers(4);
            for (int i = 0; i < answersList.Count; i++)
            {
                AnswerTexts[i].text = answersList[i];
            }
            rgbd.velocity = new Vector2(0, -Speed);
        }

        /////<summary>
        ///// Update is called once per frame
        /////</summary> 
        //void Update()
        //{
        //    if (MainGame.IsPlaying)
        //        rgbd.velocity = new Vector2(0, 0);
        //}

        public string this[int answerId]
        {
            get
            {
                return AnswerTexts[answerId].text;
            }
        }

    } 
}
