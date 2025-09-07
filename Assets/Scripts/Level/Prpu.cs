using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public static class Prpu
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);
        public class Chart
        {
            [Serializable]
            public class Root
            {
                public int formatVersion { get; set; }
                public float offset { get; set; }
                public StoryBoard storyBoard { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }
            [Serializable]
            public class StoryBoard
            {
                public int[] eventType { get; set; }
                public JudgeLineEvent[] events { get; set; }
            }
            [Serializable]
            public class JudgeLineEvent
            {
                public int[] startTime { get; set; }
                public int[] endTime { get; set; }
                public float start { get; set; }
                public float end { get; set; }
                public int easing { get; set; }
                public float[] easingCutting { get; set; }

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float[] bezierPoints { get; set; }
            }

            [Serializable]
            public class JudgeLineEventLayer
            {
                public JudgeLineEvent[] judgeLineMoveXEvents { get; set; }
                public JudgeLineEvent[] judgeLineMoveYEvents { get; set; }
                public JudgeLineEvent[] judgeLineRotateEvents { get; set; }
                public JudgeLineEvent[] judgeLineDisappearEvents { get; set; }
            }

            [Serializable]
            public class Note
            {
                public int type { get; set; }
                public bool isFake { get; set; }
                public bool above { get; set; }
                public int[] time { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float visibleTime { get; set; }
                public float speed { get; set; }
                public float size { get; set; }
                public int[] holdTime { get; set; }
                public float positionX { get; set; }
                public float positionY { get; set; }
                public int color { get; set; }
            }

            [Serializable]
            public class BpmItems
            {
                public int[] time { get; set; }
                public float bpm { get; set; }
            }

            [Serializable]
            public class Transform
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public JudgeLineEvent[] judgeLineColorEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public JudgeLineEvent[] judgeLineScaleXEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public JudgeLineEvent[] judgeLineScaleYEvents { get; set; }
            }

            [Serializable]
            public class NoteControl
            {
                public ControlItem[] disappearControls { get; set; }
                public ControlItem[] sizeControl { get; set; }
                public ControlItem[] xPosControl { get; set; }
                public ControlItem[] yPosControl { get; set; }
            }

            [Serializable]
            public class ControlItem
            {
                public int easing { get; set; }
                public float value { get; set; }
                public float x { get; set; }
            }

            [Serializable]
            public class JudgeLine
            {
                public BpmItems[] bpms { get; set; }
                public Note[] notes { get; set; }
                public int fatherLineIndex { get; set; }
                public bool inheritanceAngle { get; set; }
                public int zOrder { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public NoteControl[] noteControls { get; set; }
                public JudgeLineEvent[] speedEvents { get; set; }
                public JudgeLineEventLayer[] judgeLineEventLayers { get; set; }
                public Transform transform { get; set; }
            }
        }
    }
}
