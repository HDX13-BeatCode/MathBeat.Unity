using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathBeat.GUI
{
    // LoadingScreenManager
    // --------------------------------
    // built by Martin Nerurkar (http://www.martin.nerurkar.de)
    // for Nowhere Prophet (http://www.noprophet.com)
    // Found at:
    // https://www.youtube.com/watch?v=xJQXoG3caGc
    //
    // Licensed under GNU General Public License v3.0
    // http://www.gnu.org/licenses/gpl-3.0.txt

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UnityEngine.SceneManagement;

    public class LoadingScreenManager : MonoBehaviour
    {

        [Header("Loading Visuals")]
        public Image loadingIcon;
        public Image loadingDoneIcon;
        public Text loadingText;
        public Image progressBar;
        public Image fadeOverlay;

        [Header("Timing Settings")]
        public float waitOnLoadEnd = 0.5f;
        public float fadeDuration = 0.5f;
        //public float minimumLoadTime = 3;

        [Header("Loading Settings")]
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        public ThreadPriority loadThreadPriority;
        public string loadingString = "Loading...";
        public string loadingDoneString = "Done!";
        

        [Header("Other")]
        // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners
        public AudioListener audioListener;

        AsyncOperation operation;
        Scene currentScene;

        public static int sceneIdToLoad = -1;
        public static string sceneName = "";

        // IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
        static int loadingSceneIndex = 5;
        float timer;

        void Start()
        {
            if (sceneIdToLoad < 0 && sceneName == string.Empty)
                return;

#if DEVELOPMENT_BUILD
            Debug.Log("Showing " + ToString() + " at " + System.DateTime.Now);
#endif
            //  timer = minimumLoadTime;
            fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
            currentScene = SceneManager.GetActiveScene();
            if (sceneName != string.Empty)
                StartCoroutine(LoadAsync(sceneName));
            else if (sceneIdToLoad >= 0)
                StartCoroutine(LoadAsync(sceneIdToLoad));

        }

        /****************************************************************************************************/

        /// <summary>
        /// Load a level by scene ID
        /// </summary>
        /// <param name="levelNum"></param>
        public static void LoadScene(int levelNum)
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            sceneIdToLoad = levelNum;
            SceneManager.LoadScene(loadingSceneIndex);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Load a level by scene name
        /// </summary>
        /// <param name="levelName"></param>
        public static void LoadScene(string levelName)
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            sceneName = levelName;
            SceneManager.LoadScene(loadingSceneIndex);
        }

        /****************************************************************************************************/

        /// <summary>
        /// Loading the scene asynchronously
        /// </summary>
        /// <param name="levelNum"></param>
        /// <returns></returns>
        private IEnumerator LoadAsync(int levelNum)
        {
#if DEVELOPMENT_BUILD
            Debug.Log("Loading " + SceneManager.GetSceneByBuildIndex(levelNum).name + " at " + DateTime.Now);
#endif
            ShowLoadingVisuals();

            yield return null;

            FadeIn();
            StartOperation(levelNum);

            float lastProgress = 0f;

            // operation does not auto-activate scene, so it's stuck at 0.9
            while (!DoneLoading())
            {
                yield return null;

#if DEVELOPMENT_BUILD
                Debug.Log(operation.progress + "%...");
#endif

                if (!Mathf.Approximately(operation.progress, lastProgress))
                {
                    progressBar.fillAmount = operation.progress;
                    lastProgress = operation.progress;
                }
            }

            if (loadSceneMode == LoadSceneMode.Additive)
                audioListener.enabled = false;

            ShowCompletionVisuals();

#if DEVELOPMENT_BUILD
            Debug.Log(SceneManager.GetSceneByBuildIndex(levelNum).name + " loaded!");
#endif

            yield return new WaitForSeconds(waitOnLoadEnd);

            FadeOut();

            yield return new WaitForSeconds(fadeDuration);

            if (loadSceneMode == LoadSceneMode.Additive)
                operation = SceneManager.UnloadSceneAsync(currentScene.name);
            else
                operation.allowSceneActivation = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Loading the scene asynchronously
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        private IEnumerator LoadAsync(string levelName)
        {

#if DEVELOPMENT_BUILD
            Debug.Log("Loading " + SceneManager.GetSceneByName(levelName).name + " at " + DateTime.Now);
#endif

            ShowLoadingVisuals();

            yield return null;

            FadeIn();
            StartOperation(levelName);

            float lastProgress = 0f;

            // operation does not auto-activate scene, so it's stuck at 0.9
            while (!DoneLoading())
            {
                yield return null;

#if DEVELOPMENT_BUILD
                Debug.Log(operation.progress + "%...");
#endif

                if (!Mathf.Approximately(operation.progress, lastProgress))
                {
                    progressBar.fillAmount = operation.progress;
                    lastProgress = operation.progress;
                }
            }

            if (loadSceneMode == LoadSceneMode.Additive)
                audioListener.enabled = false;

            ShowCompletionVisuals();

#if DEVELOPMENT_BUILD
            Debug.Log(SceneManager.GetSceneByName(sceneName).name + " loaded!");
#endif

            yield return new WaitForSeconds(waitOnLoadEnd);

            FadeOut();

            yield return new WaitForSeconds(fadeDuration);

            if (loadSceneMode == LoadSceneMode.Additive)
                operation = SceneManager.UnloadSceneAsync(currentScene.name);
            else
                operation.allowSceneActivation = true;
        }

        /****************************************************************************************************/

        private void StartOperation(int levelNum)
        {
            Application.backgroundLoadingPriority = loadThreadPriority;
            operation = SceneManager.LoadSceneAsync(levelNum, loadSceneMode);


            if (loadSceneMode == LoadSceneMode.Single)
                operation.allowSceneActivation = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void StartOperation(string levelName)
        {
            Application.backgroundLoadingPriority = loadThreadPriority;
            operation = SceneManager.LoadSceneAsync(levelName, loadSceneMode);


            if (loadSceneMode == LoadSceneMode.Single)
                operation.allowSceneActivation = false;
        }

        /****************************************************************************************************/

        private bool DoneLoading()
        {
            return (loadSceneMode == LoadSceneMode.Additive && operation.isDone) || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);
        }

        void FadeIn()
        {
            fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
        }

        void FadeOut()
        {
            fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
        }

        void ShowLoadingVisuals()
        {
            loadingIcon.gameObject.SetActive(true);
            loadingDoneIcon.gameObject.SetActive(false);

            progressBar.fillAmount = 0f;
            loadingText.text = loadingString;
        }

        void ShowCompletionVisuals()
        {
            loadingIcon.gameObject.SetActive(false);
            loadingDoneIcon.gameObject.SetActive(true);

            progressBar.fillAmount = 1f;
            loadingText.text = loadingDoneString;
        }

    }
}
