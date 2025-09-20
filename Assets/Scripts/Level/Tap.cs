using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Tap : NoteObject
    {
        public override void Judge()
        {
            isJudge = true;
            judgeLine.tapPool.Release(this);
        }
    }
}
