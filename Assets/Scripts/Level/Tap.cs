using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Tap : NoteObject
    {
        public override void Update()
        {
            base.Update();
			if (noteRenderer.enabled == false && judgeLine.levelController.time + GameInformation.Instance.noteToLargeTime < noteData.startTime.curTime)
			{
                judgeLine.localNotes.Add((noteData, index));
				judgeLine.tapPool.Release(this);
			}
		}
        public override void Judge(double curTime)
        {
            if (!noteData.isFake && curTime >= noteData.startTime.curTime)
            {
                isJudge = true;
                ScoreController.Hit(HitType.Perfect, 0);
                judgeLine.levelController.hitFxController.GetHitFx(HitType.Perfect,
                    judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)),
                    1, lineIndex: judgeLine.index, noteIndex: index, hitFxColor: noteData.hitFXColor != -1 ? hitFxColor : null);
                judgeLine.tapPool.Release(this);
            }
            else if(noteData.isFake && curTime >= noteData.startTime.curTime)
            {
                isJudge = true;
                judgeLine.tapPool.Release(this);
            }
        }
    }
}
