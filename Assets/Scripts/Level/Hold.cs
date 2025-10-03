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
            if (!noteData.isFake && isFirstJudge && isOverStartTime)
                Judge();
            
            transform.localPosition = new Vector2(transform.localPosition.x, isOverStartTime ? noteData.positionY : noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(0.22f * noteData.size * GameInformation.Instance.noteScale, GetHoldLenght(curTime),
                0.22f * GameInformation.Instance.noteScale);

            var visable = GetNoteVisable(curTime);
            noteRenderer.enabled = visable;
            noteRenderer1.enabled = visable;
            noteRenderer2.enabled = visable;

            if (isOverStartTime)
            {
                noteRenderer1.enabled = false;
            }
        }

        public override void Judge()
        {
            isFirstJudge = false;
            isHolding = true;
            isJudge = true;
            judgeLine.levelController.hitFxController.GetHitFx(HitEffectType.Perfect,
                judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)));
            StartCoroutine(Holding());
        }

        IEnumerator Holding()
        {
            while (isHolding)
            {
                holdEffectTimer += Time.deltaTime;
                if(holdEffectTimer > (60 / judgeLine.bpm) / 2 && !noteData.isFake)
                {
                    judgeLine.levelController.hitFxController.GetHitFx(HitEffectType.Perfect,
                        judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)));
                    holdEffectTimer = 0.0f;
                }
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

        public override void ResetNote(double curTime)
        {
            GetNoteData();
            floorPosition = GetFloorPosY();
            transform.localPosition = new Vector2(noteData.positionX * GameInformation.Instance.screenRadioScale, noteData.above ? floorPosition : -floorPosition);
            transform.localScale = new Vector3(0.22f * noteData.size * GameInformation.Instance.noteScale, GetHoldLenght(curTime),
                0.22f * GameInformation.Instance.noteScale);
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

        public override bool GetNoteVisable(double curTime)
        {
            bool visable = false;
            if (Utils.GetHoldVisable(noteRenderer1.transform.position, noteRenderer2.transform.position, float.MinValue,
                -10, float.MaxValue, 10))
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

        public float GetHoldLenght(double curTime) => (curTime >= noteData.startTime.curTime 
            ? noteData.endfloorPosition - judgeLine.floorPosition : 
            noteData.endfloorPosition - noteData.floorPosition) * holdLengthScale;
    }
}
