using UnityEngine;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    [ExecuteInEditMode]
    public class BackgroundCanvasScale : MonoBehaviour
    {
        public CanvasScaler canvasScaler;
        void Awake() => Set();
        void Start() => Set();
        void Update() => Set();
        void Set()
        {
            if (GameInformation.Instance != null)
            {
                canvasScaler.matchWidthOrHeight =
                GameInformation.Instance.screenRadioScale >= 1 ?
                0f : 1f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight =
                (Screen.width * 1080f / Screen.height) / 1080f > 16f / 9f ?
                0f : 1f;
            }
        }
    }
}
