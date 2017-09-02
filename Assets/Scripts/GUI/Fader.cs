using UnityEngine;
using UGUI = UnityEngine.GUI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MathBeat.GUI
{
    public class Fader : MonoBehaviour
    {
        [HideInInspector]
        public bool start = false;
        [HideInInspector]
        public float fadeDamp = 0.0f;
        [HideInInspector]
        public string fadeScene = "";
        [HideInInspector]
        public int fadeSceneIdx = -1;
        [HideInInspector]
        public float alpha = 0.0f;
        [HideInInspector]
        public Color fadeColor;
        [HideInInspector]
        public bool isFadeIn = false;
        [HideInInspector]
        public bool showLoadingScreen = false;

        //Set callback
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        //Remove callback
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        //Create a texture , Color it, Paint It , then Fade Away
        void OnGUI()
        {
            //Fallback check
            if (!start)
                return;
            
            //Assign the color with variable alpha
            UGUI.color = new Color(UGUI.color.r, UGUI.color.g, UGUI.color.b, alpha);
            
            //Temp Texture
            Texture2D myTex;
            myTex = new Texture2D(1, 1);
            myTex.SetPixel(0, 0, fadeColor);
            myTex.Apply();
            
            //Print Texture
            UGUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), myTex);
            
            //Fade in and out control
            if (isFadeIn)
                alpha = Mathf.Lerp(alpha, -0.1f, fadeDamp * Time.deltaTime);
            else
                alpha = Mathf.Lerp(alpha, 1.1f, fadeDamp * Time.deltaTime);
            
            //Load scene
            if (alpha >= 1 && !isFadeIn)
            {
                if (showLoadingScreen)
                {
                    if (fadeScene != string.Empty)
                        LoadingScreenManager.LoadScene(fadeScene);
                    else if (fadeSceneIdx >= 0)
                        LoadingScreenManager.LoadScene(fadeSceneIdx);
                }
                else
                {
                    if (fadeScene != string.Empty)
                        SceneManager.LoadScene(fadeScene);
                    else if (fadeSceneIdx >= 0)
                        SceneManager.LoadScene(fadeSceneIdx);
                    DontDestroyOnLoad(gameObject);
                }
            }
            else

            if (alpha <= 0 && isFadeIn)
            {
                Destroy(gameObject);
            }

        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            //We can now fade in
            isFadeIn = true;
        }

    } 
}
