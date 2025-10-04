using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

namespace PigeonB1587.prpu
{
    /// <summary>
    /// type:
    ///     0 =>  mainCamera.orthographicSize
    ///     1 =>  mainCamera.gameObject.transform.eulerAngles.z
    /// </summary>
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

        public Camera mainCamera;
        private void Awake()
        {
            levelController = GetComponent<LevelController>();
        }

        public void Update()
        {
            if (levelController.isPlaying && !levelController.isOver)
            {
                if(events.Length != 0) UpdateStoryBoard();
            }
        }

        public void UpdateStoryBoard()
        {
            foreach (var item in events)
            {
                switch (item.type)
                {
                    case 0:
                        float i = 0;
                        UpdateEvent(levelController.time, events
            .Where(e => e != null && e.type == 0 && e.@event != null)
            .Select(e => e.@event)
            .ToArray(), ref i);
                        mainCamera.orthographicSize = i;
                        break;
                    case 1:
                        float i1 = 0;
                        UpdateEvent(levelController.time, events
            .Where(e => e != null && e.type == 1 && e.@event != null)
            .Select(e => e.@event)
            .ToArray(), ref i1);
                        mainCamera.gameObject.transform.eulerAngles = new Vector3(mainCamera.gameObject.transform.eulerAngles.x, mainCamera.gameObject.transform.eulerAngles.y, i1);
                        break;
                }
            }
        }


        private void UpdateEvent(double currentTime, ChartObject.JudgeLineEvent[] events, ref float value)
        {
            if (events.Length == 0)
                return;
            int i = GetEventIndex(currentTime, ref events);
            var @event = events[i];
            if (currentTime >= @event.endTime.curTime)
            {
                value = @event.end;
                return;
            }
            value += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start, @event.end, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints);
        }

        private int GetEventIndex(double curTime, ref ChartObject.JudgeLineEvent[] events)
        {
            int left = 0;
            int right = events.Length - 1;
            int index = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (events[mid].startTime.curTime <= curTime)
                {
                    index = mid;
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return index;
        }
    }
}