//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using MathBeat.Core;
//using System.Collections.Generic;
//using Newtonsoft.Json;

//namespace MathBeat.Editor
//{
//    public class QuizEditor : EditorWindow
//    {
//        public List<Quiz> QuizData;
//        string filePath;


//        string quizPath = "/Resources/Values/Quiz.json";

//        [MenuItem("Tools/MathBeat/Quiz Editor")]
//        static void Init()
//        {
//            //EditorUtility.DisplayDialog("Quiz Editor", "Welcome to Quiz Editor!", "OK");
//            QuizEditor window = GetWindow<QuizEditor>("Quiz Editor");
//        }

//        private void Awake()
//        {
//            QuizData = new List<Quiz>();
//            filePath = Application.dataPath + quizPath;
//        }

//        void LoadQuizData()
//        {

//            if (File.Exists(filePath))
//            {
//                string json = File.ReadAllText(filePath);
//                QuizData = JsonConvert.DeserializeObject<List<Quiz>>(json);
//            }
//            else
//            {
//                QuizData = new List<Quiz>();
//            }
//        }

//        void SaveQuizData()
//        {
//            string json = JsonConvert.SerializeObject(QuizData);
//            File.WriteAllText(filePath, json);
//        }

//        private void OnGUI()
//        {
//            if (QuizData != null)
//            {
//                // create a serialized object and properties to allow edit
//                SerializedObject objData = new SerializedObject(this);
//                SerializedProperty objProp = objData.FindProperty("QuizData");

//                // Now let the GUI run
//                EditorGUILayout.PropertyField(objProp, true, GUILayout.ExpandHeight(true));
//                objData.ApplyModifiedProperties();

//                if (GUILayout.Button("Save")) SaveQuizData();
                
//            }

//            if (GUILayout.Button("Load")) LoadQuizData();
//        }
//    } 
//}