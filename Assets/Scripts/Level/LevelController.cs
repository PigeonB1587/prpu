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
        public Animator gui;

        public AudioSource musicPlayer;

        public Text musicNameText, levelText, comboText, subComboText, scoreText;
        public Image backgroundImage;

        public RectTransform progressBarRect;
        public RectTransform guiRect;


        public double time = 0d;

        public bool isLoading = true;
        public bool isPlay = false;

        private CanvasGroup comboTextCanvasGroup, subComboTextCanvasGroup;

        public void Awake()
        {
            ScoreController.ResetScore();
            gui.speed = 0;
            reader = GetComponent<Reader>();
            lineController = GetComponent<JudgeLineController>();
            hitFxController = GetComponent<HitEffectController>();
            comboTextCanvasGroup = comboText.GetComponent<CanvasGroup>();
            subComboTextCanvasGroup = subComboText.GetComponent<CanvasGroup>();
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
            gui.Play("LevelStart");
            gui.speed = 1;
            isLoading = false;
            await UniTask.Delay(2000);
            gui.enabled = false;
            musicPlayer.Play();
            isPlay = true;
            StartCoroutine(LevelUpdate());
            await UniTask.CompletedTask;
            return;
        }

        private void HideCombo()
        {
            comboTextCanvasGroup.alpha = 0;
            subComboTextCanvasGroup.alpha = 0;
        }

        private void ShowCombo()
        {
            comboTextCanvasGroup.alpha = 1;
            subComboTextCanvasGroup.alpha = 1;
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
                    scoreText.text = Utils.GetScoreText(ScoreController.score);
                    comboText.text = ScoreController.combo.ToString();
                    if(ScoreController.combo < 3)
                    {
                        HideCombo();
                    }
                    else
                    {
                        ShowCombo();
                    }

                    if(Input.GetKeyDown(KeyCode.S))
                        musicPlayer.time = musicPlayer.clip.length - 5;
                }
                yield return null;
            }
            yield return null;
            Debug.Log("Level Over");
            if (ScoreController.combo < 3)
            {
                comboText.gameObject.SetActive(false);
                subComboText.gameObject.SetActive(false);
            }
            gui.enabled = true;
            gui.Play("LevelEnd");
        }
    }
}