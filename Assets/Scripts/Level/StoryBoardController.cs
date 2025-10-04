using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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

        private Dictionary<int, StoryBoardEvent[]> _cachedEventsByType;
        private bool _isCacheInitialized = false;

        private void Awake()
        {
            levelController = GetComponent<LevelController>();
        }

        public void SetCachedEvents(Dictionary<int, List<StoryBoardEvent>> eventsByType)
        {
            _cachedEventsByType = new Dictionary<int, StoryBoardEvent[]>();

            foreach (var kvp in eventsByType)
            {
                _cachedEventsByType[kvp.Key] = kvp.Value.ToArray();
            }

            _isCacheInitialized = true;
        }

        public void Update()
        {
            if (levelController.isPlaying && !levelController.isOver)
            {
                if (events.Length != 0 && _isCacheInitialized)
                    UpdateStoryBoard();
            }
        }

        public void UpdateStoryBoard()
        {
            if (!_isCacheInitialized) return;

            if (_cachedEventsByType.TryGetValue(0, out var type0Events) && type0Events.Length > 0)
            {
                float cameraSize = 0;
                UpdateEvent(levelController.time, type0Events, ref cameraSize);
                mainCamera.orthographicSize = cameraSize;
            }

            if (_cachedEventsByType.TryGetValue(1, out var type1Events) && type1Events.Length > 0)
            {
                float rotationZ = 0;
                UpdateEvent(levelController.time, type1Events, ref rotationZ);
                var currentEulerAngles = mainCamera.transform.eulerAngles;
                mainCamera.transform.eulerAngles = new Vector3(currentEulerAngles.x, currentEulerAngles.y, rotationZ);
            }
        }

        private void UpdateEvent(double currentTime, StoryBoardEvent[] storyBoardEvents, ref float value)
        {
            if (storyBoardEvents.Length == 0)
                return;

            int eventIndex = GetEventIndex(currentTime, storyBoardEvents);
            var @event = storyBoardEvents[eventIndex].@event;

            if (currentTime >= @event.endTime.curTime)
            {
                value = @event.end;
                return;
            }

            value += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start, @event.end, @event.easingLeft, @event.easingRight,
                @event.bezierPoints != null && @event.bezierPoints.Length == 4,
                @event.bezierPoints);
        }

        private int GetEventIndex(double curTime, StoryBoardEvent[] storyBoardEvents)
        {
            int left = 0;
            int right = storyBoardEvents.Length - 1;
            int index = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (storyBoardEvents[mid].@event.startTime.curTime <= curTime)
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