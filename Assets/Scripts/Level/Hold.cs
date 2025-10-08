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

        private float holdEffectTimer;
        public const float holdLengthScale = 0.0526316f;

        public override void Awake()
        {
            base.Awake();
            isFirstJudge = true;
            isHolding = false;
            overJudge = false;
        }

        public override void Update()
        {
            double curTime = judgeLine.levelController.time;
            var isOverStartTime = curTime >= noteData.startTime.curTime;
            floorPosition = isOverStartTime ? 0 : GetFloorPosY();
            Judge(curTime);

            SetNoteTransform(curTime);

            var visable = GetNoteVisable(curTime);
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;

            if (isOverStartTime || isHolding)
            {
                noteRenderer1.enabled = false;
            }

            if (visable == false && !isJudge && !isHolding && judgeLine.levelController.time + GameInformation.Instance.noteToLargeTime < noteData.startTime.curTime)
            {
                judgeLine.AddNote(noteData, index);
                judgeLine.holdPool.Release(this);
            }
        }

        public override void SetNoteTransform(double curTime)
        {
            var isOverStartTime = curTime >= noteData.startTime.curTime;
            var judgeData = judgeLine.judgeLineData.noteControls;
            var control = judgeLine;

            xPosControl = control.GetControlValue(floorPosition, judgeData.xPosControl);
            yPosControl = control.GetControlValue(floorPosition, judgeData.yPosControl);
            rotateControl = control.GetControlValue(floorPosition, judgeData.rotateControls, 0);
            sizeControl = control.GetControlValue(floorPosition, judgeData.sizeControl);
            disappearControl = control.GetControlValue(floorPosition, judgeData.disappearControls);

            float xPos = noteX * xPosControl;
            float yPos = (isOverStartTime ? noteData.positionY : (noteData.above ? floorPosition : -floorPosition)) * yPosControl;
            float zRotation = noteData.above ? 0f : 180f;
            float scale = noteScale * sizeControl;

            transform.localPosition = new Vector2(xPos, yPos);
            transform.localScale = new Vector3(scale, GetHoldLenght(curTime) * sizeControl, scale);
            transform.localEulerAngles = new Vector3(0, 0, zRotation + rotateControl);

            noteRenderer.color = new Color(
                noteColor.r,
                noteColor.g,
                noteColor.b,
                noteColor.a * disappearControl
            );
            noteRenderer1.color = new Color(
                noteColor.r,
                noteColor.g,
                noteColor.b,
                noteColor.a * disappearControl
            );
            noteRenderer2.color = new Color(
                noteColor.r,
                noteColor.g,
                noteColor.b,
                noteColor.a * disappearControl
            );
        }

        public override void Judge(double curTime)
        {
            if (isFirstJudge && !noteData.isFake && curTime >= noteData.startTime.curTime)
            {
                isFirstJudge = false;
                isHolding = true;
                isJudge = true;
                judgeLine.levelController.hitFxController.GetHitFx(HitType.Perfect,
                    transform.position,
                    1, lineIndex: judgeLine.index, noteIndex: index, hitFxColor: noteData.hitFXColor != -1 ? hitFxColor : null);
                StartCoroutine(Holding());
            }
            else if (isFirstJudge && noteData.isFake && curTime >= noteData.startTime.curTime)
            {
                isFirstJudge = false;
                isHolding = true;
                isJudge = true;
                StartCoroutine(FakeHolding());
            }
        }

        IEnumerator Holding()
        {
            while (isHolding)
            {
                holdEffectTimer += Time.deltaTime;
                if (holdEffectTimer > (60 / judgeLine.bpm) / 2 && !noteData.isFake && judgeLine.levelController.time <= noteData.endTime.curTime - 0.08f)
                {
                    judgeLine.levelController.hitFxController.GetHitFx(HitType.Perfect,
                        judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)),
                        3, hitFxColor: noteData.hitFXColor != -1 ? hitFxColor : null);
                    holdEffectTimer = 0.0f;
                }

                if (!overJudge && judgeLine.levelController.time > noteData.endTime.curTime - 0.08f)
                {
                    ScoreController.Hit(HitType.Perfect, 0);
                    overJudge = true;
                }

                if (judgeLine.levelController.time >= noteData.endTime.curTime)
                {
                    Hide();
                    if (judgeLine.levelController.time >= noteData.endTime.curTime + 0.08f)
                    {
                        break;
                    }
                }
                yield return null;
            }
            judgeLine.holdPool.Release(this);
        }

        IEnumerator FakeHolding()
        {
            while (isHolding)
            {
                if (!overJudge && judgeLine.levelController.time > noteData.endTime.curTime - 0.08f)
                {
                    overJudge = true;
                }
                if (judgeLine.levelController.time >= noteData.endTime.curTime)
                {
                    break;
                }
                yield return null;
            }
            judgeLine.holdPool.Release(this);
        }

        public void Hide()
        {
            noteRenderer.enabled = false;
            noteRenderer1.enabled = false;
            noteRenderer2.enabled = false;
        }

        public void OnDestroy() => StopCoroutine(Holding());

        public override void ResetNote(double curTime)
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            SetNoteTransform(curTime);
            var visable = GetNoteVisable(curTime);
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;
        }

        public override void GetNoteData()
        {
            base.GetNoteData();
            holdEffectTimer = 0.0f;
            isJudge = false;
            isFirstJudge = true;
            isHolding = false;
            overJudge = false;

            if (noteData.isHL)
            {
                noteRenderer1.sprite = hlImage1;
                noteRenderer2.sprite = hlImage2;
            }
            else
            {
                noteRenderer1.sprite = defaultImage1;
                noteRenderer2.sprite = defaultImage2;
            }
        }

        public override bool GetNoteVisable(double curTime)
        {
            bool visable = false;
            if (Utils.GetHoldVisable(noteRenderer1.transform.position, noteRenderer2.transform.position, float.MinValue,
                -10, float.MaxValue, 10))
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

        public float GetHoldLenght(double curTime) => (float)(curTime >= noteData.startTime.curTime
            ? noteData.endfloorPosition - judgeLine.floorPosition :
            noteData.endfloorPosition - noteData.floorPosition) * holdLengthScale;
    }
}