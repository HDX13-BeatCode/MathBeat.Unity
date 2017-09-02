using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathBeat.Core
{
    [Serializable]
    public class QuizSystem
    {
        public List<Quiz> Quiz { get; set; }
        public Quiz PickQuiz(int difficulty)
        {
            Random rand = new Random();
            //var current = new Quiz();

            //// this will make sure that we will get 
            //// the desired level
            //do {
            //    current = Quiz[rand.Next(Quiz.Count)];
            //}
            //while (current.Difficulty > difficulty);

            // I never thought I would use LINQ for this
            var available =
                Quiz.Where(quiz => quiz.Difficulty <= difficulty)
                    .ToList();

            var current = available[rand.Next(available.Count())];
            
            return current;
        }

        public QuizSystem()
        {
            Quiz = new List<Quiz>();
        }
    }

    [Serializable]
    public class Quiz
    {
        //public enum Level { Easy = 1, Normal, Hard}
        public int Difficulty { get; set; }
        public string Question { get; set; }
        public Answers Answers { get; set; }
        public bool IsCorrect(string answer)
        {
            //if the answer is the same with
            //the Answers.Correct (correct answer)
            return answer.Equals(Answers.Correct);
        }
    }

    [Serializable]
    public class Answers
    {
        public string Correct { get; set; }
        public List<string> Others { get; set; }
        public List<string> GetAnswers(int amount = 4)
        {
            if (amount < 3)
            {
                throw new InsufficientAnswersException();
            }
            else
            {
                // make a new randomizer
                Random rand = new Random();
                // make a temporary list to contain the answers
                var listAnswers = new List<string>(amount);
                // of clurse, we need to add the correct answer to the list,
                // otherwise people will be mad because there are no right answers.
                listAnswers.Add(Correct);
                // I could use arrays, though.
                List<string> previous = new List<string>(amount-1);
                // now loop to get the answers list
                // until listAnswers is full
                while (listAnswers.Count < listAnswers.Capacity)
                {
                    //selects a random Others answer
                    string selected = Others[rand.Next(Others.Count)];
                    if (!previous.Contains(selected))
                    {
                        listAnswers.Add(selected);
                        previous.Add(selected);
                    }
                }

                // Now it's time to randomize the <strike>array</strike> list
                // Based on Knuth shuffle algorithm, found on
                // https://forum.unity3d.com/threads/randomize-array-in-c.86871/
                // http://csharphelper.com/blog/2014/07/randomize-arrays-in-c/

                for (int i = 0; i < listAnswers.Count - 1; i++)
                {
                    int j = rand.Next(i, listAnswers.Count);
                    var temp = listAnswers[i];
                    listAnswers[i] = listAnswers[j];
                    listAnswers[j] = temp;
                }

                return listAnswers;
            }
            
        }

        public Answers()
        {
            Others = new List<string>();
        }

        [Serializable]
        public class InsufficientAnswersException : Exception
        {
            public InsufficientAnswersException() : 
                this("Not enough answers to generate. Please try to generate at least 3 answers.", new ArgumentOutOfRangeException()) { }
            public InsufficientAnswersException(string message) : base(message) { }
            public InsufficientAnswersException(string message, Exception inner) : base(message, inner) { }
            protected InsufficientAnswersException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }

}
