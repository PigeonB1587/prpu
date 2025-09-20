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
            judgeLine.dragPool.Release(this);
        }
    }
}
