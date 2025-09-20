using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public static class Utils
    {
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

                int n = 64;
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
    }
}
