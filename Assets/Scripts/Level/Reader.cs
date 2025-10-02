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
                storyBoard = GetStoryBoard(root.storyBoard),
                judgeLineList = GetJudgeLines(root.judgeLineList)
            };

            await UniTask.CompletedTask;
            return;
        }

        private ChartObject.StoryBoard GetStoryBoard(Prpu.Chart.StoryBoard prpuStoryBoard)
        {
            if (prpuStoryBoard == null) return new ChartObject.StoryBoard();

            return new ChartObject.StoryBoard
            {
                eventType = prpuStoryBoard.eventType,
                events = GetJudgeLineEvents(prpuStoryBoard.events, null)
            };
        }

        private ChartObject.JudgeLine[] GetJudgeLines(Prpu.Chart.JudgeLine[] prpuJudgeLines)
        {
            if (prpuJudgeLines == null) return Array.Empty<ChartObject.JudgeLine>();

            var judgeLines = new ChartObject.JudgeLine[prpuJudgeLines.Length];
            for (int i = 0; i < prpuJudgeLines.Length; i++)
            {
                var prpuJudgeLine = prpuJudgeLines[i];
                var GetedBpms = GetBpmItems(prpuJudgeLine.bpms);
                var judgeLine = new ChartObject.JudgeLine
                {
                    bpms = GetedBpms,
                    speedEvents = GetSpeedEvents(prpuJudgeLine.speedEvents, prpuJudgeLine.bpms),
                    noteControls = GetNoteControls(prpuJudgeLine.noteControls),
                    judgeLineEventLayers = GetJudgeLineEventLayers(prpuJudgeLine.judgeLineEventLayers, prpuJudgeLine.bpms),
                    transform = GetTransform(prpuJudgeLine.transform, prpuJudgeLine.bpms)
                };

                judgeLine.notes = GetNotes(prpuJudgeLine.notes, prpuJudgeLine.bpms, judgeLine.speedEvents);

                judgeLines[i] = judgeLine;
            }
            return judgeLines;
        }

        private ChartObject.BpmItems[] GetBpmItems(Prpu.Chart.BpmItems[] prpuBpmItems)
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

        private ChartObject.Note[] GetNotes(Prpu.Chart.Note[] prpuNotes, Prpu.Chart.BpmItems[] prpuBpmItems, ChartObject.SpeedEvent[] speedEvents)
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

        private ChartObject.NoteControl GetNoteControls(Prpu.Chart.NoteControl prpuControl)
        {
            if (prpuControl == null) return new ChartObject.NoteControl();
            var noteControl = new ChartObject.NoteControl()
            {
                disappearControls = GetControlItems(prpuControl.disappearControls),
                rotateControls = GetControlItems(prpuControl.rotateControls),
                sizeControl = GetControlItems(prpuControl.sizeControl),
                xPosControl = GetControlItems(prpuControl.xPosControl),
                yPosControl = GetControlItems(prpuControl.yPosControl)
            };
            return noteControl;
        }

        private ChartObject.ControlItem[] GetControlItems(Prpu.Chart.ControlItem[] prpuControlItems)
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

        private ChartObject.SpeedEvent[] GetSpeedEvents(Prpu.Chart.JudgeLineEvent[] prpuSpeedEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
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
                        int n = GameInformation.Instance.speedEventLerpSize;
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

        private ChartObject.JudgeLineEventLayer[] GetJudgeLineEventLayers(Prpu.Chart.JudgeLineEventLayer[] prpuLayers, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuLayers == null) return Array.Empty<ChartObject.JudgeLineEventLayer>();

            var layers = new ChartObject.JudgeLineEventLayer[prpuLayers.Length];
            for (int i = 0; i < prpuLayers.Length; i++)
            {
                var prpuLayer = prpuLayers[i];
                layers[i] = new ChartObject.JudgeLineEventLayer
                {
                    judgeLineMoveXEvents = GetJudgeLineEvents(prpuLayer.judgeLineMoveXEvents, prpuBpmItems),
                    judgeLineMoveYEvents = GetJudgeLineEvents(prpuLayer.judgeLineMoveYEvents, prpuBpmItems),
                    judgeLineRotateEvents = GetJudgeLineEvents(prpuLayer.judgeLineRotateEvents, prpuBpmItems),
                    judgeLineDisappearEvents = GetJudgeLineEvents(prpuLayer.judgeLineDisappearEvents, prpuBpmItems)
                };
            }
            return layers;
        }

        private ChartObject.JudgeLineEvent[] GetJudgeLineEvents(Prpu.Chart.JudgeLineEvent[] prpuEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
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

        private ChartObject.Transform GetTransform(Prpu.Chart.Transform prpuTransform, Prpu.Chart.BpmItems[] prpuBpmItems)
        {
            if (prpuTransform == null) return new ChartObject.Transform();

            return new ChartObject.Transform
            {
                judgeLineColorEvents = GetJudgeLineEvents(prpuTransform.judgeLineColorEvents, prpuBpmItems),
                judgeLineTextEvents = GetTextEvents(prpuTransform.judgeLineTextEvents, prpuBpmItems),
                judgeLineTextureSize = prpuTransform.judgeLineTextureSize,
                fatherLineIndex = prpuTransform.fatherLineIndex,
                anchor = prpuTransform.anchor,
                localPositionMode = prpuTransform.localPositionMode,
                localEulerAnglesMode = prpuTransform.localEulerAnglesMode,
                zOrder = prpuTransform.zOrder,
                judgeLineTextureScaleXEvents = GetJudgeLineEvents(prpuTransform.judgeLineTextureScaleXEvents, prpuBpmItems),
                judgeLineTextureScaleYEvents = GetJudgeLineEvents(prpuTransform.judgeLineTextureScaleYEvents, prpuBpmItems)
            };
        }

        private ChartObject.TextEvent[] GetTextEvents(Prpu.Chart.TextEvent[] prpuTextEvents, Prpu.Chart.BpmItems[] prpuBpmItems)
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
