using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    public class Pause : MonoBehaviour
    {
        public LevelController controller;

        public float waitTime = 2f;
        public float timer = 0f;
        public void Click()
        {
            if (timer == 0f)
            {
                StartCoroutine(StartWaitPause());
            }
            else if (timer < waitTime)
            {
                Debug.Log("Pause");
            }
        }

        IEnumerator StartWaitPause()
        {
            while (true)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    Debug.Log("Pause Cancel");
                    timer = 0f;
                    break;
                }
                yield return null;
            }
        }
    }
}