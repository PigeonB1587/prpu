using System;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class GameInformation : MonoBehaviour
    {
        public static GameInformation Instance { get; private set; }

        public float screenW;
        public float screenH;

        public bool isHitFXEnabled;
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
        public string songsDifficulty;
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
        public float xSize;
        public float ySize;
    }

    [Serializable]
    public class NoteAsset
    {
        public int noteIndex;
        public string hitSoundAddressableKey;
        public bool autoPlayHitSound;
        public Color hitFXColor;
    }

    public class ChartObject
    {
        [Serializable]
        public struct Time
        {
            public int[] beatTime;
            public double realTime;

            public Time Set(int[] t, BpmItems[] s)
            {
                beatTime = t;
                realTime = Beat2Sec(GetBeatTime(t), s);
                return this;
            }
            public double Beat2Sec(double t, BpmItems[] BPMArray)
            {
                double sec = 0.0;
                for (int i = 0; i < BPMArray.Length; i++)
                {
                    BpmItems e = BPMArray[i];
                    double bpmv = e.bpm;

                    if (i != BPMArray.Length - 1)
                    {
                        double etBeat = (BPMArray[i + 1].time.beatTime[0] + BPMArray[i + 1].time.beatTime[1] / BPMArray[i + 1].time.beatTime[2])
                            - (e.time.beatTime[0] + e.time.beatTime[1] / e.time.beatTime[2]);
                        if (t >= etBeat)
                        {
                            sec += etBeat * (60 / bpmv);
                            t -= etBeat;
                        }
                        else
                        {
                            sec += t * (60 / bpmv);
                            break;
                        }
                    }
                    else
                    {
                        sec += t * (60 / bpmv);
                    }
                }
                return sec;
            }
            public double GetBeatTime(int[] t1) => t1[0] + t1[1] / (double)t1[2];
        }

        [Serializable]
        public class Root
        {
            public int formatVersion;
            public float offset;
            public JudgeLine[] judgeLineList;
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
        public class JudgeLineColorEvent
        {
            public Time startTime;
            public Time endTime;
            public Color start;
            public Color end;
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
            public float start;
            public float end;
            public float floorPosition;
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
            public bool isHl;
            public Time startTime;
            public float visibleTime;
            public float speed;
            public float size;
            public Time endTime;
            public float positionX;
            public float positionY;
            public float floorPosition;
            public int color;
        }

        [Serializable]
        public class BpmItems
        {
            public Time time;
            public float bpm;
        }

        [Serializable]
        public class Storyboard
        {
            public JudgeLineColorEvent[] judgeLineColorEvents;
            public JudgeLineEvent[] judgeLineScaleXEvents;
            public JudgeLineEvent[] judgeLineScaleYEvents;
        }

        [Serializable]
        public class NoteControl
        {
            public ControlItem[] disappearControls;
            public ControlItem[] sizeControl;
            public ControlItem[] xPosControl;
            public ControlItem[] yPosControl;
        }

        [Serializable]
        public struct ControlItem
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
            public int fatherLineIndex;
            public bool inheritanceAngle;
            public int zOrder;
            public NoteControl[] noteControls;
            public SpeedEvent[] speedEvents;
            public JudgeLineEventLayer[] judgeLineEventLayers;
            public Storyboard storyboard;
        }
    }
}
