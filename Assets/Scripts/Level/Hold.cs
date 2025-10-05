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

            transform.localPosition = new Vector2(transform.localPosition.x, isOverStartTime ? noteData.positionY : noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(0.22f * noteData.size * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale, GetHoldLenght(curTime),
                0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale);

            var visable = GetNoteVisable(curTime);
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;

            if (isOverStartTime)
            {
                noteRenderer1.enabled = false;
            }

            if (visable == false && !isJudge && !isHolding && judgeLine.levelController.time + GameInformation.Instance.noteToLargeTime < noteData.startTime.curTime)
            {
                judgeLine.AddNote(noteData, index);
                judgeLine.holdPool.Release(this);
            }
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
                if (holdEffectTimer > (60 / judgeLine.bpm) / 2 && !noteData.isFake)
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
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(0.22f * noteData.size * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale, GetHoldLenght(curTime),
                0.22f * GameInformation.Instance.noteScale * GameInformation.Instance.screenRadioScale);
            var visable = GetNoteVisable(curTime);
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;
        }

        public override void GetNoteData()
        {
            holdEffectTimer = 0.0f;
            isJudge = false;
            isFirstJudge = true;
            isHolding = false;
            overJudge = false;

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
            noteRenderer.color = Utils.IntToColor(noteData.color);
            noteRenderer1.color = Utils.IntToColor(noteData.color);
            noteRenderer2.color = Utils.IntToColor(noteData.color);
            hitFxColor = Utils.IntToColor(noteData.hitFXColor);
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
