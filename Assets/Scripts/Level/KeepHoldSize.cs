using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    [ExecuteInEditMode]
    public class KeepHoldSize : MonoBehaviour
    {
        public Transform parentTr;

        void OnEnable()
        {
            parentTr ??= transform.parent;
            UpdateScale();
        }

        void OnValidate() => UpdateScale();
        void OnDrawGizmos() => UpdateScale();
        void Update() => UpdateScale();

        void UpdateScale()
        {
            if (parentTr == null) return;

            var pScale = parentTr.lossyScale;
            if (Mathf.Abs(pScale.y) < 0.001f) return;

            transform.localScale = new Vector3(1f, 11f / (50f * pScale.y), 1f);
        }
    }
}