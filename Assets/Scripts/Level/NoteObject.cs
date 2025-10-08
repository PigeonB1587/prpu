using System;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class NoteObject : MonoBehaviour
    {
        public int index = 0;
        public ChartObject.Note noteData;
        public JudgeLine judgeLine;
        public SpriteRenderer noteRenderer;
        public Sprite hlImage;
        public Sprite defaultImage;
        public float floorPosition = 0f;
        public bool isJudge = false;

        public Color noteColor;
        public Color hitFxColor;
        public float noteScale;
        public float noteX;

        public float xPosControl;
        public float yPosControl;
        public float rotateControl;
        public float sizeControl;
        public float disappearControl;

        public virtual void Awake()
        {
            noteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Start()
        {
            ResetNote(judgeLine.levelController.time);
        }

        public virtual void Update()
        {
            double curTime = judgeLine.levelController.time;
            Judge(curTime);
            floorPosition = GetFloorPosY();
            SetNoteTransform(curTime);
        }

        public virtual void SetNoteTransform(double curTime)
        {
            var judgeData = judgeLine.judgeLineData.noteControls;
            var control = judgeLine;

            xPosControl = control.GetControlValue(floorPosition, judgeData.xPosControl);
            yPosControl = control.GetControlValue(floorPosition, judgeData.yPosControl);
            rotateControl = control.GetControlValue(floorPosition, judgeData.rotateControls, 0);
            sizeControl = control.GetControlValue(floorPosition, judgeData.sizeControl);
            disappearControl = control.GetControlValue(floorPosition, judgeData.disappearControls);

            float xPos = noteX * xPosControl;
            float yPos = (noteData.above ? floorPosition : -floorPosition) * yPosControl;
            transform.localPosition = new Vector2(xPos, yPos);
            float zRotation = noteData.above ? 0f : 180f;
            transform.localEulerAngles = new Vector3(0, 0, zRotation + rotateControl);
            float scale = noteScale * sizeControl;
            transform.localScale = new Vector3(scale, scale, scale);
            noteRenderer.color = new Color(
                noteColor.r,
                noteColor.g,
                noteColor.b,
                noteColor.a * disappearControl
            );
            noteRenderer.enabled = GetNoteVisable(curTime);
        }


        public virtual void ResetNote(double curTime)
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            SetNoteTransform(curTime);
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
            noteColor = Utils.IntToColor(noteData.color);
            hitFxColor = Utils.IntToColor(noteData.hitFXColor);
            noteScale = 0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale;
            noteX = noteData.positionX * GameInformation.Instance.screenRadioScale;
        }

        public virtual bool GetNoteVisable(double curTime)
        {
            bool visable = false;
            if (transform.position.y >= -10 && transform.position.y <= 10)
            {
                if (floorPosition >= -0.001)
                {
                    if (curTime >= noteData.visibleTime.curTime)
                    {
                        visable = true;
                    }
                }
            }
            if (judgeLine.disappear < 0)
                visable = false;
            return visable;
        }

        public virtual float GetFloorPosY() => (float)(noteData.floorPosition - judgeLine.floorPosition + (noteData.above ? 1 : -1) * noteData.positionY) * noteData.speed;
    }
}