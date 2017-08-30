using UnityEngine;
using MathBeat.Core;
using System.Collections;

namespace MathBeat.GUI
{
    public class DifficultyButton : MonoBehaviour
    {
        public int DifficultyIndex;
        public GameHandler GameData;

        private void Start()
        {
            GameData = GameHandler.Game;
        }

        public void SetDifficulty()
        {
            GameData.GameDifficulty = DifficultyIndex;
        }
    } 
}
