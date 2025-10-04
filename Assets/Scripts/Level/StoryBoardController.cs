using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    [System.Serializable]
    public class StoryBoardEvent
    {
        public int type;
        public ChartObject.JudgeLineEvent @event;
    }

    public class StoryBoardController : MonoBehaviour
    {
        public LevelController levelController;

        public StoryBoardEvent[] events;
        private void Awake()
        {
            levelController = GetComponent<LevelController>();
        }
    }
}