using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections;

namespace PigeonB1587.prpu
{
    public class LevelController : MonoBehaviour
    {
        public Reader reader;
        public JudgeLineController lineController;
        public NotePool notePool;

        public AudioSource musicPlayer;

        public double time = 0d;

        public bool isLoading = true;
        public bool isPlay = false;

        public void Awake()
        {
            reader = GetComponent<Reader>();
            lineController = GetComponent<JudgeLineController>();
        }

        public void Start()
        {
            musicPlayer.Pause();
            LevelStart().Forget();
        }

        public async UniTask LevelStart()
        {
            musicPlayer.clip = GameInformation.Instance.music;
            musicPlayer.volume = GameInformation.Instance.musicVolume;
            musicPlayer.pitch = GameInformation.Instance.levelSpeed;
            musicPlayer.loop = false;
            musicPlayer.time = 0;
            if (Reader.chart == null || Reader.chart.level != GameInformation.Instance.levelStartInfo.songsLevel || Reader.chart.songID != GameInformation.Instance.levelStartInfo.songsId)
            {
                await reader.ReadChart(Phigros.Fv3ToPrpuFv2(Phigros.GetJsonToObject(GameInformation.Instance.chart.text)),
                    GameInformation.Instance.levelStartInfo.songsId,
                    GameInformation.Instance.levelStartInfo.songsLevel);
            }
            await lineController.SpawnJudgmentLine();
            isLoading = false;
            musicPlayer.Play();
            isPlay = true;
            StartCoroutine(LevelUpdate());
            await UniTask.CompletedTask;
            return;
        }

        public IEnumerator LevelUpdate()
        {
            while (musicPlayer.time < musicPlayer.clip.length - 0.22049f)
            {
                if (isPlay)
                {
                    time = musicPlayer.time;
                }
                yield return null;
            }
            yield return null;
        }
    }
}