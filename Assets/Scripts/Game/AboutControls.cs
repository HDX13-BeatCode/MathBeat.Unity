using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MathBeat.Game
{
    public class AboutControls : MonoBehaviour
    {
        [Header("Controls")]
        public Camera MainCamera;
        public Button Next, Previous;
        public Animator Anime;

        [Header("Screens")]
        public int ScreenIndex = 0;
        public int MaxScreens = 3;

        delegate void OnScreenChanged();
        event OnScreenChanged ScreenChanged;


        // Use this for initialization
        void Start()
        {
            MainCamera = FindObjectOfType<Camera>();
            ScreenChanged();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                Return();
        }

        public void Return()
        {
            GUI.Initiate.Fade("Menu", Color.black, 2);
        }

        private void OnEnable()
        {
            ScreenChanged += Changed;
        }

        private void OnDisable()
        {
            ScreenChanged -= Changed;
        }

        private void Changed()
        {
            Anime.SetInteger("CameraPos", ScreenIndex);

            Next.enabled = !IsMax;
            Previous.enabled = !IsMin;
        }

        public void NextScreen()
        {
            if (!IsMax)
            {
                ScreenIndex++;
                ScreenChanged();
            }

        }

        public void PreviousScreen()
        {
            if (!IsMin)
            {
                ScreenIndex--;
                ScreenChanged();
            }
        }

        private bool IsMax { get { return ScreenIndex >= MaxScreens - 1; } }
        private bool IsMin { get { return ScreenIndex <= 0; } }

        public void LoadURL(string path)
        {
            Application.OpenURL(path);
        }
    } 
}
