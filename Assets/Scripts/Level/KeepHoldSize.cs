using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class KeepHoldSize : MonoBehaviour
    {
        public Transform parentTr;

        private Hold hold;

        void OnEnable()
        {
            parentTr ??= transform.parent;
            hold ??= parentTr.GetComponent<Hold>();
            UpdateScale();
        }

        void OnValidate() => UpdateScale();
        void OnDrawGizmos() => UpdateScale();
        void LateUpdate() => UpdateScale();

        void UpdateScale()
        {
            if (parentTr == null) return;

            var pScale = parentTr.lossyScale;
            if (Mathf.Abs(pScale.y) < 0.001f) return;

            transform.localScale = new Vector3(1f, 11f / (50f * pScale.y) * (GameInformation.Instance != null ? GameInformation.Instance.screenRadioScale : 1f), 1f);
        }
    }
}