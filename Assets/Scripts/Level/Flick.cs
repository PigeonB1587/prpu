using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Flick : NoteObject
    {
		public override void Update()
		{
			base.Update();
			if (noteRenderer.enabled == false && judgeLine.levelController.time + GameInformation.Instance.noteToLargeTime < noteData.startTime.curTime)
			{
				judgeLine.localNotes.Add((noteData, index));
				judgeLine.flickPool.Release(this);
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
                    4, lineIndex: judgeLine.index, noteIndex: index, hitFxColor: noteData.hitFXColor != -1 ? hitFxColor : null);
                judgeLine.flickPool.Release(this);
            }
            else if (noteData.isFake && curTime >= noteData.startTime.curTime)
            {
                isJudge = true;
                judgeLine.flickPool.Release(this);
            }
        }
    }
}
