using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class TextObject : MonoBehaviour
    {
        public TextMesh text;
        public string startText, endText;
        public int progress = 0;
        public JudgeLine fatherLine;

        public void Awake()
        {
            text = GetComponent<TextMesh>();
        }
        public void LateUpdate()
        {
            transform.position = fatherLine.transform.position;
        }
    }
}
