using UnityEngine;

namespace PigeonB1587.prpu
{
    [ExecuteInEditMode]
    public class TextObject : MonoBehaviour
    {
        public TextMesh text;
        public string startText, endText;
        public float progress = 0;

        private bool isFirstCharSame;
        private bool isStartTextEmpty;
        private bool isEndTextEmpty;

        public void Awake()
        {
            text = GetComponent<TextMesh>();
        }

        public void Update()
        {
            isStartTextEmpty = string.IsNullOrEmpty(startText);
            isEndTextEmpty = string.IsNullOrEmpty(endText);

            if (!isStartTextEmpty && !isEndTextEmpty)
            {
                isFirstCharSame = startText[0] == endText[0];
            }
            else
            {
                isFirstCharSame = false;
            }
            UpdateTextDisplay();
        }

        private void UpdateTextDisplay()
        {
            progress = Mathf.Clamp01(progress);

            if (isStartTextEmpty || isEndTextEmpty)
            {
                if (isStartTextEmpty && !isEndTextEmpty)
                {
                    int totalCharacters = endText.Length;
                    int displayCharacters = Mathf.CeilToInt(totalCharacters * progress);
                    displayCharacters = Mathf.Clamp(displayCharacters, 0, totalCharacters);
                    text.text = endText.Substring(0, displayCharacters);
                }
                else if (!isStartTextEmpty && isEndTextEmpty)
                {
                    int totalCharacters = startText.Length;
                    int displayCharacters = Mathf.CeilToInt(totalCharacters * (1 - progress));
                    displayCharacters = Mathf.Clamp(displayCharacters, 0, totalCharacters);
                    text.text = startText.Substring(0, displayCharacters);
                }
                else
                {
                    text.text = "";
                }
            }
            else if (!isFirstCharSame)
            {
                text.text = progress >= 1 ? endText : startText;
            }
            else
            {
                if (progress <= 0)
                {
                    text.text = startText;
                }
                else if (progress >= 1)
                {
                    text.text = endText;
                }
                else
                {
                    int totalCharacters = endText.Length;
                    int displayCharacters = Mathf.CeilToInt(totalCharacters * progress);
                    displayCharacters = Mathf.Clamp(displayCharacters, 0, totalCharacters);
                    text.text = endText.Substring(0, displayCharacters);
                }
            }
        }
    }
}
