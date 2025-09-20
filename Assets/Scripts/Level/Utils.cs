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

                int n = 128;
                double stm = sd / n, td = 0;

                double prevSpeed = Easings.Lerp(s.easing, ss, st, et, s.start, s.end,
                    s.easingLeft, s.easingRight, s.bezierPoints != null && s.bezierPoints.Length >= 4, s.bezierPoints);

                for (int j = 1; j <= n; j++)
                {
                    double ct = ss + j * stm;
                    double currSpeed = Easings.Lerp(s.easing, ct, st, et, s.start, s.end,
                        s.easingLeft, s.easingRight, s.bezierPoints != null && s.bezierPoints.Length >= 4, s.bezierPoints);

                    td += (prevSpeed + currSpeed) * stm / 2;
                    prevSpeed = currSpeed;
                }

                p += (float)(s.floorPosition + td);
                if (b) break;
            }
            return p;
        }


        public static Vector2 LocalToWorld(Vector2 localPos, Vector2 parentPos, float parentRot)
        {
            float rad = parentRot * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float rotatedX = localPos.x * cos - localPos.y * sin;
            float rotatedY = localPos.x * sin + localPos.y * cos;

            return new Vector2(rotatedX + parentPos.x, rotatedY + parentPos.y);
        }

        public static int RgbaToInt(byte r, byte g, byte b, byte a)
        {
            return (r << 24) | (g << 16) | (b << 8) | a;
        }

        public static Color IntToColor(int colorInt)
        {
            byte r = (byte)(colorInt >> 24);
            byte g = (byte)(colorInt >> 16);
            byte b = (byte)(colorInt >> 8);
            byte a = (byte)colorInt;

            return new Color(
                r / 255f,
                g / 255f,
                b / 255f,
                a / 255f
            );
        }

        public static bool IsLineIntersectingRect(Vector2 p1, Vector2 p2, float left, float right, float bottom, float top)
        {
            if (IsPointInRect(p1, left, right, bottom, top) || IsPointInRect(p2, left, right, bottom, top))
                return true;

            return LineIntersectsLine(p1, p2, new Vector2(left, bottom), new Vector2(left, top)) ||   // 左边界
                   LineIntersectsLine(p1, p2, new Vector2(right, bottom), new Vector2(right, top)) ||  // 右边界
                   LineIntersectsLine(p1, p2, new Vector2(left, bottom), new Vector2(right, bottom)) ||// 下边界
                   LineIntersectsLine(p1, p2, new Vector2(left, top), new Vector2(right, top));        // 上边界
        }

        private static bool IsPointInRect(Vector2 point, float left, float right, float bottom, float top)
        {
            return point.x >= left && point.x <= right && point.y >= bottom && point.y <= top;
        }

        private static bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
            if (d == 0) return false;

            float t = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
            float u = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

            return t >= 0 && t <= 1 && u >= 0 && u <= 1;
        }
    }
}
