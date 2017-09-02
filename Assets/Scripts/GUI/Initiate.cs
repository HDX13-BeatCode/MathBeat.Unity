using UnityEngine;
using System.Collections;

namespace MathBeat.GUI
{
    public static class Initiate
    {
        //Create Fader object and assing the fade scripts and assign all the variables
        public static void Fade(string scene, Color col, float damp, bool showLoading = false)
        {
            GameObject init = new GameObject();
            init.name = "Fader";
            init.AddComponent<Fader>();
            Fader scr = init.GetComponent<Fader>();
            scr.fadeDamp = damp;
            scr.fadeScene = scene;
            scr.fadeColor = col;
            scr.showLoadingScreen = showLoading;
            scr.start = true;
        }

        public static void Fade(int scene, Color col, float damp, bool showLoading = false)
        {
            GameObject init = new GameObject();
            init.name = "Fader";
            init.AddComponent<Fader>();
            Fader scr = init.GetComponent<Fader>();
            scr.fadeDamp = damp;
            scr.fadeSceneIdx = scene;
            scr.fadeColor = col;
            scr.showLoadingScreen = showLoading;
            scr.start = true;
        }

    } 
}
