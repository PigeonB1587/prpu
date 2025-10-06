using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    public class Pause : MonoBehaviour
    {
        public LevelController controller;
        public InterfaceHitSound hitSound;
        public Animator ringAnimator;

        public float waitTime = 2f;
        public float timer = 0f;
        private Coroutine pauseCoroutine;

        public void Start()
        {
            ringAnimator.speed = 0;
        }

        public void TryToPause()
        {
            if (timer == 0f)
            {
                ringAnimator.speed = 1;
                ringAnimator.Play("Pause", 0, 0);
                pauseCoroutine = StartCoroutine(StartWaitPause());
            }
            else if (timer < waitTime && timer != 0f)
            {
                Debug.Log("Pause");
                ringAnimator.speed = 0;
                ringAnimator.Play("Pause", 0, 0);
                hitSound.PlaySound();
                timer = 0f;
                if (pauseCoroutine != null)
                {
                    StopCoroutine(pauseCoroutine);
                    pauseCoroutine = null;
                }
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
                    ringAnimator.speed = 0;
                    ringAnimator.Play("Pause", 0, 0);
                    pauseCoroutine = null;
                    break;
                }
                yield return null;
            }
        }
    }
}