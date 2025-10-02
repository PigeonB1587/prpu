using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    public class LevelController : MonoBehaviour
    {
        public Reader reader;
        public JudgeLineController lineController;
        public HitEffectController hitFxController;

        public AudioSource musicPlayer;

        public Text musicNameText, levelText;
        public Image backgroundImage;

        public RectTransform progressBarRect;
        public RectTransform guiRect;


        public double time = 0d;

        public bool isLoading = true;
        public bool isPlay = false;

        public void Awake()
        {
            reader = GetComponent<Reader>();
            lineController = GetComponent<JudgeLineController>();
            hitFxController = GetComponent<HitEffectController>();
        }

        public void Start()
        {
            musicPlayer.Pause();
            if(GameInformation.Instance == null)
            {
                Debug.LogError("Cannot find the instace \"GameInformation\".");
            }
            time -= GameInformation.Instance.offset + reader.offset;
            musicNameText.text = GameInformation.Instance.levelStartInfo.songsName;
            levelText.text = GameInformation.Instance.levelStartInfo.songsLevel;
            backgroundImage.sprite = GameInformation.Instance.illustration;
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
            await UniTask.Delay(2000);
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
                    time = musicPlayer.time - GameInformation.Instance.offset + reader.offset;
                    var progress = (float)time / musicPlayer.clip.length;
                    var xPos = Mathf.Lerp(
                GameInformation.Instance.screenRadioScale * -960,
                GameInformation.Instance.screenRadioScale * 960,
                progress
                    );
                    progressBarRect.anchoredPosition = new Vector2(xPos, 0);
                }
                yield return null;
            }
            yield return null;
            Debug.Log("Level Over");
        }
    }
}