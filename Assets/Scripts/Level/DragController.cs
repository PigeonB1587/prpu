using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class DragController : TapController
    {
        public override void Judge()
        {
            judgeLine.notePool.dragPool.Release(this);
        }
    }
}