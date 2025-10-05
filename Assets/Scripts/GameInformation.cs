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
        public float noteToLargeTime = 30;

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

        public async UniTask<T> LoadAddressableAsset<T>(string address) where T : UnityEngine.Object
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
        public float imageX;
        public float imageY;
    }

    [Serializable]
    public class NoteAsset
    {
        public int noteIndex;
        public int judgeLineIndex;
        public string hitSoundAddressableKey;
    }

    // 以下标注 OK注释 的均已实装
    public class ChartObject
    {
        [Serializable]
        public class Root
        {
            public string songID; //OK
            public string level; //OK
            public StoryBoard storyBoard;
            public JudgeLine[] judgeLineList;
        }
        [Serializable]
        public struct Time
        {
            public double curTime; //OK
            public double beatTime; //OK
            public Time GetTime(Prpu.Chart.BpmItems[] bpmItems, int[] beatTime)
            {
                if (bpmItems == null || bpmItems.Length == 0)
                    return new Time();
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
            } //OK
            public Time GetNewTime(ChartObject.BpmItems[] bpmItems, int[] beatTime)
            {
                if (bpmItems == null || bpmItems.Length == 0)
                    return new Time();
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
            } //OK
        }
        [Serializable]
        public class StoryBoard
        {
            public int[] eventType;
            public JudgeLineEvent[] events;
        }
        [Serializable]
        public class JudgeLineEvent //OK
        {
            public Time startTime; //OK
            public Time endTime; //OK
            public float start; //OK
            public float end; //OK
            public int easing; //OK
            public float easingLeft; //OK
            public float easingRight; //OK
            public float[] bezierPoints; //OK
        }
        [Serializable]
        public class SpeedEvent //OK
        {
            public Time startTime; //OK
            public Time endTime; //OK
            public float start = 0; //OK
            public float end = 0; //OK
            public int easing; //OK
            public double floorPosition = 0; //OK
            public float easingLeft; //OK
            public float easingRight; //OK
            public float[] bezierPoints; //OK
        }
        [Serializable]
        public class TextEvent
        {
            public Time startTime;
            public Time endTime;
            public string start;
            public string end;
            public int easing;
            public float easingLeft; //OK
            public float easingRight; //OK
            public float[] bezierPoints;
        }
        [Serializable]
        public class ColorEvent //OK
        {
            public Time startTime; //OK
            public Time endTime; //OK
            public Color start; //OK
            public Color end; //OK
            public int easing; //OK
            public float easingLeft; //OK
            public float easingRight; //OK
            public float[] bezierPoints; //OK
        }

        [Serializable]
        public class JudgeLineEventLayer
        {
            public JudgeLineEvent[] judgeLineMoveXEvents; //OK
            public JudgeLineEvent[] judgeLineMoveYEvents; //OK
            public JudgeLineEvent[] judgeLineRotateEvents; //OK
            public JudgeLineEvent[] judgeLineDisappearEvents; //OK
        }

        [Serializable]
        public class Note
        {
            public int type; //OK
            public bool isFake;
            public bool above; //OK
            public Time startTime; //OK
            public int[] visibleTime; //OK
            public float speed; //OK
            public float size; //OK
            public bool isHL; //OK
            public Time endTime; //OK
            public float positionX; //OK
            public float positionY; //OK
            public int color; //OK
            public int hitFXColor; //OK
            public float judgeSize;
            public double floorPosition = 0; //OK
            public double endfloorPosition = 0; //OK
        }

        [Serializable]
        public class BpmItems //OK
        {
            public Time time; //OK
            public float bpm; //OK
        }

        [Serializable]
        public class Transform
        {
            public ColorEvent[] judgeLineColorEvents;
            public TextEvent[] judgeLineTextEvents;
            public int fatherLineIndex; //OK
            public bool localPositionMode; //OK
            public bool localEulerAnglesMode; //OK
            public int zOrder; //OK
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
            public BpmItems[] bpms; //OK
            public Note[] notes;
            public NoteControl noteControls;
            public SpeedEvent[] speedEvents; //OK
            public JudgeLineEventLayer[] judgeLineEventLayers; //OK
            public Transform transform;
        }
    }
}
