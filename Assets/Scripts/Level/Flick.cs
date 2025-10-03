using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Flick : NoteObject
    {
        public override void Judge()
        {
            isJudge = true;
			ScoreController.Hit(HitType.Perfect, 0);
			judgeLine.levelController.hitFxController.GetHitFx(HitType.Perfect,
                judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)),
                4);
            judgeLine.flickPool.Release(this);
        }
    }
}
