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
            judgeLine.flickPool.Release(this);
        }
    }
}
