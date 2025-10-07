using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

namespace PigeonB1587.prpu
{
    public class JudgeLine : MonoBehaviour
    {
        public int index = 0;
        public ChartObject.JudgeLine judgeLineData;
        public SpriteRenderer lineRenderer;
        public LevelController levelController;
        public GameObject textObject;
        private TextObject instanteTextObject;

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
        public float textProgress = 0;

        public float floorPosition = 0;

        public Transform fatherLine;
        public Color endColor;
        public void Start()
        {
            lineRenderer.color = GameInformation.Instance.isFCAPIndicator ? levelController.perfectLine : levelController.defaultLine;
            lineRenderer.sortingOrder = judgeLineData.transform.zOrder;
            defautImageX = lineRenderer.size.x;
            defautImageY = lineRenderer.size.y;
            if (judgeLineData.transform.judgeLineColorEvents.Length != 0)
            {
                usingCustomColor = true;
            }
            if (judgeLineData.transform.judgeLineTextEvents.Length != 0)
            {
                instanteTextObject = Instantiate(textObject, transform).GetComponent<TextObject>();
                lineRenderer.enabled = false;
            }
            localNotes = judgeLineData.notes
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

                textProgress = 0;
            }
            //
            UpdateEventLayers(curTime);
            UpdateBpms(curTime, ref judgeLineData.bpms, ref bpm);
            floorPosition = Utils.GetCurFloorPosition(curTime, judgeLineData.speedEvents);
        }

        public void UpdateTransform()
        {
            float x = moveX * 17.77778f * GameInformation.Instance.screenRadioScale;
            float y = moveY * 10f;
            Vector2 basePos = new Vector2(x, y);

            if (judgeLineData.transform.localPositionMode)
            {
                transform.position = judgeLineData.transform.fatherLineIndex == -1
                    ? basePos
                    : Utils.LocalToWorld(basePos, fatherLine.transform.position, fatherLine.transform.eulerAngles.z);
            }
            else
            {
                transform.position = basePos;
            }

            Quaternion targetRot = Quaternion.Euler(0, 0, rotate);

            if (judgeLineData.transform.localEulerAnglesMode)
            {
                if (judgeLineData.transform.fatherLineIndex != -1)
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

            if (judgeLineData.transform.judgeLineTextEvents.Length != 0)
            {
                instanteTextObject.progress = textProgress;
                instanteTextObject.text.color = endColor;
                instanteTextObject.transform.localScale = new Vector2(scaleX, scaleY);
            }
            else
            {
                lineRenderer.size = new Vector2(scaleX * defautImageX, scaleY * defautImageY);

                lineRenderer.color = endColor;
            }
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
                    SetNote(note, originalIndex, i);
                }
            }
        }

        private void SetNote(ChartObject.Note note, int originalIndex, int listIndex)
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

        public void AddNote(ChartObject.Note note, int noteIndex)
        {
            if (localNotes.Count <= 1) return;

            int left = 0;
            int right = localNotes.Count - 1;
            int index = localNotes.Count;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (localNotes[mid].note.startTime.curTime >= note.startTime.curTime)
                {
                    index = mid;
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            localNotes.Insert(index, (note, noteIndex));
        }

        public float GetControlValue(float floorPos, ChartObject.ControlItem[] controlItems, float defautValue = 1)
        {
            if (controlItems.Length == 0 || controlItems == null) return defautValue;

            var index = GetControlIndex(floorPos, ref controlItems);
            if (index == -1)
                return defautValue;
            var item = controlItems[index];
            if (index == 0)
                return item.value;
            return Easings.Lerp(item.easing, floorPos, controlItems[index - 1].x, item.x, controlItems[index - 1].value, item.value);
        }

        private void UpdateEventLayers(double currentTime)
        {
            for (int i = 0; i < judgeLineData.judgeLineEventLayers.Length; i++)
            {
                UpdateEvent(currentTime, ref judgeLineData.judgeLineEventLayers[i].judgeLineMoveXEvents, ref moveX);
                UpdateEvent(currentTime, ref judgeLineData.judgeLineEventLayers[i].judgeLineMoveYEvents, ref moveY);
                UpdateEvent(currentTime, ref judgeLineData.judgeLineEventLayers[i].judgeLineRotateEvents, ref rotate);
                UpdateEvent(currentTime, ref judgeLineData.judgeLineEventLayers[i].judgeLineDisappearEvents, ref disappear);
            }

            if (usingCustomColor)
            {
                UpdateColorEvent(currentTime, ref judgeLineData.transform.judgeLineColorEvents, ref colorR, ref colorG, ref colorB);
            }

            if (judgeLineData.transform.judgeLineTextureScaleXEvents.Length != 0)
            {
                UpdateEvent(currentTime, ref judgeLineData.transform.judgeLineTextureScaleXEvents, ref scaleX);
            }
            else
            {
                scaleX++;
            }
            if (judgeLineData.transform.judgeLineTextureScaleYEvents.Length != 0)
            {
                UpdateEvent(currentTime, ref judgeLineData.transform.judgeLineTextureScaleYEvents, ref scaleY);
            }
            else
            {
                scaleY++;
            }
            if (judgeLineData.transform.judgeLineTextEvents.Length != 0)
            {
                UpdateTextEvent(currentTime, ref judgeLineData.transform.judgeLineTextEvents, ref textProgress);
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

        private void UpdateTextEvent(double currentTime, ref ChartObject.TextEvent[] events, ref float value)
        {
            if (events.Length == 0)
                return;
            int i = GetTextEventIndex(currentTime, ref events);
            var @event = events[i];
            instanteTextObject.endText = @event.end;
            instanteTextObject.startText = @event.start;
            if (currentTime >= @event.endTime.curTime)
            {
                value += 1;
                return;
            }
            else
            {
                value += (Easings.Lerp(@event.easing, currentTime, @event.startTime.curTime, @event.endTime.curTime,
                @event.start.Length, @event.end.Length, @event.easingLeft, @event.easingRight, @event.bezierPoints != null && @event.bezierPoints.Length == 4 ? true : false,
                @event.bezierPoints)) / (float)@event.end.Length;
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

        private int GetTextEventIndex(double curTime, ref ChartObject.TextEvent[] events)
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

        private int GetControlIndex(float x, ref ChartObject.ControlItem[] events)
        {
            if (events == null || events.Length == 0)
                return 0;
            if (events.Length == 1)
                return 0;
            int left = 0;
            int right = events.Length - 1;
            if (x > events[right].x)
                return right;
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (events[mid].x >= x)
                {
                    right = mid;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return left > 0 && x > events[left - 1].x ? left : left - 1;
        }
    }
}
