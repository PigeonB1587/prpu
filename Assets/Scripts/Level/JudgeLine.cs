using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace PigeonB1587.prpu
{
    public class JudgeLine : MonoBehaviour
    {
        public int index = 0;
        public ChartObject.JudgeLine jugdeLineData;
        public SpriteRenderer lineRenderer;
        public LevelController levelController;

        public List<(ChartObject.Note note, int index)> localNotes = new();
        public GameObject tapPrefab, dragPrefab, flickPrefab, holdPrefab;
        public ObjectPool<Tap> tapPool;
        public ObjectPool<Drag> dragPool;
        public ObjectPool<Flick> flickPool;
        public ObjectPool<Hold> holdPool;

        public bool usingCustomColor;

        public float moveX = 0;
        public float moveY = 0;
        public float rotate = 0;
        public float disappear = 0;

        public float bpm = 120f;

        public float defautImageX;
        public float defautImageY;

        public float colorR = 1, colorG = 1, colorB = 1;
        public float scaleX = 1, scaleY = 1;

        public float floorPosition = 0;

        public Transform fatherLine;
        public Color endColor;
        public void Start()
        {
            lineRenderer.color = GameInformation.Instance.isFCAPIndicator ? levelController.perfectLine : levelController.defaultLine;
            lineRenderer.sortingOrder = jugdeLineData.transform.zOrder;
            defautImageX = lineRenderer.size.x;
            defautImageY = lineRenderer.size.y;
            if (jugdeLineData.transform.judgeLineColorEvents.Length != 0)
            {
                usingCustomColor = true;
            }
            localNotes = jugdeLineData.notes
                .Select((note, idx) => (note, idx))
                .ToList();
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
            dragPool = new ObjectPool<Drag>(
                createFunc: () => Instantiate(dragPrefab, transform).GetComponent<Drag>(),
                actionOnGet: (drag) =>
                {
                    drag.gameObject.SetActive(true);
                    drag.isJudge = false;
                },
                actionOnRelease: (drag) => drag.gameObject.SetActive(false),
                actionOnDestroy: (drag) => Destroy(drag.gameObject),
                defaultCapacity: 20,
                maxSize: 3000
            );
            flickPool = new ObjectPool<Flick>(
                createFunc: () => Instantiate(flickPrefab, transform).GetComponent<Flick>(),
                actionOnGet: (flick) =>
                {
                    flick.gameObject.SetActive(true);
                    flick.isJudge = false;
                },
                actionOnRelease: (flick) => flick.gameObject.SetActive(false),
                actionOnDestroy: (flick) => Destroy(flick.gameObject),
                defaultCapacity: 20,
                maxSize: 3000
            );
            holdPool = new ObjectPool<Hold>(
                createFunc: () => Instantiate(holdPrefab, transform).GetComponent<Hold>(),
                actionOnGet: (hold) =>
                {
                    hold.gameObject.SetActive(true);
                    hold.isFirstJudge = true;
                    hold.isHolding = false;
                    hold.overJudge = false;
                    hold.isJudge = false;
                },
                actionOnRelease: (hold) => hold.gameObject.SetActive(false),
                actionOnDestroy: (hold) => Destroy(hold.gameObject),
                defaultCapacity: 20,
                maxSize: 3000
            );
        }

        public void UpdateLine(double curTime)
        {
            // reset values
            {
                moveX = 0;
                moveY = 0;
                rotate = 0;
                disappear = 0;

                colorR = 0;
                colorG = 0;
                colorB = 0;

                scaleX = 0;
                scaleY = 0;
            }
            //
            UpdateEventLayers(curTime);
            UpdateBpms(curTime, ref jugdeLineData.bpms, ref bpm);
            floorPosition = Utils.GetCurFloorPosition(curTime, jugdeLineData.speedEvents);
        }

        public void UpdateTransform()
        {
            float x = moveX * 17.77778f * GameInformation.Instance.screenRadioScale;
            float y = moveY * 10f;
            Vector2 basePos = new Vector2(x, y);

            if (jugdeLineData.transform.localPositionMode)
            {
                transform.position = jugdeLineData.transform.fatherLineIndex == -1
                    ? basePos
                    : Utils.LocalToWorld(basePos, fatherLine.transform.position, fatherLine.transform.eulerAngles.z);
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
                    targetRot = Quaternion.Euler(fatherLine.transform.eulerAngles) * targetRot;
                }
                transform.eulerAngles = targetRot.eulerAngles;
            }
            else
            {
                transform.eulerAngles = targetRot.eulerAngles;
            }

            if (!usingCustomColor)
            {
                endColor = new Color(levelController.perfectLine.r, levelController.perfectLine.g, levelController.perfectLine.b, disappear);
            }
            else
            {
                endColor = new Color(colorR, colorG, colorB, disappear);
            }

            lineRenderer.size = new Vector2(scaleX * defautImageX, scaleY * defautImageY);

            lineRenderer.color = endColor;
        }

        public void UpdateNote()
        {
            double currentTime = levelController.time;
            float visableY0 = GameInformation.Instance.visableY[0];
            float visableY1 = GameInformation.Instance.visableY[1];
            float visableX0 = GameInformation.Instance.visableX[0];
            float visableX1 = GameInformation.Instance.visableX[1];
            float screenRadioScale = GameInformation.Instance.screenRadioScale;

            for (int i = localNotes.Count - 1; i >= 0; i--)
            {
                var (note, originalIndex) = localNotes[i];
                bool shouldGetNote = false;

                float baseY = (float)(note.floorPosition - floorPosition) + note.positionY;
                Vector3 localPos = new Vector3(
                    note.positionX * screenRadioScale,
                    baseY * note.speed,
                    0);
                Vector3 worldPos = Utils.LocalToWorld(localPos, transform.position, transform.eulerAngles.z);

                if (note.type != 3)
                {
                    shouldGetNote = (worldPos.y > visableY0 && worldPos.y < visableY1 &&
                                   worldPos.x > visableX0 && worldPos.x < visableX1) ||
                                  (note.startTime.curTime - currentTime <= 0.3f);
                }
                else
                {
                    float endBaseY = (float)(note.endfloorPosition - floorPosition) + note.positionY;
                    Vector3 endLocalPos = new Vector3(
                        note.positionX * screenRadioScale,
                        endBaseY * note.speed,
                        0);
                    Vector3 endWorldPos = Utils.LocalToWorld(endLocalPos, transform.position, transform.eulerAngles.z);

                    shouldGetNote = Utils.GetHoldVisable(worldPos, endWorldPos, visableX0, visableY0, visableX1, visableY1) ||
                                  (note.startTime.curTime - currentTime <= 0.3f);
                }

                if (shouldGetNote)
                {
                    GetAndSetupNote(note, originalIndex, i);
                }
            }
        }

        private void GetAndSetupNote(ChartObject.Note note, int originalIndex, int listIndex)
        {
            NoteObject noteObject = note.type switch
            {
                1 => tapPool.Get(),
                2 => dragPool.Get(),
                3 => holdPool.Get(),
                4 => flickPool.Get(),
                _ => tapPool.Get()
            };

            noteObject.noteData = note;
            noteObject.judgeLine = this;
            noteObject.index = originalIndex;

            noteObject.Start();
            localNotes.RemoveAt(listIndex);
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

            if (usingCustomColor)
            {
                UpdateColorEvent(currentTime, ref jugdeLineData.transform.judgeLineColorEvents, ref colorR, ref colorG, ref colorB);
            }

            if (jugdeLineData.transform.judgeLineTextureScaleXEvents.Length != 0)
            {
                UpdateEvent(currentTime, ref jugdeLineData.transform.judgeLineTextureScaleXEvents, ref scaleX);
            }
            else
            {
                scaleX++;
            }
            if (jugdeLineData.transform.judgeLineTextureScaleYEvents.Length != 0)
            {
                UpdateEvent(currentTime, ref jugdeLineData.transform.judgeLineTextureScaleYEvents, ref scaleY);
            }
            else
            {
                scaleY++;
            }
        }

        private void UpdateEvent(double currentTime, ref ChartObject.JudgeLineEvent[] events, ref float value)
        {
            if (events.Length == 0)
                return;
            int i = GetEventIndex(currentTime, ref events);
            var @event = events[i];
            if (currentTime >= @event.endTime.curTime)
            {
                value += @event.end;
                return;
            }
            else
            {
                value += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start, @event.end, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints);
            }
        }

        private void UpdateColorEvent(double currentTime, ref ChartObject.ColorEvent[] events, ref float value1, ref float value2, ref float value3)
        {
            if (events.Length == 0)
                return;
            int i = GetColorEventIndex(currentTime, ref events);
            var @event = events[i];
            if (currentTime >= @event.endTime.curTime)
            {
                value1 += @event.end.r;
                value2 += @event.end.g;
                value3 += @event.end.b;
                return;
            }
            else
            {
                value1 += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start.r, @event.end.r, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints);
                value2 += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start.g, @event.end.g, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints);
                value3 += Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start.b, @event.end.b, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints);
            }
        }


        private void UpdateBpms(double currentTime, ref ChartObject.BpmItems[] events, ref float value)
        {
            if (events.Length == 0)
                return;
            int i = GetBpmIndex(currentTime, ref events);
            var @event = events[i];
            if (currentTime >= @event.time.curTime)
            {
                value = @event.bpm;
                return;
            }
            value = @event.bpm;
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

        private int GetColorEventIndex(double curTime, ref ChartObject.ColorEvent[] events)
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

        private int GetBpmIndex(double curTime, ref ChartObject.BpmItems[] events)
        {
            int left = 0;
            int right = events.Length - 1;
            int index = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (events[mid].time.curTime <= curTime)
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
