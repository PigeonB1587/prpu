using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

            obj.judgeLineList = new Prpu.Chart.JudgeLine[chartObject.judgeLineList.Length];
            for (int i = 0; i < chartObject.judgeLineList.Length; i++)
            {
                obj.judgeLineList[i] = new Prpu.Chart.JudgeLine
                {
                    bpms = new Prpu.Chart.BpmItems[1]
                    {
                        new Prpu.Chart.BpmItems
                        {
                            time = new int[3] { 0, 0, 1 },
                            bpm = chartObject.judgeLineList[i].bpm
                        }
                    },
                    noteControls = null,
                    transform = new Prpu.Chart.Transform
                    {
                        judgeLineColorEvents = null,
                        judgeLineTextEvents = null,
                        judgeLineTextureSize = new float[2] { 57.6f, 0.075f },
                        fatherLineIndex = -1,
                        anchor = new float[2] { 0.5f, 0.5f },
                        localPositionMode = true,
                        localEulerAnglesMode = true,
                        zOrder = 0,
                        judgeLineTextureScaleXEvents = null,
                        judgeLineTextureScaleYEvents = null
                    },
                    notes = new Prpu.Chart.Note[chartObject.judgeLineList[i].notesAbove.Length +
                    chartObject.judgeLineList[i].notesBelow.Length],
                    speedEvents = new Prpu.Chart.JudgeLineEvent[chartObject.judgeLineList[i].speedEvents.Length],
                    judgeLineEventLayers = new Prpu.Chart.JudgeLineEventLayer[1]
                    {
                        new Prpu.Chart.JudgeLineEventLayer
                        {
                            judgeLineMoveXEvents = new Prpu.Chart.JudgeLineEvent[chartObject.judgeLineList[i].judgeLineMoveEvents.Length],
                            judgeLineMoveYEvents = new Prpu.Chart.JudgeLineEvent[chartObject.judgeLineList[i].judgeLineMoveEvents.Length],
                            judgeLineDisappearEvents = new Prpu.Chart.JudgeLineEvent[chartObject.judgeLineList[i].judgeLineDisappearEvents.Length],
                            judgeLineRotateEvents = new Prpu.Chart.JudgeLineEvent[chartObject.judgeLineList[i].judgeLineRotateEvents.Length]
                        }
                    },

                };

                double GetBeatTime(int[] k)
                {
                    return k[0] + k[1] / (double)k[2];
                };

                List<Prpu.Chart.Note> _notes = new();
                if(chartObject.judgeLineList[i].notesAbove != null && chartObject.judgeLineList[i].notesAbove.Length != 0)
                {
                    for (int j = 0; j < chartObject.judgeLineList[i].notesAbove.Length; j++)
                    {
                        _notes.Add(new Prpu.Chart.Note
                        {
                            type = chartObject.judgeLineList[i].notesAbove[j].type,
                            isFake = false,
                            above = true,
                            startTime = CTMF(new int[2] { chartObject.judgeLineList[i].notesAbove[j].time, 32 }),
                            visibleTime = Array.Empty<int>(),
                            speed = chartObject.judgeLineList[i].notesAbove[j].speed,
                            size = 1,
                            endTime = CTMF(new int[2] { chartObject.judgeLineList[i].notesAbove[j].time
                        + chartObject.judgeLineList[i].notesAbove[j].holdTime, 32 }),
                            positionX = chartObject.judgeLineList[i].notesAbove[j].positionX,
                            positionY = 0,
                            color = -1,
                            hitFXColor = -1,
                            judgeSize = 1
                        });
                    }
                }

                if (chartObject.judgeLineList[i].notesBelow != null && chartObject.judgeLineList[i].notesBelow.Length != 0)
                {
                    for (int j = 0; j < chartObject.judgeLineList[i].notesBelow.Length; j++)
                    {
                        _notes.Add(new Prpu.Chart.Note
                        {
                            type = chartObject.judgeLineList[i].notesBelow[j].type,
                            isFake = false,
                            above = false,
                            startTime = CTMF(new int[2] { chartObject.judgeLineList[i].notesBelow[j].time, 32 }),
                            visibleTime = Array.Empty<int>(),
                            speed = chartObject.judgeLineList[i].notesBelow[j].speed,
                            size = 1,
                            endTime = CTMF(new int[2] { chartObject.judgeLineList[i].notesBelow[j].time
                        + chartObject.judgeLineList[i].notesBelow[j].holdTime, 32 }),
                            positionX = chartObject.judgeLineList[i].notesBelow[j].positionX,
                            positionY = 0,
                            color = -1,
                            hitFXColor = -1,
                            judgeSize = 1
                        });
                    }
                }
                _notes = _notes.OrderBy(note => GetBeatTime(note.startTime)).ToList();

                obj.judgeLineList[i].notes = _notes.ToArray();

                for (int k = 0; k < chartObject.judgeLineList[i].speedEvents.Length; k++)
                {
                    obj.judgeLineList[i].speedEvents[k] = new Prpu.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new int[2] { chartObject.judgeLineList[i].speedEvents[k].startTime, 32 }),
                        endTime = CTMF(new int[2] { chartObject.judgeLineList[i].speedEvents[k].endTime, 32 }),
                        start = chartObject.judgeLineList[i].speedEvents[k].value * (float)speedScale,
                        end = chartObject.judgeLineList[i].speedEvents[k].value * (float)speedScale,
                        easing = 1,
                        easingLeft = 0,
                        easingRight = 1,
                        bezierPoints = Array.Empty<float>()
                    };
                }

                obj.judgeLineList[i].speedEvents = obj.judgeLineList[i].speedEvents
     .OrderBy(s => GetBeatTime(s.startTime))
     .ToArray();

                for (int k = 0; k < chartObject.judgeLineList[i].judgeLineMoveEvents.Length; k++)
                {
                    obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveXEvents[k] = new Prpu.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineMoveEvents[k].startTime, 32 }),
                        endTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineMoveEvents[k].endTime, 32 }),
                        start = chartObject.judgeLineList[i].judgeLineMoveEvents[k].start - 0.5f,
                        end = chartObject.judgeLineList[i].judgeLineMoveEvents[k].end - 0.5f,
                        easing = 1,
                        easingLeft = 0,
                        easingRight = 1,
                        bezierPoints = Array.Empty<float>()
                    };
                }

                obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveXEvents = obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveXEvents
     .OrderBy(s => GetBeatTime(s.startTime))
     .ToArray();

                for (int k = 0; k < chartObject.judgeLineList[i].judgeLineMoveEvents.Length; k++)
                {
                    obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveYEvents[k] = new Prpu.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineMoveEvents[k].startTime, 32 }),
                        endTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineMoveEvents[k].endTime, 32 }),
                        start = chartObject.judgeLineList[i].judgeLineMoveEvents[k].start2.Value - 0.5f,
                        end = chartObject.judgeLineList[i].judgeLineMoveEvents[k].end2.Value - 0.5f,
                        easing = 1,
                        easingLeft = 0,
                        easingRight = 1,
                        bezierPoints = Array.Empty<float>()
                    };
                }

                obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveYEvents = obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineMoveYEvents
     .OrderBy(s => GetBeatTime(s.startTime))
     .ToArray();

                for (int k = 0; k < chartObject.judgeLineList[i].judgeLineRotateEvents.Length; k++)
                {
                    obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineRotateEvents[k] = new Prpu.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineRotateEvents[k].startTime, 32 }),
                        endTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineRotateEvents[k].endTime, 32}),
                        start = chartObject.judgeLineList[i].judgeLineRotateEvents[k].start,
                        end = chartObject.judgeLineList[i].judgeLineRotateEvents[k].end,
                        easing = 1,
                        easingLeft = 0,
                        easingRight = 1,
                        bezierPoints = Array.Empty<float>()
                    };
                }

                obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineRotateEvents = obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineRotateEvents
     .OrderBy(s => GetBeatTime(s.startTime))
     .ToArray();

                for (int k = 0; k < chartObject.judgeLineList[i].judgeLineDisappearEvents.Length; k++)
                {
                    obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineDisappearEvents[k] = new Prpu.Chart.JudgeLineEvent
                    {
                        startTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineDisappearEvents[k].startTime, 32 }),
                        endTime = CTMF(new int[2] { chartObject.judgeLineList[i].judgeLineDisappearEvents[k].endTime, 32 }),
                        start = chartObject.judgeLineList[i].judgeLineDisappearEvents[k].start,
                        end = chartObject.judgeLineList[i].judgeLineDisappearEvents[k].end,
                        easing = 1,
                        easingLeft = 0,
                        easingRight = 1,
                        bezierPoints = Array.Empty<float>()
                    };
                }

                obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineDisappearEvents = obj.judgeLineList[i].judgeLineEventLayers[0].judgeLineDisappearEvents
     .OrderBy(s => GetBeatTime(s.startTime))
     .ToArray();
            }

            return obj;
        }

        public const double speedScale = 6d;
        public static int[] CTMF(int[] f)
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
        private static int GCD(int a, int b)
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
