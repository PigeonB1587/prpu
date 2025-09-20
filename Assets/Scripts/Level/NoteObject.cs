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
            transform.localPosition = new Vector2(transform.localPosition.x, noteData.above ? GetFloorPosY() : -GetFloorPosY());
            if (judgeLine.levelController.time >= noteData.startTime.curTime)
                Judge();
        }

        public virtual void ResetNote()
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
            if(noteData.visibleTime != Array.Empty<int>())
            {
                visableTimeData.GetTimeLast(judgeLine.jugdeLineData.bpms, noteData.visibleTime);
                useVisableTime = true;
            }
            else
            {
                useVisableTime = false;
            }
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? GetFloorPosY() : -GetFloorPosY());
        }

        public virtual void Judge()
        {
            
        }

        public virtual float GetFloorPosY() => (noteData.floorPosition - judgeLine.floorPosition + noteData.positionY) * noteData.speed;
    }
}
