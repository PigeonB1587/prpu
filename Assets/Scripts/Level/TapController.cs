using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class TapController : MonoBehaviour
    {
        public ChartObject.Note noteData;
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
            if(noteData.isHL)
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
            transform.localPosition = new Vector2(noteData.positionX, noteData.above ? GetFloorPosY() : -GetFloorPosY());
        }

        public virtual void Judge()
        {
            judgeLine.notePool.tapPool.Release(this);
        }

        public virtual float GetFloorPosY() => (noteData.floorPosition - judgeLine.floorPosition + noteData.positionY) * noteData.speed;
    }
}
