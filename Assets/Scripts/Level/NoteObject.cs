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
            visableTimeData = new ChartObject.Time();
        }

        public virtual void Start()
        {
            ResetNote();
        }

        public virtual void Update()
        {
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(transform.localPosition.x, noteData.above ? floorPosition : -floorPosition);
            if (judgeLine.levelController.time >= noteData.startTime.curTime)
                Judge();
            noteRenderer.enabled = GetNoteVisable();
        }

        public virtual void ResetNote()
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? floorPosition : -floorPosition);
            
            noteRenderer.enabled = GetNoteVisable();
        }

        public virtual void Judge()
        {
            
        }

        public virtual void GetNoteData()
        {
            isJudge = false;
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
        }

        public virtual bool GetNoteVisable()
        {
            bool visable = false;
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
            if (judgeLine.disappear < 0)
                visable = false;
            return visable;
        }

        public virtual float GetFloorPosY() => (noteData.floorPosition - judgeLine.floorPosition + noteData.positionY) * noteData.speed;
    }
}
