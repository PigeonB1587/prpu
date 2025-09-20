using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class JudgeLine : MonoBehaviour
    {
        public ChartObject.JudgeLine jugdeLineData;
        public SpriteRenderer lineRenderer;
        public LevelController levelController;

        public List<ChartObject.Note> notes;
        public GameObject tapPrefab;
        public ObjectPool<Tap> tapPool;

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
            SetNotePool();
        }
        private void SetNotePool()
        {
            tapPool = new ObjectPool<Tap>(
                createFunc: () => Instantiate(tapPrefab, transform).GetComponent<Tap>(),
                actionOnGet: (tap) =>
                {
                    tap.gameObject.SetActive(true);
                    tap.isJudge = false;
                },
                actionOnRelease: (tap) => tap.gameObject.SetActive(false),
                actionOnDestroy: (tap) => Destroy(tap.gameObject),
                defaultCapacity: 20,
                maxSize: 3000
            );
        }

        public void UpdateLine()
        {
            if (!levelController.isLoading)
            {
                double curTime = levelController.time;
                moveX = 0;
                moveY = 0;
                rotate = 0;
                disappear = 0;
                UpdateEventLayers(curTime);
            }
        }

        public void UpdateTransform()
        {
            float x = moveX * 17.77778f * GameInformation.Instance.screenRadioScale;
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

        public void UpdateNote()
        {
            for(int i = 0; i < notes.Count; i++)
            {
                var f = notes[i].floorPosition - floorPosition + notes[i].positionY;
                var v = transform.TransformVector(new Vector3(notes[i].positionX * GameInformation.Instance.screenRadioScale,
                    f * notes[i].speed,
                    0)).y;

                if (v >= -10 && v <= 10)
                {
                    NoteObject n;
                    switch (notes[i].type)
                    {
                        case 1:
                            n = tapPool.Get();
                            break;
                        default:
                            n = tapPool.Get();
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

        private void UpdateEventLayers(double currentTime)
        {
            for (int i = 0; i < jugdeLineData.judgeLineEventLayers.Length; i++)
            {
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineMoveXEvents, ref moveX);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineMoveYEvents, ref moveY);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineRotateEvents, ref rotate);
                UpdateEvent(currentTime, ref jugdeLineData.judgeLineEventLayers[i].judgeLineDisappearEvents, ref disappear);
            }
            floorPosition = Utils.GetCurFloorPosition(currentTime, jugdeLineData.speedEvents);
        }

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
