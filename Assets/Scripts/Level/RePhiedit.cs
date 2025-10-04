using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PigeonB1587.prpu
{
    public class RePhiedit
    {
        public static Chart.Root GetJsonToObject(string chartJson) =>
            JsonConvert.DeserializeObject<Chart.Root>(chartJson);

        public static string GetObjectToJson(Chart.Root chartObject) =>
            JsonConvert.SerializeObject(chartObject);

        public static Prpu.Chart.Root RPEToPrpuFv2(Chart.Root chartObject)
        {
            var obj = new Prpu.Chart.Root()
            {
                formatVersion = 2,
                offset = chartObject.META.offset / 1000f,
                storyBoard = null,
            };

            obj.judgeLineList = new Prpu.Chart.JudgeLine[chartObject.judgeLineList.Length];

            for (int i = 0; i < chartObject.judgeLineList.Length; i++)
            {
                var sourceLine = chartObject.judgeLineList[i];

                var bpmItems = new List<Prpu.Chart.BpmItems>();
                for (int j = 0; j < chartObject.BPMList.Length; j++)
                {
                    bpmItems.Add(new Prpu.Chart.BpmItems
                    {
                        time = chartObject.BPMList[j].startTime,
                        bpm = chartObject.BPMList[j].bpm
                    });
                }

                var notes = new List<Prpu.Chart.Note>();
                if (sourceLine.notes != null)
                {
                    for (int j = 0; j < sourceLine.notes.Length; j++)
                    {
                        var sourceNote = sourceLine.notes[j];
                        notes.Add(new Prpu.Chart.Note
                        {
                            type = RpeNoteTypeToPrpu(sourceNote.type),
                            isFake = sourceNote.isFake != 0,
                            above = sourceNote.above != 0,
                            startTime = sourceNote.startTime,
                            visibleTime = Array.Empty<int>(),
                            speed = sourceNote.speed,
                            size = sourceNote.size,
                            endTime = sourceNote.endTime,
                            positionX = sourceNote.positionX * 0.013169f,
                            positionY = sourceNote.yOffset * 0.011111f,
                            color = sourceNote.color != null && sourceNote.color.Length >= 3 ?
                                   Utils.RgbaToInt((byte)sourceNote.color[0], (byte)sourceNote.color[1], (byte)sourceNote.color[2], 255) : -1,
                            hitFXColor = -1,
                            judgeSize = sourceNote.judgeArea
                        });
                    }
                }

                var eventLayers = new List<Prpu.Chart.JudgeLineEventLayer>();
                if (sourceLine.eventLayers != null)
                {
                    for (int j = 0; j < sourceLine.eventLayers.Length; j++)
                    {
                        var sourceLayer = sourceLine.eventLayers[j];
                        var targetLayer = new Prpu.Chart.JudgeLineEventLayer();

                        if (sourceLayer.moveXEvents != null)
                        {
                            targetLayer.judgeLineMoveXEvents = new Prpu.Chart.JudgeLineEvent[sourceLayer.moveXEvents.Length];
                            for (int c = 0; c < sourceLayer.moveXEvents.Length; c++)
                            {
                                var sourceEvent = sourceLayer.moveXEvents[c];
                                targetLayer.judgeLineMoveXEvents[c] = new Prpu.Chart.JudgeLineEvent
                                {
                                    startTime = sourceEvent.startTime,
                                    endTime = sourceEvent.endTime,
                                    start = sourceEvent.start / 1350f,
                                    end = sourceEvent.end / 1350f,
                                    easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                    easingLeft = sourceEvent.easingLeft,
                                    easingRight = sourceEvent.easingRight,
                                    bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                                };
                            }
                        }

                        if (sourceLayer.moveYEvents != null)
                        {
                            targetLayer.judgeLineMoveYEvents = new Prpu.Chart.JudgeLineEvent[sourceLayer.moveYEvents.Length];
                            for (int c = 0; c < sourceLayer.moveYEvents.Length; c++)
                            {
                                var sourceEvent = sourceLayer.moveYEvents[c];
                                targetLayer.judgeLineMoveYEvents[c] = new Prpu.Chart.JudgeLineEvent
                                {
                                    startTime = sourceEvent.startTime,
                                    endTime = sourceEvent.endTime,
                                    start = sourceEvent.start / 900f,
                                    end = sourceEvent.end / 900f,
                                    easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                    easingLeft = sourceEvent.easingLeft,
                                    easingRight = sourceEvent.easingRight,
                                    bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                                };
                            }
                        }

                        if (sourceLayer.rotateEvents != null)
                        {
                            targetLayer.judgeLineRotateEvents = new Prpu.Chart.JudgeLineEvent[sourceLayer.rotateEvents.Length];
                            for (int c = 0; c < sourceLayer.rotateEvents.Length; c++)
                            {
                                var sourceEvent = sourceLayer.rotateEvents[c];
                                targetLayer.judgeLineRotateEvents[c] = new Prpu.Chart.JudgeLineEvent
                                {
                                    startTime = sourceEvent.startTime,
                                    endTime = sourceEvent.endTime,
                                    start = sourceEvent.start,
                                    end = sourceEvent.end,
                                    easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                    easingLeft = sourceEvent.easingLeft,
                                    easingRight = sourceEvent.easingRight,
                                    bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                                };
                            }
                        }

                        if (sourceLayer.alphaEvents != null)
                        {
                            targetLayer.judgeLineDisappearEvents = new Prpu.Chart.JudgeLineEvent[sourceLayer.alphaEvents.Length];
                            for (int c = 0; c < sourceLayer.alphaEvents.Length; c++)
                            {
                                var sourceEvent = sourceLayer.alphaEvents[c];
                                targetLayer.judgeLineDisappearEvents[c] = new Prpu.Chart.JudgeLineEvent
                                {
                                    startTime = sourceEvent.startTime,
                                    endTime = sourceEvent.endTime,
                                    start = sourceEvent.start / 255f,
                                    end = sourceEvent.end / 255f,
                                    easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                    easingLeft = sourceEvent.easingLeft,
                                    easingRight = sourceEvent.easingRight,
                                    bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                                };
                            }
                        }

                        eventLayers.Add(targetLayer);
                    }
                }

                var speedEvents = new List<Prpu.Chart.JudgeLineEvent>();
                if (sourceLine.eventLayers != null)
                {
                    foreach (var layer in sourceLine.eventLayers)
                    {
                        if (layer.speedEvents != null)
                        {
                            foreach (var sourceEvent in layer.speedEvents)
                            {
                                speedEvents.Add(new Prpu.Chart.JudgeLineEvent
                                {
                                    startTime = sourceEvent.startTime,
                                    endTime = sourceEvent.endTime,
                                    start = sourceEvent.start * (float)speedScale,
                                    end = sourceEvent.end * (float)speedScale,
                                    easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                    easingLeft = 0,
                                    easingRight = 1,
                                    bezierPoints = Array.Empty<float>()
                                });
                            }
                        }
                    }
                }

                var colorEvents = new List<Prpu.Chart.JudgeLineEvent>();
                var scaleXEvents = new List<Prpu.Chart.JudgeLineEvent>();
                var scaleYEvents = new List<Prpu.Chart.JudgeLineEvent>();
                var textEvents = new List<Prpu.Chart.TextEvent>();

                if (sourceLine.extended != null)
                {
                    if (sourceLine.extended.colorEvents != null)
                    {
                        foreach (var sourceEvent in sourceLine.extended.colorEvents)
                        {
                            colorEvents.Add(new Prpu.Chart.JudgeLineEvent
                            {
                                startTime = sourceEvent.startTime,
                                endTime = sourceEvent.endTime,
                                start = Utils.RgbaToInt((byte)sourceEvent.start[0], (byte)sourceEvent.start[1], (byte)sourceEvent.start[2], 255),
                                end = Utils.RgbaToInt((byte)sourceEvent.end[0], (byte)sourceEvent.end[1], (byte)sourceEvent.end[2], 255),
                                easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                easingLeft = sourceEvent.easingLeft,
                                easingRight = sourceEvent.easingRight,
                                bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                            });
                        }
                    }

                    if (sourceLine.extended.scaleXEvents != null)
                    {
                        foreach (var sourceEvent in sourceLine.extended.scaleXEvents)
                        {
                            scaleXEvents.Add(new Prpu.Chart.JudgeLineEvent
                            {
                                startTime = sourceEvent.startTime,
                                endTime = sourceEvent.endTime,
                                start = sourceEvent.start,
                                end = sourceEvent.end,
                                easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                easingLeft = sourceEvent.easingLeft,
                                easingRight = sourceEvent.easingRight,
                                bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                            });
                        }
                    }

                    if (sourceLine.extended.scaleYEvents != null)
                    {
                        foreach (var sourceEvent in sourceLine.extended.scaleYEvents)
                        {
                            scaleYEvents.Add(new Prpu.Chart.JudgeLineEvent
                            {
                                startTime = sourceEvent.startTime,
                                endTime = sourceEvent.endTime,
                                start = sourceEvent.start,
                                end = sourceEvent.end,
                                easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                easingLeft = sourceEvent.easingLeft,
                                easingRight = sourceEvent.easingRight,
                                bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                            });
                        }
                    }

                    if (sourceLine.extended.textEvents != null)
                    {
                        foreach (var sourceEvent in sourceLine.extended.textEvents)
                        {
                            textEvents.Add(new Prpu.Chart.TextEvent
                            {
                                startTime = sourceEvent.startTime,
                                endTime = sourceEvent.endTime,
                                start = sourceEvent.start,
                                end = sourceEvent.end,
                                easing = RpeEasingTypeToPrpu(sourceEvent.easingType),
                                bezierPoints = sourceEvent.bezier == 1 ? sourceEvent.bezierPoints : Array.Empty<float>()
                            });
                        }
                    }
                }

                var transform = new Prpu.Chart.Transform
                {
                    judgeLineColorEvents = colorEvents.Count > 0 ? colorEvents.ToArray() : null,
                    judgeLineTextEvents = textEvents.Count > 0 ? textEvents.ToArray() : null,
                    fatherLineIndex = sourceLine.father,
                    localPositionMode = sourceLine.father != -1,
                    localEulerAnglesMode = sourceLine.rotateWithFather,
                    zOrder = sourceLine.zOrder,
                    judgeLineTextureScaleXEvents = scaleXEvents.Count > 0 ? scaleXEvents.ToArray() : null,
                    judgeLineTextureScaleYEvents = scaleYEvents.Count > 0 ? scaleYEvents.ToArray() : null
                };

                obj.judgeLineList[i] = new Prpu.Chart.JudgeLine
                {
                    bpms = bpmItems.ToArray(),
                    notes = notes.ToArray(),
                    noteControls = null,
                    speedEvents = speedEvents.ToArray(),
                    judgeLineEventLayers = eventLayers.ToArray(),
                    transform = transform
                };
            }

            return obj;
        }

        public const double speedScale = 1.3333333333333333d;

        public static int RpeNoteTypeToPrpu(int i) => i switch
        {
            1 => 1,
            2 => 3,
            3 => 4,
            4 => 2,
            _ => 1
        };

        public static int RpeEasingTypeToPrpu(int i) => i switch
        {
            1 => 1,
            2 => 3,
            3 => 2,
            4 => 6,
            5 => 5,
            6 => 4,
            7 => 7,
            8 => 9,
            9 => 8,
            10 => 12,
            11 => 11,
            12 => 10,
            13 => 13,
            14 => 15,
            15 => 14,
            16 => 18,
            17 => 17,
            18 => 21,
            19 => 20,
            20 => 24,
            21 => 23,
            22 => 22,
            23 => 25,
            24 => 27,
            25 => 26,
            26 => 30,
            27 => 29,
            28 => 31,
            29 => 28,
            _ => 1
        };

        // RPE 170 -
        public class Chart
        {
            [Serializable]
            public class BPMItem
            {
                public float bpm { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class META
            {
                public int RPEVersion { get; set; }
                public int offset { get; set; }
            }
            [Serializable]
            public class JudgeLine
            {
                public string Texture { get; set; }
                public EventLayers[] eventLayers { get; set; }
                public Extended extended { get; set; }
                public Note[] notes { get; set; }
                public bool rotateWithFather { get; set; } = false;
                public int father { get; set; }
                public int zOrder { get; set; }
            }
            [Serializable]
            public class Note
            {
                public int above { get; set; }
                public int alpha { get; set; }
                public int[] color { get; set; }
                public int[] endTime { get; set; }
                public int isFake { get; set; }
                public float positionX { get; set; }
                public float size { get; set; }
                public float speed { get; set; }
                public int[] startTime { get; set; }
                public int type { get; set; }
                public float visibleTime { get; set; }
                public float yOffset { get; set; }
                public float judgeArea { get; set; }
            }
            [Serializable]
            public class EventLayers
            {
                public Event[] alphaEvents { get; set; }
                public Event[] moveXEvents { get; set; }
                public Event[] moveYEvents { get; set; }
                public Event[] rotateEvents { get; set; }
                public Event[] speedEvents { get; set; }
            }
            [Serializable]
            public class Event
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? bezier { get; set; } = null;
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float[] bezierPoints { get; set; }
                public float easingLeft { get; set; } = 0.0f;
                public float easingRight { get; set; } = 1.0f;
                public int easingType { get; set; } = 1;
                public float end { get; set; }
                public int[] endTime { get; set; }
                public float start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class ColorEvent
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? bezier { get; set; } = null;
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float[] bezierPoints { get; set; }
                public float easingLeft { get; set; }
                public float easingRight { get; set; }
                public int easingType { get; set; }
                public int[] end { get; set; }
                public int[] endTime { get; set; }
                public int[] start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class TextEvent
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public int? bezier { get; set; } = null;
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public float[] bezierPoints { get; set; }
                public float easingLeft { get; set; }
                public float easingRight { get; set; }
                public int easingType { get; set; }
                public string end { get; set; }
                public int[] endTime { get; set; }
                public string start { get; set; }
                public int[] startTime { get; set; }
            }
            [Serializable]
            public class Extended
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public Event[] scaleXEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public Event[] scaleYEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public ColorEvent[] colorEvents { get; set; }
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public TextEvent[] textEvents { get; set; }
            }
            [Serializable]
            public class Root
            {
                public BPMItem[] BPMList { get; set; }
                public META META { get; set; }
                public JudgeLine[] judgeLineList { get; set; }
            }
        }
    }
}
