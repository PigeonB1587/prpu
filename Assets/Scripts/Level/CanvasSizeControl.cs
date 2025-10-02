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
            if (GameInformation.Instance != null)
            {
                rectTransform.sizeDelta = GameInformation.Instance.screenRadioScale >= 1
                ? new Vector2(1920, 1080)
                : new Vector2(1920 * GameInformation.Instance.screenRadioScale, 1080);
            }
            else
            {
                float scaledWidth = Screen.width * 1080f / Screen.height;
                rectTransform.sizeDelta = (Screen.width * 1080f / Screen.height) / 1080f > 16f / 9f
                ? new Vector2(1920, 1080)
                : new Vector2(scaledWidth, 1080);
            }
        }
    }
}
