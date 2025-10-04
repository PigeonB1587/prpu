using System;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class NoteObject : MonoBehaviour
    {
        public int index = 0;
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
            ResetNote(judgeLine.levelController.time);
        }

        public virtual void Update()
        {
            double curTime = judgeLine.levelController.time;
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(transform.localPosition.x, noteData.above ? floorPosition : -floorPosition);
            Judge(curTime);
            noteRenderer.enabled = GetNoteVisable(curTime);
        }

        public virtual void ResetNote(double curTime)
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(0.22f * noteData.size * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale,
                0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale,
                0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale);
            noteRenderer.enabled = GetNoteVisable(curTime);
        }

        public virtual void Judge(double curTime)
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
                visableTimeData.GetNewTime(judgeLine.jugdeLineData.bpms, noteData.visibleTime);
                useVisableTime = true;
            }
            else
            {
                useVisableTime = false;
            }
            noteRenderer.color = Utils.IntToColor(noteData.color);
        }

        public virtual bool GetNoteVisable(double curTime)
        {
            bool visable = false;
            if (transform.position.y >= -10 && transform.position.y <= 10)
            {
                if (floorPosition >= -0.001)
                {
                    if (useVisableTime)
                    {
                        if (noteData.startTime.curTime - curTime <= visableTimeData.curTime)
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

        public virtual float GetFloorPosY() => (float)(noteData.floorPosition - judgeLine.floorPosition + noteData.positionY) * noteData.speed;
    }
}
