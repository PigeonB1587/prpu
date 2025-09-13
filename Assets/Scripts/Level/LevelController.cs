using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class LevelController : MonoBehaviour
    {
        public Reader reader;
        public JudgeLineController lineController;

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
            musicPlayer.time = 0;
            if (Reader.chart == null || Reader.chart.songID != GameInformation.Instance.levelStartInfo.songsId)
            {
                await reader.ReadChart(Phigros.Fv3ToPrpuFv2(Phigros.GetJsonToObject(GameInformation.Instance.chart.text)),
                    GameInformation.Instance.levelStartInfo.songsId);
            }
            await lineController.SpawnJudgmentLine();
            isLoading = false;
            musicPlayer.Play();
            isPlay = true;
            await UniTask.CompletedTask;
            return;
        }

        public void Update()
        {
            if (isPlay)
            {
                time = musicPlayer.time;
            }
        }
    }
}