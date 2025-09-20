using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class NoteObject : MonoBehaviour
    {
        public ChartObject.Note noteData;
        public bool useVisableTime = false;
        public ChartObject.Time visableTimeData;
        public JudgeLine judgeLine;
        public SpriteRenderer noteRenderer;
        public Sprite hlImage;
        public Sprite defaultImage;
        public float floorPosition = 0f;
        public bool isJudge = false;

        public virtual void Awake()
        {
            noteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Start()
        {
            ResetNote();
        }

        public virtual void Update()
        {
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(transform.localPosition.x, noteData.above ? floorPosition : -floorPosition);
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
            if (judgeLine.disappear < 0) // 还原一下Phiedit远古特性（？
                visable = false;
            noteRenderer.enabled = visable;
            if (judgeLine.levelController.time >= noteData.startTime.curTime)
                Judge();
        }

        public virtual void ResetNote()
        {
            isJudge = false;
            floorPosition = 0f;
            if (noteData.isHL)
            {
                noteRenderer.sprite = hlImage;
            }
            else
            {
                noteRenderer.sprite = defaultImage;
            }
            if (noteData.above)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            if(noteData.visibleTime != Array.Empty<int>())
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
            if (transform.localPosition.y >= -10 && transform.localPosition.y <= 10)
                visable = true;
            if (floorPosition >= -0.001)
                visable = true;
            if (useVisableTime && noteData.startTime.curTime - judgeLine.levelController.time <= visableTimeData.curTime)
                visable = true;
            if (judgeLine.disappear >= 0)
                visable = true;
            noteRenderer.enabled = visable;
        }

        public virtual void Judge()
        {
            
        }

        public virtual float GetFloorPosY() => (noteData.floorPosition - judgeLine.floorPosition + noteData.positionY) * noteData.speed;
    }
}
