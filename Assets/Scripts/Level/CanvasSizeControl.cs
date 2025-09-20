using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    [ExecuteInEditMode]
    public class CanvasSizeControl : MonoBehaviour
    {
        public RectTransform rectTransform;

        void Start() => UpdateSizeDelta();
        void Update() => UpdateSizeDelta();
        void UpdateSizeDelta()
        {
            float scaledWidth = Screen.width * 1080f / Screen.height;
            rectTransform.sizeDelta = (Screen.width * 1080f / Screen.height) / 1080f > 16f / 9f
                ? new Vector2(1920, 1080)
                : new Vector2(scaledWidth, 1080);
        }
    }
}
