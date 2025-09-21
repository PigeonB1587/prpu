using System;
using System.Collections;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Hold : NoteObject
    {
        public Sprite hlImage1;
        public Sprite defaultImage1;
        public SpriteRenderer noteRenderer1;
        public Sprite hlImage2;
        public Sprite defaultImage2;
        public SpriteRenderer noteRenderer2;

        public bool isFirstJudge = true;
        public bool isHolding = false;
        public bool overJudge = false;

        public override void Awake()
        {
            base.Awake();
            isFirstJudge = true;
            isHolding = false;
            overJudge = false;
        }

        public override void Update()
        {
            floorPosition = GetFloorPosY();
            bool visable = false;
            if (isFirstJudge && judgeLine.levelController.time >= noteData.startTime.curTime)
                Judge();
            transform.localPosition = new Vector2(transform.localPosition.x, isJudge ? 0 : noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(transform.localScale.x, GetHoldLenght(), transform.localScale.z);

            if (floorPosition >= -0.001)
            {
                if (useVisableTime)
                {
                    if (noteData.startTime.curTime - judgeLine.levelController.time <= visableTimeData.curTime)
                    {
                        visable = true;
                    }
                }
                else
                {
                    visable = true;
                }
            }
            if (isJudge)
            {
                if (useVisableTime)
                {
                    if (noteData.startTime.curTime - judgeLine.levelController.time <= visableTimeData.curTime)
                    {
                        visable = true;
                    }
                }
                else
                {
                    visable = true;
                }
            }
            if (judgeLine.disappear < 0)
                visable = false;
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;

            if (isJudge)
            {
                noteRenderer1.enabled = false;
            }
        }

        public override void Judge()
        {
            isFirstJudge = false;
            isHolding = true;
            isJudge = true;
            StartCoroutine(Holding());
        }

        IEnumerator Holding()
        {
            while (isHolding)
            {
                if (judgeLine.levelController.time >= noteData.endTime.curTime)
                {
                    noteRenderer.enabled = false;
                    noteRenderer1.enabled = false;
                    noteRenderer2.enabled = false;
                    if (judgeLine.levelController.time >= noteData.endTime.curTime + 0.08f)
                    {
                        break;
                    }
                }
                yield return null;
            }
            judgeLine.holdPool.Release(this);
        }

        public override void ResetNote()
        {
            isJudge = false;
            isFirstJudge = true;
            isHolding = false;
            overJudge = false;
            floorPosition = 0f;
            if (noteData.isHL)
            {
                noteRenderer.sprite = hlImage;
                noteRenderer1.sprite = hlImage1;
                noteRenderer2.sprite = hlImage2;
            }
            else
            {
                noteRenderer.sprite = defaultImage;
                noteRenderer1.sprite = defaultImage1;
                noteRenderer2.sprite = defaultImage2;
            }
            if (noteData.above)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            if (noteData.visibleTime != Array.Empty<int>())
            {
                visableTimeData.GetTimeLast(judgeLine.jugdeLineData.bpms, noteData.visibleTime);
                useVisableTime = true;
            }
            else
            {
                useVisableTime = false;
            }
            noteRenderer.color = Utils.IntToColor(noteData.color);

            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? floorPosition : -floorPosition);

            bool visable = false;
            if (floorPosition >= -0.001)
            {
                if (useVisableTime)
                {
                    if (noteData.startTime.curTime - judgeLine.levelController.time <= visableTimeData.curTime)
                    {
                        visable = true;
                    }
                }
                else
                {
                    visable = true;
                }
            }
            if (judgeLine.disappear < 0)
                visable = false;
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;
        }

        public float GetHoldLenght() => (judgeLine.levelController.time >= noteData.startTime.curTime ? noteData.endfloorPosition - judgeLine.floorPosition : noteData.endfloorPosition - noteData.floorPosition) * 0.0526316f;
    }
}
