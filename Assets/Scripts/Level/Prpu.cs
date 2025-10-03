using Newtonsoft.Json;
using System;

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
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
                public float easingLeft { get; set; }
                public float easingRight { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float[] bezierPoints { get; set; }
            }
            [Serializable]
            public class TextEvent
            {
                public int[] startTime { get; set; }
                public int[] endTime { get; set; }
                public string start { get; set; }
                public string end { get; set; }
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
                public int[] startTime { get; set; }
                public int[] visibleTime { get; set; }
                public float speed { get; set; }
                public float size { get; set; }
                public int[] endTime { get; set; }
                public float positionX { get; set; }
                public float positionY { get; set; }
                public int color { get; set; }
                public int hitFXColor { get; set; }
                public float judgeSize { get; set; }
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
                public TextEvent[] judgeLineTextEvents { get; set; }
                public int fatherLineIndex { get; set; }
                public bool localPositionMode { get; set; }
                public bool localEulerAnglesMode { get; set; }
                public int zOrder { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public JudgeLineEvent[] judgeLineTextureScaleXEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public JudgeLineEvent[] judgeLineTextureScaleYEvents { get; set; }
            }

            [Serializable]
            public class NoteControl
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public ControlItem[] disappearControls { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public ControlItem[] rotateControls { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public ControlItem[] sizeControl { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public ControlItem[] xPosControl { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public NoteControl noteControls { get; set; }
                public JudgeLineEvent[] speedEvents { get; set; }
                public JudgeLineEventLayer[] judgeLineEventLayers { get; set; }
                public Transform transform { get; set; }
            }
        }
    }
}
