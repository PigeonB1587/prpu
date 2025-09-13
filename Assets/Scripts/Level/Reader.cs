using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class Reader : MonoBehaviour
    {
        public static ChartObject.Root chart;
        public int formatVersion;
        public float offset;

        public async UniTask ReadChart(Prpu.Chart.Root root, string songID)
        {
            formatVersion = root.formatVersion;
            offset = root.offset;
            chart = new ChartObject.Root
            {
                songID = songID,
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
                    isHL = false,
                    endTime = prpuNote.endTime != null ? new ChartObject.Time().GetTime(prpuBpmItems, prpuNote.endTime) : default,
                    positionX = prpuNote.positionX,
                    positionY = prpuNote.positionY,
                    color = prpuNote.color,
                    autoPlayHitSound = prpuNote.autoPlayHitSound,
                    hitFXColor = prpuNote.hitFXColor,
                    judgeSize = prpuNote.judgeSize,
                    floorPosition = 0,
                    endfloorPosition = 0
                };

                notes[i].floorPosition = GetCurFloorPosition(notes[i].startTime.curTime, speedEvents);
                notes[i].endfloorPosition = GetCurFloorPosition(notes[i].endTime.curTime, speedEvents);
            }
            return notes;
        }

        public static bool GetHL(Prpu.Chart.Root chart, Prpu.Chart.Note thisNote)
        {
            double targetTime = thisNote.startTime[0] + thisNote.startTime[1] / (double)thisNote.startTime[2];

            for (int i = 0; i < chart.judgeLineList.Length; i++)
            {
                var judgeLine = chart.judgeLineList[i];
                int left = 0;
                int right = judgeLine.notes.Length - 1;

                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    var note = judgeLine.notes[mid];
                    double noteTime = note.startTime[0] + note.startTime[1] / (double)note.startTime[2];

                    if (noteTime == targetTime)
                    {
                        if (!thisNote.Equals(note))
                        {
                            return true;
                        }

                        int temp = mid;
                        while (--temp >= 0)
                        {
                            var leftNote = judgeLine.notes[temp];
                            double leftTime = leftNote.startTime[0] + leftNote.startTime[1] / (double)leftNote.startTime[2];
                            if (leftTime != targetTime) break;
                            if (!thisNote.Equals(leftNote)) return true;
                        }

                        temp = mid;
                        while (++temp < judgeLine.notes.Length)
                        {
                            var rightNote = judgeLine.notes[temp];
                            double rightTime = rightNote.startTime[0] + rightNote.startTime[1] / (double)rightNote.startTime[2];
                            if (rightTime != targetTime) break;
                            if (!thisNote.Equals(rightNote)) return true;
                        }

                        break;
                    }
                    else if (noteTime < targetTime)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid - 1;
                    }
                }
            }

            return false;
        }

        public static float GetCurFloorPosition(double t, ChartObject.SpeedEvent[] e)
        {
            float p = 0.0f;
            for (int i = 0; i < e.Length; i++)
            {
                var s = e[i];
                double st = s.startTime.curTime, et = s.endTime.curTime, d = et - st;

                if (d <= 0)
                {
                    p += s.floorPosition;
                    continue;
                }

                bool a = t >= et, b = t >= st && t < et;
                if (!a && !b) continue;

                double ss = st, se = a ? et : t, sd = se - ss;
                if (sd <= 0)
                {
                    if (a) p += s.floorPosition;
                    continue;
                }

                int n = 100;
                double stm = sd / n, td = 0;
                for (int j = 0; j < n; j++)
                {
                    double ct = ss + j * stm;
                    td += Easings.Lerp(s.easing, ct, st, et, s.start, s.end, s.easingLeft, s.easingRight,
                        s.bezierPoints != null && s.bezierPoints.Length >= 4, s.bezierPoints) * stm;
                }

                p += (float)(s.floorPosition + td);
                if (b) break;
            }
            return p;
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

                if (i > 0)
                {
                    var lastEvent = speedEvents[i - 1];
                    double totalDistance = 0;

                    double eventDuration = lastEvent.endTime.curTime - lastEvent.startTime.curTime;
                    if (eventDuration > 0)
                    {
                        int steps = 100;
                        double stepTime = eventDuration / steps;

                        for (int s = 0; s < steps; s++)
                        {
                            double currentTime = lastEvent.startTime.curTime + s * stepTime;
                            float currentSpeed = Easings.Lerp(
    type: lastEvent.easing,
    nowTime: currentTime,
    startTime: lastEvent.startTime.curTime,
    endTime: lastEvent.endTime.curTime,
    valueStart: lastEvent.start,
    valueEnd: lastEvent.end,
    el: lastEvent.easingLeft,
    er: lastEvent.easingRight,
    bezier: lastEvent.bezierPoints != null && lastEvent.bezierPoints.Length >= 4,
    bezierPoint: lastEvent.bezierPoints
);
                            totalDistance += currentSpeed * stepTime;
                        }
                    }

                    double gapTime = speedEvents[i].startTime.curTime - lastEvent.endTime.curTime;
                    double gapDistance = lastEvent.end * gapTime;

                    speedEvents[i].floorPosition += (float)(lastEvent.floorPosition + totalDistance + gapDistance);
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
