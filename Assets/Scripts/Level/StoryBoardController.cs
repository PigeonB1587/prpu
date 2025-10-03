using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class StoryBoardController : MonoBehaviour
    {
        public LevelController levelController;

        private void Awake()
        {
            levelController = GetComponent<LevelController>();
        }
    }
}