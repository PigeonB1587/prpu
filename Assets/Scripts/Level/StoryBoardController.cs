using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    /// <summary>
    /// <para>type:</para>
    /// <para>    0 =>  mainCamera.orthographicSize</para>
    /// <para>    1 =>  mainCamera.gameObject.transform.eulerAngles.z</para>
    /// <para>    2 =>  chartName.color.a</para>
    /// <para>    3 =>  level.color.a</para>
    /// <para>    4 =>  score.color.a</para>
    /// <para>    5 =>  combo.color.a</para>
    /// <para>    6 =>  comboText.color.a</para>
    /// <para>    7 =>  pauseBtn.color.a</para>
    /// <para>    8 =>  audioSource.volume</para>
    /// <para>    9 =>  audioSource.pitch</para>
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
        public AudioSource audioSource;
        public Image pauseBtn;
        public Text chartName, level, score, combo, comboText;

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

            if (_cachedEventsByType.TryGetValue(2, out var type2Events) && type2Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type2Events, ref alpha);
                var color = chartName.color;
                color.a = alpha;
                chartName.color = color;
            }

            if (_cachedEventsByType.TryGetValue(3, out var type3Events) && type3Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type3Events, ref alpha);
                var color = level.color;
                color.a = alpha;
                level.color = color;
            }

            if (_cachedEventsByType.TryGetValue(4, out var type4Events) && type4Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type4Events, ref alpha);
                var color = score.color;
                color.a = alpha;
                score.color = color;
            }

            if (_cachedEventsByType.TryGetValue(5, out var type5Events) && type5Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type5Events, ref alpha);
                var color = combo.color;
                color.a = alpha;
                combo.color = color;
            }

            if (_cachedEventsByType.TryGetValue(6, out var type6Events) && type6Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type6Events, ref alpha);
                var color = comboText.color;
                color.a = alpha;
                comboText.color = color;
            }

            if (_cachedEventsByType.TryGetValue(7, out var type7Events) && type7Events.Length > 0)
            {
                float alpha = 0;
                UpdateEvent(levelController.time, type7Events, ref alpha);
                var color = pauseBtn.color;
                color.a = alpha;
                pauseBtn.color = color;
            }

            if (_cachedEventsByType.TryGetValue(8, out var type8Events) && type8Events.Length > 0)
            {
                float volume = 0;
                UpdateEvent(levelController.time, type8Events, ref volume);
                audioSource.volume = volume;
            }

            if (_cachedEventsByType.TryGetValue(9, out var type9Events) && type9Events.Length > 0)
            {
                float pitch = 0;
                UpdateEvent(levelController.time, type9Events, ref pitch);
                audioSource.pitch = pitch;
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