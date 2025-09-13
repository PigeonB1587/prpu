using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PigeonB1587.prpu
{
    public class LevelController : MonoBehaviour
    {
        public Reader reader;

        public AudioSource musicPlayer;

        public void Awake()
        {
            reader = GetComponent<Reader>();
        }

        public void Start()
        {
            
        }

        public async UniTask LevelStart()
        {
            await UniTask.CompletedTask;
            return;
        }
    }
}