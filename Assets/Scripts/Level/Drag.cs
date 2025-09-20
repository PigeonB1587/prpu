using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Drag : NoteObject
    {
        public override void Judge()
        {
            judgeLine.dragPool.Release(this);
        }
    }
}
