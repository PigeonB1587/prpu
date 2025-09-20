using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Reader : MonoBehaviour
    {
        public static ChartObject.Root chart;
        public int formatVersion;
        public float offset;
        public const float bottomSpeed = 1;
        private Prpu.Chart.Root chartRoot;
        public async UniTask ReadChart(Prpu.Chart.Root root, string songID, string level)
        {
            formatVersion = root.formatVersion;
            offset = root.offset;
            chartRoot = root;
            chart = new ChartObject.Root
            {
                songID = songID,
                level = level,
                storyBoard = ConvertStoryBoard(root.storyBoard),
                judgeLineList = ConvertJudgeLines(root.judgeLineList)
            };

            await UniTask.CompletedTask;
            return;
        }

        private ChartObject.StoryBoard ConvertStoryBoard(Prpu.Chart.StoryBoard prpuStoryBoard)
        {
            if (prpuStoryBoard == null) return null;

            return new ChartObject.StoryBoard
            {
                eventType = prpuStoryBoard.eventType,
                events = ConvertJudgeLineEvents(prpuStoryBoard.events, null)
            };
        }

        private ChartObject.JudgeLine[] ConvertJudgeLines(Prpu.Chart.JudgeLine[] prpuJudgeLines)
        {
            if (prpuJudgeLines == null) return Array.Empty<ChartObject.JudgeLine>();

            var judgeLines = new ChartObject.JudgeLine[prpuJudgeLines.Length];
            for (int i = 0; i < prpuJudgeLines.Length; i++)
            {
                var prpuJudgeLine = prpuJudgeLines[i];
                var convertedBpms = ConvertBpmItems(prpuJudgeLine.bpms);
                var judgeLine = new ChartObject.JudgeLine
                {
                    bpms = convertedBpms,
                    speedEvents = ConvertSpeedEvents(prpuJudgeLine.speedEvents, prpuJudgeLine.bpms),
                    noteControls = ConvertNoteControls(prpuJudgeLine.noteControls),
                    judgeLineEventLayers = ConvertJudgeLineEventLayers(prpuJudgeLine.judgeLineEventLayers, prpuJudgeLine.bpms),
                    transform = ConvertTransform(prpuJudgeLine.transform, prpuJudgeLine.bpms)
                };

                judgeLine.notes = ConvertNotes(prpuJudgeLine.notes, prpuJudgeLine.bpms, judgeLine.speedEvents);

                judgeLines[i] = judgeLine;
            }
            return judgeLines;
        }

        private ChartObject.BpmItems[] ConvertBpmItems(Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuBpmItems == null) return Array.Empty<ChartObject.BpmItems>();

            var bpmItems = new ChartObject.BpmItems[prpuBpmItems.Length];
            for (int i = 0; i < prpuBpmItems.Length; i++)
            {
                var prpuBpm = prpuBpmItems[i];
                bpmItems[i] = new ChartObject.BpmItems
                {
                    bpm = prpuBpm.bpm,
                    time = new ChartObject.Time().GetTime(prpuBpmItems, prpuBpm.time)
                };
            }
            return bpmItems;
        }

        private ChartObject.Note[] ConvertNotes(Prpu.Chart.Note[] prpuNotes, Prpu.Chart.BpmItems[] prpuBpmItems, ChartObject.SpeedEvent[] speedEvents)
        {
            if (prpuNotes == null) return Array.Empty<ChartObject.Note>();

            var notes = new ChartObject.Note[prpuNotes.Length];
            for (int i = 0; i < prpuNotes.Length; i++)
            {
                var prpuNote = prpuNotes[i];
                notes[i] = new ChartObject.Note
                {
                    type = prpuNote.type,
                    isFake = prpuNote.isFake,
                    above = prpuNote.above,
                    startTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuNote.startTime),
                    visibleTime = prpuNote.visibleTime,
                    speed = prpuNote.speed,
                    size = prpuNote.size,
                    isHL = Utils.GetHL(chartRoot, prpuNote),
                    endTime = prpuNote.endTime != null ? new ChartObject.Time().GetTime(prpuBpmItems, prpuNote.endTime) : default,
                    positionX = prpuNote.positionX,
                    positionY = prpuNote.positionY,
                    color = prpuNote.color,
                    hitFXColor = prpuNote.hitFXColor,
                    judgeSize = prpuNote.judgeSize,
                    floorPosition = 0,
                    endfloorPosition = 0
                };

                notes[i].floorPosition = Utils.GetCurFloorPosition(notes[i].startTime.curTime, speedEvents);
                notes[i].endfloorPosition = Utils.GetCurFloorPosition(notes[i].endTime.curTime, speedEvents);
            }
            return notes;
        }

        private ChartObject.NoteControl[] ConvertNoteControls(Prpu.Chart.NoteControl[] prpuNoteControls)
        {
            if (prpuNoteControls == null) return Array.Empty<ChartObject.NoteControl>();

            var noteControls = new ChartObject.NoteControl[prpuNoteControls.Length];
            for (int i = 0; i < prpuNoteControls.Length; i++)
            {
                var prpuControl = prpuNoteControls[i];
                noteControls[i] = new ChartObject.NoteControl
                {
                    disappearControls = ConvertControlItems(prpuControl.disappearControls),
                    rotateControls = ConvertControlItems(prpuControl.rotateControls),
                    sizeControl = ConvertControlItems(prpuControl.sizeControl),
                    xPosControl = ConvertControlItems(prpuControl.xPosControl),
                    yPosControl = ConvertControlItems(prpuControl.yPosControl)
                };
            }
            return noteControls;
        }

        private ChartObject.ControlItem[] ConvertControlItems(Prpu.Chart.ControlItem[] prpuControlItems)
        {
            if (prpuControlItems == null) return Array.Empty<ChartObject.ControlItem>();

            var controlItems = new ChartObject.ControlItem[prpuControlItems.Length];
            for (int i = 0; i < prpuControlItems.Length; i++)
            {
                var prpuItem = prpuControlItems[i];
                controlItems[i] = new ChartObject.ControlItem
                {
                    easing = prpuItem.easing,
                    value = prpuItem.value,
                    x = prpuItem.x
                };
            }
            return controlItems;
        }

        private ChartObject.SpeedEvent[] ConvertSpeedEvents(Prpu.Chart.JudgeLineEvent[] prpuSpeedEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuSpeedEvents == null) return Array.Empty<ChartObject.SpeedEvent>();

            var speedEvents = new ChartObject.SpeedEvent[prpuSpeedEvents.Length];

            for (int i = 0; i < prpuSpeedEvents.Length; i++)
            {
                var prpuEvent = prpuSpeedEvents[i];
                speedEvents[i] = new ChartObject.SpeedEvent
                {
                    startTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.startTime),
                    endTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.endTime),
                    start = prpuEvent.start,
                    end = prpuEvent.end,
                    easing = prpuEvent.easing,
                    floorPosition = 0,
                    easingLeft = prpuEvent.easingLeft,
                    easingRight = prpuEvent.easingRight,
                    bezierPoints = prpuEvent.bezierPoints
                };
                //µæµ×ÊÂ¼þ
                if(i == 0 && speedEvents[i].startTime.curTime > 0)
                {
                    speedEvents[i].floorPosition = (float)(speedEvents[i].startTime.curTime) * bottomSpeed;
                }
                if (i > 0)
                {
                    var last = speedEvents[i - 1];
                    double td = 0;
                    double d = last.endTime.curTime - last.startTime.curTime;

                    if (d > 0)
                    {
                        int n = 128;
                        double stm = d / n;
                        double prevSpeed = Easings.Lerp(last.easing, last.startTime.curTime,
                            last.startTime.curTime, last.endTime.curTime,
                            last.start, last.end, last.easingLeft, last.easingRight,
                            last.bezierPoints?.Length >= 4, last.bezierPoints);

                        for (int s = 1; s <= n; s++)
                        {
                            double ct = last.startTime.curTime + s * stm;
                            double currSpeed = Easings.Lerp(last.easing, ct,
                                last.startTime.curTime, last.endTime.curTime,
                                last.start, last.end, last.easingLeft, last.easingRight,
                                last.bezierPoints?.Length >= 4, last.bezierPoints);

                            td += (prevSpeed + currSpeed) * stm / 2;
                            prevSpeed = currSpeed;
                        }
                    }
                }
            }
            return speedEvents;
        }

        private ChartObject.JudgeLineEventLayer[] ConvertJudgeLineEventLayers(Prpu.Chart.JudgeLineEventLayer[] prpuLayers, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuLayers == null) return Array.Empty<ChartObject.JudgeLineEventLayer>();

            var layers = new ChartObject.JudgeLineEventLayer[prpuLayers.Length];
            for (int i = 0; i < prpuLayers.Length; i++)
            {
                var prpuLayer = prpuLayers[i];
                layers[i] = new ChartObject.JudgeLineEventLayer
                {
                    judgeLineMoveXEvents = ConvertJudgeLineEvents(prpuLayer.judgeLineMoveXEvents, prpuBpmItems),
                    judgeLineMoveYEvents = ConvertJudgeLineEvents(prpuLayer.judgeLineMoveYEvents, prpuBpmItems),
                    judgeLineRotateEvents = ConvertJudgeLineEvents(prpuLayer.judgeLineRotateEvents, prpuBpmItems),
                    judgeLineDisappearEvents = ConvertJudgeLineEvents(prpuLayer.judgeLineDisappearEvents, prpuBpmItems)
                };
            }
            return layers;
        }

        private ChartObject.JudgeLineEvent[] ConvertJudgeLineEvents(Prpu.Chart.JudgeLineEvent[] prpuEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuEvents == null) return Array.Empty<ChartObject.JudgeLineEvent>();

            var events = new ChartObject.JudgeLineEvent[prpuEvents.Length];
            for (int i = 0; i < prpuEvents.Length; i++)
            {
                var prpuEvent = prpuEvents[i];
                events[i] = new ChartObject.JudgeLineEvent
                {
                    startTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.startTime),
                    endTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.endTime),
                    start = prpuEvent.start,
                    end = prpuEvent.end,
                    easing = prpuEvent.easing,
                    easingLeft = prpuEvent.easingLeft,
                    easingRight = prpuEvent.easingRight,
                    bezierPoints = prpuEvent.bezierPoints
                };
            }
            return events;
        }

        private ChartObject.Transform ConvertTransform(Prpu.Chart.Transform prpuTransform, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuTransform == null) return null;

            return new ChartObject.Transform
            {
                judgeLineColorEvents = ConvertJudgeLineEvents(prpuTransform.judgeLineColorEvents, prpuBpmItems),
                judgeLineTextEvents = ConvertTextEvents(prpuTransform.judgeLineTextEvents, prpuBpmItems),
                judgeLineTextureSize = prpuTransform.judgeLineTextureSize,
                fatherLineIndex = prpuTransform.fatherLineIndex,
                anchor = prpuTransform.anchor,
                localPositionMode = prpuTransform.localPositionMode,
                localEulerAnglesMode = prpuTransform.localEulerAnglesMode,
                zOrder = prpuTransform.zOrder,
                judgeLineTextureScaleXEvents = ConvertJudgeLineEvents(prpuTransform.judgeLineTextureScaleXEvents, prpuBpmItems),
                judgeLineTextureScaleYEvents = ConvertJudgeLineEvents(prpuTransform.judgeLineTextureScaleYEvents, prpuBpmItems)
            };
        }

        private ChartObject.TextEvent[] ConvertTextEvents(Prpu.Chart.TextEvent[] prpuTextEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuTextEvents == null) return Array.Empty<ChartObject.TextEvent>();

            var textEvents = new ChartObject.TextEvent[prpuTextEvents.Length];
            for (int i = 0; i < prpuTextEvents.Length; i++)
            {
                var prpuEvent = prpuTextEvents[i];
                textEvents[i] = new ChartObject.TextEvent
                {
                    startTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.startTime),
                    endTime = new ChartObject.Time().GetTime(prpuBpmItems, prpuEvent.endTime),
                    start = prpuEvent.start,
                    end = prpuEvent.end,
                    easing = prpuEvent.easing,
                    easingCutting = prpuEvent.easingCutting,
                    bezierPoints = prpuEvent.bezierPoints
                };
            }
            return textEvents;
        }
    }
}
