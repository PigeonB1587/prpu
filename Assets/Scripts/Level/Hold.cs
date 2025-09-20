using System;
using System.Collections;
using System.Collections.Generic;
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
        public bool overJudge = false;

        public override void Awake()
        {
            base.Awake();
            isFirstJudge = true;
            overJudge = false;
        }

        public override void Update()
        {
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(transform.localPosition.x, isJudge ? 0 : noteData.above ? floorPosition : -floorPosition);
            bool visable = false;
            if (judgeLine.levelController.time >= noteData.startTime.curTime)
                Judge();
            if (transform.position.y >= -10 && transform.position.y <= 10)
            {
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
        }

        public override void Judge()
        {
            isFirstJudge = false;
            isJudge = true;
        }

        public override void ResetNote()
        {
            isJudge = false;
            isFirstJudge = true;
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
            if (transform.position.y >= -10 && transform.position.y <= 10 && floorPosition >= -0.001)
            {
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
            }
            if (judgeLine.disappear < 0)
                visable = false;
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;
        }
    }
}
