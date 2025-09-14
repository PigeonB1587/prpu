using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class JudgeLine : MonoBehaviour
    {
        public ChartObject.JudgeLine jugdeLineData;
        public SpriteRenderer lineRenderer;
        public LevelController levelController;
        public NotePool notePool;

        public List<ChartObject.Note> notes;

        public bool usingCustomColor;
        public Color perfectLine;
        public Color goodLine;
        public Color defaultLine;

        public float moveX = 0;
        public float moveY = 0;
        public float rotate = 0;
        public float disappear = 0;

        public float floorPosition = 0;

        public Transform fatherLine;
        public Color endColor;
        public void Start()
        {
            lineRenderer.color = GameInformation.Instance.isFCAPIndicator ? perfectLine : defaultLine;
            lineRenderer.size = new Vector2(jugdeLineData.transform.judgeLineTextureSize[0],
                jugdeLineData.transform.judgeLineTextureSize[1]);
            transform.position = new Vector2(500f, 500f);
            lineRenderer.sortingOrder = jugdeLineData.transform.zOrder;
            if (jugdeLineData.transform.judgeLineColorEvents.Length != 0)
            {
                usingCustomColor = true;
            }

            notes = jugdeLineData.notes.ToList();
        }

        public void Update()
        {
            if (!levelController.isLoading)
            {
                double curTime = levelController.time;
                moveX = 0;
                moveY = 0;
                rotate = 0;
                disappear = 0;
                UpdateEventLayers(curTime);
                UpdateNote();
            }
        }

        public void UpdateNote()
        {
            for(int i = 0; i < notes.Count; i++)
            {
                var f = notes[i].floorPosition - floorPosition + notes[i].positionY;
                var v = transform.TransformPoint(new Vector3(notes[i].positionX, f * notes[i].speed, 0)).y;

                if(v >= -10 && v <= 10)
                {
                    TapController n;
                    switch (notes[i].type)
                    {
                        case 1:
                            n = notePool.GetTap(transform);
                            break;
                        case 2:
                            n = notePool.GetDrag(transform);
                            break;
                        default:
                            n = notePool.GetDrag(transform);
                            break;
                    }
                    n.noteData = notes[i];
                    n.judgeLine = this;
                    n.Start();
                    notes.RemoveAt(i);
                    i--;
                }
            }
        }

        public void UpdateTransform()
        {
            float x = moveX * 17.77778f * (GameInformation.Instance.screenRadio / (16f / 9f));
            float y = moveY * 10f;
            Vector2 basePos = new Vector2(x, y);

            if (jugdeLineData.transform.localPositionMode)
            {
                transform.localPosition = jugdeLineData.transform.fatherLineIndex == -1
                    ? basePos
                    : fatherLine.transform.TransformPoint(basePos);
            }
            else
            {
                transform.position = basePos;
            }

            Quaternion targetRot = Quaternion.Euler(0, 0, rotate);

            if (jugdeLineData.transform.localEulerAnglesMode)
            {
                if (jugdeLineData.transform.fatherLineIndex != -1)
                {
                    targetRot = Quaternion.Euler(fatherLine.transform.localEulerAngles) * targetRot;
                }
                transform.localEulerAngles = targetRot.eulerAngles;
            }
            else
            {
                transform.eulerAngles = targetRot.eulerAngles;
            }

            if (!usingCustomColor)
            {
                endColor = new Color(perfectLine.r, perfectLine.g, perfectLine.b, disappear);
            }
            else
            {

            }

            lineRenderer.color = endColor;
        }

        private void UpdateEventLayers(double currentTime)
        {
            for (int i = 0; i < jugdeLineData.judgeLineEventLayers.Length; i++)
            {
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineMoveXEvents, ref moveX);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineMoveYEvents, ref moveY);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineRotateEvents, ref rotate);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineDisappearEvents, ref disappear);
            }
            floorPosition = Reader.GetCurFloorPosition(currentTime, jugdeLineData.speedEvents);
        }

        /// <summary>
        /// 事件的逻辑就是 根据当前时间，获取对应的事件对象，并插值计算
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="events"></param>
        /// <param name="value">增量</param>
        private void UpdateEvent(double currentTime, ref ChartObject.JudgeLineEvent[] events, ref float value)
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
