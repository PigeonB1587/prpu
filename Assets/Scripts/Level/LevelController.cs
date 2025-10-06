using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace PigeonB1587.prpu
{
    public class LevelController : MonoBehaviour
    {
        public Reader reader;
        public JudgeLineController lineController;
        public SpriteRenderer previewLineRenderer;
        public HitEffectController hitFxController;
        public StoryBoardController storyBoardController;
        public Animator gui, levelAni;

        public AudioSource musicPlayer;

        public Text musicNameText, levelText, comboText, subComboText, scoreText;
        public Image backgroundImage;
        public Button pauseBtn;

        public RectTransform progressBarRect;
        public RectTransform guiRect;

        public Color perfectLine;
        public Color goodLine;
        public Color defaultLine;

        public GameObject gameObjects;

        public double time = 0d;

        public bool isLoading = true;
        public bool isPlaying = false;
        public bool isOver = false;

        private CanvasGroup comboTextCanvasGroup, subComboTextCanvasGroup;

        public void Awake()
        {
            ScoreController.ResetScore();
            reader = GetComponent<Reader>();
            lineController = GetComponent<JudgeLineController>();
            hitFxController = GetComponent<HitEffectController>();
            storyBoardController = GetComponent<StoryBoardController>();
            comboTextCanvasGroup = comboText.GetComponent<CanvasGroup>();
            subComboTextCanvasGroup = subComboText.GetComponent<CanvasGroup>();
        }

        public void Start()
        {
            musicPlayer.Pause();

            gameObjects.transform.localScale = new Vector3(0, 1, 1);
            gui.speed = 0;
            levelAni.speed = 0;

            pauseBtn.interactable = false;

            if (GameInformation.Instance == null)
            {
                Debug.LogError("Cannot find the instace \"GameInformation\".");
            }

            previewLineRenderer.color = GameInformation.Instance.isFCAPIndicator ? perfectLine : defaultLine;

            time -= GameInformation.Instance.offset + reader.offset;
            musicNameText.text = GameInformation.Instance.levelStartInfo.songsName;
            levelText.text = GameInformation.Instance.levelStartInfo.songsLevel;
            backgroundImage.sprite = GameInformation.Instance.illustration;

            LevelStart().Forget();
        }

        public void SetLevelMods()
        {
            var mods = GameInformation.Instance.levelStartInfo.levelMods;
            foreach (var mod in mods)
            {
                if (mod == "DisablePause")
                {
                    pauseBtn.interactable = false;
                }
            }
        }

        public async UniTask LoadStoryBoard()
        {
            if (Reader.chart?.storyBoard == null)
            {
                storyBoardController.events = Array.Empty<StoryBoardEvent>();
                Debug.Log("The storyboard is empty, so this section will be skipped.");
                await UniTask.CompletedTask;
                return;
            }

            if (Reader.chart.storyBoard.events == null || Reader.chart.storyBoard.eventType == null)
            {
                storyBoardController.events = Array.Empty<StoryBoardEvent>();
                Debug.Log("The storyboard is empty, so this section will be skipped.");
                await UniTask.CompletedTask;
                return;
            }

            if (Reader.chart.storyBoard.events.Length != Reader.chart.storyBoard.eventType.Length)
            {
                storyBoardController.events = Array.Empty<StoryBoardEvent>();
                Debug.LogWarning("Storyboard loading failed: Mismatched length between events and type marker array. Storyboard ignored. " +
                 "Event count: " + (Reader.chart.storyBoard.events?.Length ?? 0) + ", " +
                 "Type marker count: " + (Reader.chart.storyBoard.eventType?.Length ?? 0));
                await UniTask.CompletedTask;
                return;
            }

            storyBoardController.events = new StoryBoardEvent[Reader.chart.storyBoard.events.Length];
            for (int i = 0; i < Reader.chart.storyBoard.events.Length; i++)
            {
                storyBoardController.events[i] = new StoryBoardEvent
                {
                    type = Reader.chart.storyBoard.eventType[i],
                    @event = Reader.chart.storyBoard.events[i]
                };
            }

            var eventsByType = new Dictionary<int, List<StoryBoardEvent>>();

            foreach (var storyBoardEvent in storyBoardController.events)
            {
                if (storyBoardEvent?.@event == null) continue;

                if (!eventsByType.ContainsKey(storyBoardEvent.type))
                {
                    eventsByType[storyBoardEvent.type] = new List<StoryBoardEvent>();
                }

                eventsByType[storyBoardEvent.type].Add(storyBoardEvent);
            }

            foreach (var typeEvents in eventsByType.Values)
            {
                typeEvents.Sort((a, b) => a.@event.startTime.curTime.CompareTo(b.@event.startTime.curTime));
            }

            storyBoardController.SetCachedEvents(eventsByType);

            await UniTask.CompletedTask;
            return;
        }

        public async UniTask LevelStart()
        {
            musicPlayer.clip = GameInformation.Instance.music;
            musicPlayer.volume = GameInformation.Instance.musicVolume;
            musicPlayer.pitch = GameInformation.Instance.levelSpeed;
            musicPlayer.loop = false;
            musicPlayer.time = 0;

            SetLevelMods();

            if (Reader.chart == null || Reader.chart.level != GameInformation.Instance.levelStartInfo.songsLevel || Reader.chart.songID != GameInformation.Instance.levelStartInfo.songsId)
            {
                // Phigros.Fv3ToPrpuFv2(Phigros.GetJsonToObject(GameInformation.Instance.chart.text))
                // RePhiedit.RPEToPrpuFv2(RePhiedit.GetJsonToObject(GameInformation.Instance.chart.text))
                await reader.ReadChart(RePhiedit.RPEToPrpuFv2(RePhiedit.GetJsonToObject(GameInformation.Instance.chart.text)),
                    GameInformation.Instance.levelStartInfo.songsId,
                    GameInformation.Instance.levelStartInfo.songsLevel);
            }

            await LoadStoryBoard();

            await hitFxController.LoadCustomClip();
            await lineController.SpawnJudgmentLine();

            isLoading = false;

            gui.speed = 1;
            gui.Play("LevelStart");
            gui.Update(0f);

            levelAni.speed = 1;
            levelAni.Play("Start");
            levelAni.Update(0f);

            while (true)
            {
                if (levelAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    break;
                }
                await UniTask.Yield();
            }
            await UniTask.Yield();

            pauseBtn.interactable = true;

            gameObjects.transform.localScale = new Vector3(1, 1, 1);
            previewLineRenderer.gameObject.SetActive(false);
            
            gui.enabled = false;
            musicPlayer.Play();
            isPlaying = true;
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

        public void Pause()
        {
            musicPlayer.Pause();
        }

        public IEnumerator LevelUpdate()
        {
            while (musicPlayer.time < musicPlayer.clip.length - 0.22049f)
            {
                if (isPlaying)
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
            isPlaying = false;
            isOver = true;
            if (ScoreController.combo < 3)
            {
                comboText.gameObject.SetActive(false);
                subComboText.gameObject.SetActive(false);
            }

            gui.enabled = true;
            levelAni.enabled = false;
            gameObjects.SetActive(false);
            gui.Play("LevelEnd");

            ScoreController.CheckMissCount();
            Debug.Log($"Combo: {ScoreController.combo}, Score: {ScoreController.score}, Max Combo: {ScoreController.maxCombo}\n" +
              $"Perfect: {ScoreController.perfectCount}, Good: {ScoreController.goodCount}, Bad: {ScoreController.badCount}, Miss: {ScoreController.missCount}");
        }
    }
}