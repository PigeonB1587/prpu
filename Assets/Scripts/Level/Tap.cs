using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Tap : NoteObject
    {
        public override void Judge()
        {
            judgeLine.tapPool.Release(this);
        }
    }
}
