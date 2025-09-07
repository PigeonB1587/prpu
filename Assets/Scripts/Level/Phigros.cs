using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Phigros
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);

        /// <summary>
        /// Convert "formatVersion = 3" chart to "formatVersion = 1"
        /// </summary>
        /// <param name="chartObject">Any "formatVersion = 3" chart</param>
        /// <returns>"formatVersion = 1" chart</returns>
        public static Chart.Root Fv3ToFv1(Chart.Root chartObject)
        {
            var obj = new Chart.Root
            {
                formatVersion = 1,
                offset = chartObject.offset,
                numOfNotes = chartObject.numOfNotes,
                judgeLineList = new Chart.JudgeLine[chartObject.judgeLineList.Length]
            };
            for (int i = 0; i < chartObject.judgeLineList.Length; i++)
            {
                var sourceLine = chartObject.judgeLineList[i];
                var targetLine = obj.judgeLineList[i] = new Chart.JudgeLine
                {
                    bpm = sourceLine.bpm,
                    notesAbove = sourceLine.notesAbove,
                    notesBelow = sourceLine.notesBelow,
                    numOfNotesAbove = sourceLine.numOfNotesAbove,
                    numOfNotesBelow = sourceLine.numOfNotesBelow,
                    speedEvents = new Chart.SpeedEvent[sourceLine.speedEvents.Length],
                    judgeLineMoveEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineMoveEvents.Length],
                    judgeLineDisappearEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineDisappearEvents.Length],
                    judgeLineRotateEvents = new Chart.JudgeLineEvent[sourceLine.judgeLineRotateEvents.Length]
                };
                for (int j = 0; j < sourceLine.speedEvents.Length; j++)
                {
                    var se = sourceLine.speedEvents[j];
                    targetLine.speedEvents[j] = new Chart.SpeedEvent
                    {
                        startTime = se.startTime,
                        endTime = se.endTime,
                        value = se.value,
                        floorPosition = null
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineMoveEvents.Length; j++)
                {
                    var me = sourceLine.judgeLineMoveEvents[j];
                    targetLine.judgeLineMoveEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = me.startTime,
                        endTime = me.endTime,
                        start = ConvertValue3ToValue1(me.start, me.start2.Value),
                        end = ConvertValue3ToValue1(me.end, me.end2.Value),
                        start2 = null,
                        end2 = null
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineDisappearEvents.Length; j++)
                {
                    var de = sourceLine.judgeLineDisappearEvents[j];
                    targetLine.judgeLineDisappearEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = de.startTime,
                        endTime = de.endTime,
                        start = de.start,
                        end = de.end,
                        start2 = null,
                        end2 = null
                    };
                }
                for (int j = 0; j < sourceLine.judgeLineRotateEvents.Length; j++)
                {
                    var re = sourceLine.judgeLineRotateEvents[j];
                    targetLine.judgeLineRotateEvents[j] = new Chart.JudgeLineEvent
                    {
                        startTime = re.startTime,
                        endTime = re.endTime,
                        start = re.start,
                        end = re.end,
                        start2 = null,
                        end2 = null
                    };
                }
            }
            return obj;
        }

        /*
        public static Chart.Root Fv1ToFv3(Chart.Root chartObject)
        {
            var obj = new Chart.Root();
            return obj;
        }

        public static Chart.Root Pgr250ToPgr100(Chart.Root chartObject)
        {
            var obj = new Chart.Root();
            return obj;
        }

        public static Chart.Root Pgr100ToPgr250(Chart.Root chartObject)
        {
            var obj = new Chart.Root();
            return obj;
        }
        */

        public static Prpu.Chart.Root Fv3ToPrpuFv2(Chart.Root chartObject)
        {
            var obj = new Prpu.Chart.Root()
            {
                formatVersion = 2,
                offset = chartObject.offset,
                storyBoard = null,
            };


            return obj;
        }

        public const double speedScale = 6d;
        public int[] CTMF(int[] f)
        {
            if (f[0] == 0) return new[] { 0, 0, 1 };

            int g = GCD(Math.Abs(f[0]), Math.Abs(f[1]));
            int n = f[0] / g, d = f[1] / g;

            if (d < 0) { n = -n; d = -d; }

            int i = n / d, r = n % d;
            if (n < 0 && r != 0) i--;

            int num = n - i * d;
            return new[] { i, num, num == 0 ? 1 : d };
        }
        private int GCD(int a, int b)
        {
            while (b != 0) (a, b) = (b, a % b);
            return a;
        }
        public static float ConvertValue3ToValue1(float x3, float y3) => 1000 * x3 * 880 + y3 * 520;
        public static (float x3, float y3) ConvertValue1ToValue3(float value1) => ((float)Math.Truncate(value1) / 880000f, value1 % 880000f);

        public class Chart
        {
            [Serializable]
            public class Root
            {
                public int formatVersion { get; set; }
                public float offset { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotes { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }

            [Serializable]
            public class JudgeLine
            {
                public float bpm { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotes { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotesAbove { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? numOfNotesBelow { get; set; }
                public Note[] notesAbove { get; set; }
                public Note[] notesBelow { get; set; }
                public SpeedEvent[] speedEvents { get; set; }
                public JudgeLineEvent[] judgeLineDisappearEvents { get; set; }
                public JudgeLineEvent[] judgeLineMoveEvents { get; set; }
                public JudgeLineEvent[] judgeLineRotateEvents { get; set; }
            }

            [Serializable]
            public class Note
            {
                public int type { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int time { get; set; }
                public float positionX { get; set; }

                [JsonConverter(typeof(FloatToIntConverter))]
                public int holdTime { get; set; }
                public float speed { get; set; }
                public float floorPosition { get; set; }
            }

            [Serializable]
            public class SpeedEvent
            {
                [JsonConverter(typeof(FloatToIntConverter))]
                public int startTime { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int endTime { get; set; }
                public float value { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? floorPosition { get; set; }
            }

            [Serializable]
            public class JudgeLineEvent
            {
                [JsonConverter(typeof(FloatToIntConverter))]
                public int startTime { get; set; }
                [JsonConverter(typeof(FloatToIntConverter))]
                public int endTime { get; set; }
                public float start { get; set; }
                public float end { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? start2 { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float? end2 { get; set; }
            }
        }
    }

    public class FloatToIntConverter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is double d)
                return (int)Math.Round(d);
            if (reader.Value is float f)
                return (int)Math.Round(f);
            if (reader.Value is long l)
                return (int)l;
            return 0;
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
