using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    [RequireComponent(typeof(AudioSource))]
    public class InterfaceHitSound : MonoBehaviour
    {
        public AudioSource audioSource;

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound()
        {
            audioSource.volume = GameInformation.Instance.soundEffectVolume;
            audioSource.Play();
        }
    }
}
