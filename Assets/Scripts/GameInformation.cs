using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PigeonB1587.prpu
{
    public class GameInformation : MonoBehaviour
    {
        public static GameInformation Instance { get; private set; }

        public float screenW;
        public float screenH;
        public int speedEventLerpSize = 128;

        public float[] visableX;
        public float[] visableY;

        public float screenRadioScale;

        public TextAsset chart;
        public Sprite illustration;
        public AudioClip music;

        public bool isHitFXEnabled;
        public bool isFCAPIndicator;
        public bool autoPlay;
        public float offset;
        public float levelSpeed;
        public float noteScale;
        public float hitFXVolume;
        public float soundEffectVolume;
        public float musicVolume;
        public float backgroundAlpha;
        public bool useLowResolution;


        public LevelStartInfo levelStartInfo;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            screenRadioScale = Mathf.Min((float)Screen.width / Screen.height, 16f / 9f) / (16f / 9f);
            screenW = Screen.width;
            screenH = Screen.height;
        }

        void Start()
        {
            if (useLowResolution)
            {
                Screen.SetResolution((int)(screenW / 2f), (int)(screenH / 2f), Screen.fullScreenMode);
            }
        }

        void Update()
        {
            screenRadioScale = Mathf.Min((float)Screen.width / Screen.height, 16f / 9f) / (16f / 9f);
        }

        public async UniTask LoadLevelAsset()
        {
            if (Instance.levelStartInfo == null)
                return;

            chart = await LoadAddressableAsset<TextAsset>(Instance.levelStartInfo.chartAddressableKey);
            illustration = await LoadAddressableAsset<Sprite>(Instance.levelStartInfo.illustrationKey);
            music = await LoadAddressableAsset<AudioClip>(Instance.levelStartInfo.musicAddressableKey);

            Debug.Log("Completed");

            await UniTask.CompletedTask;
            return;
        }

        async UniTask<T> LoadAddressableAsset<T>(string address) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load {address}: {handle.OperationException.Message}");
                return null;
            }
            return handle.Result;
        }
    }

    [Serializable]
    public class LevelStartInfo
    {
        public string songsId;
        public string songsName;
        public string composer;
        public string charter;
        public string illustrator;
        public string songsLevel;
        public string musicAddressableKey;
        public string chartAddressableKey;
        public string illustrationKey;
        public List<JudgeLineImage> judgeLineImages;
        public List<NoteAsset> noteAssets;
        public List<string> levelMods;
    }

    [Serializable]
    public class JudgeLineImage
    {
        public int judgeLineIndex;
        public string imageAddressableKey;
    }

    [Serializable]
    public class NoteAsset
    {
        public int noteType;
        public string hitSoundAddressableKey;
    }

    public class ChartObject
    {
        [Serializable]
        public class Root
        {
            public string songID;
            public string level;
            public StoryBoard storyBoard;
            public JudgeLine[] judgeLineList;
        }
        [Serializable]
        public struct Time
        {
            public double curTime;
            public double beatTime;
            public Time GetTime(Prpu.Chart.BpmItems[] bpmItems, int[] beatTime)
            {
                var time = new Time();
                time.beatTime = beatTime[0] + (double)beatTime[1] / (double)beatTime[2];
                double sec = 0.0;
                for (int i = 0; i < bpmItems.Length; i++)
                {
                    Prpu.Chart.BpmItems e = bpmItems[i];
                    double bpmv = e.bpm;

                    if (i != bpmItems.Length - 1)
                    {
                        double etBeat = (bpmItems[i + 1].time[0] + bpmItems[i + 1].time[1] / bpmItems[i + 1].time[2])
                            - (e.time[0] + e.time[1] / e.time[2]);
                        if (time.beatTime >= etBeat)
                        {
                            sec += etBeat * (60 / bpmv);
                            time.beatTime -= etBeat;
                        }
                        else
                        {
                            sec += time.beatTime * (60 / bpmv);
                            break;
                        }
                    }
                    else
                    {
                        sec += time.beatTime * (60 / bpmv);
                    }
                }
                time.curTime = sec;
                return time;
            }
            public Time GetTimeLast(ChartObject.BpmItems[] bpmItems, int[] beatTime)
            {
                var time = new Time();
                time.beatTime = beatTime[0] + (double)beatTime[1] / (double)beatTime[2];
                double sec = 0.0;
                for (int i = 0; i < bpmItems.Length; i++)
                {
                    ChartObject.BpmItems e = bpmItems[i];
                    double bpmv = e.bpm;

                    if (i != bpmItems.Length - 1)
                    {
                        double etBeat = bpmItems[i + 1].time.beatTime
                            - e.time.beatTime;
                        if (time.beatTime >= etBeat)
                        {
                            sec += etBeat * (60 / bpmv);
                            time.beatTime -= etBeat;
                        }
                        else
                        {
                            sec += time.beatTime * (60 / bpmv);
                            break;
                        }
                    }
                    else
                    {
                        sec += time.beatTime * (60 / bpmv);
                    }
                }
                time.curTime = sec;
                return time;
            }
        }
        [Serializable]
        public class StoryBoard
        {
            public int[] eventType;
            public JudgeLineEvent[] events;
        }
        [Serializable]
        public class JudgeLineEvent
        {
            public Time startTime;
            public Time endTime;
            public float start;
            public float end;
            public int easing;
            public float easingLeft;
            public float easingRight;
            public float[] bezierPoints;
        }
        [Serializable]
        public class SpeedEvent
        {
            public Time startTime;
            public Time endTime;
            public float start = 0;
            public float end = 0;
            public int easing;
            public float floorPosition = 0;
            public float easingLeft;
            public float easingRight;
            public float[] bezierPoints;
        }
        [Serializable]
        public class TextEvent
        {
            public Time startTime;
            public Time endTime;
            public string start;
            public string end;
            public int easing;
            public float[] easingCutting;
            public float[] bezierPoints;
        }

        [Serializable]
        public class JudgeLineEventLayer
        {
            public JudgeLineEvent[] judgeLineMoveXEvents;
            public JudgeLineEvent[] judgeLineMoveYEvents;
            public JudgeLineEvent[] judgeLineRotateEvents;
            public JudgeLineEvent[] judgeLineDisappearEvents;
        }

        [Serializable]
        public class Note
        {
            public int type;
            public bool isFake;
            public bool above;
            public Time startTime;
            public int[] visibleTime;
            public float speed;
            public float size;
            public bool isHL;
            public Time endTime;
            public float positionX;
            public float positionY;
            public int color;
            public bool autoPlayHitSound;
            public int hitFXColor;
            public float judgeSize;
            public float floorPosition = 0;
            public float endfloorPosition = 0;
        }

        [Serializable]
        public class BpmItems
        {
            public Time time;
            public float bpm;
        }

        [Serializable]
        public class Transform
        {
            public JudgeLineEvent[] judgeLineColorEvents;
            public TextEvent[] judgeLineTextEvents;
            public float[] judgeLineTextureSize;
            public int fatherLineIndex;
            public float[] anchor;
            public bool localPositionMode;
            public bool localEulerAnglesMode;
            public int zOrder;
            public JudgeLineEvent[] judgeLineTextureScaleXEvents;
            public JudgeLineEvent[] judgeLineTextureScaleYEvents;
        }

        [Serializable]
        public class NoteControl
        {
            public ControlItem[] disappearControls;
            public ControlItem[] rotateControls;
            public ControlItem[] sizeControl;
            public ControlItem[] xPosControl;
            public ControlItem[] yPosControl;
        }

        [Serializable]
        public class ControlItem
        {
            public int easing;
            public float value;
            public float x;
        }

        [Serializable]
        public class JudgeLine
        {
            public BpmItems[] bpms;
            public Note[] notes;
            public NoteControl noteControls;
            public SpeedEvent[] speedEvents;
            public JudgeLineEventLayer[] judgeLineEventLayers;
            public Transform transform;
        }
    }
}
