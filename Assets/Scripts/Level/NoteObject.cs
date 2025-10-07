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
            floorPosition = GetFloorPosY();
            SetNoteTransform();
            Judge(curTime);
            noteRenderer.enabled = GetNoteVisable(curTime);
        }
        
        public virtual void SetNoteTransform()
        {
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.xPosControl), (noteData.above ? floorPosition : -floorPosition) * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.yPosControl));
            if (noteData.above)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0 * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.rotateControls));
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 180 * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.rotateControls));
            }
            var scale = 0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.sizeControl);
            transform.localScale = new Vector3(scale,
                scale,
                scale);
            noteRenderer.color = new Color(noteColor.r, noteColor.g, noteColor.b, noteColor.a * judgeLine.GetControlValue(floorPosition, judgeLine.judgeLineData.noteControls.disappearControls));
        }

        public virtual void ResetNote(double curTime)
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            SetNoteTransform();
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
            noteColor = Utils.IntToColor(noteData.color);
            hitFxColor = Utils.IntToColor(noteData.hitFXColor);
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
