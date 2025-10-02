using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Drag : NoteObject
    {
        public override void Judge()
        {
            isJudge = true;
            judgeLine.levelController.hitFxController.GetHitFx(HitEffectType.Perfect,
                judgeLine.transform.TransformPoint(new Vector3(transform.localPosition.x, 0, 0)));
            judgeLine.dragPool.Release(this);
        }
    }
}
