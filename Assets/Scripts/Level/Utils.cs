using System;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public static class Utils
    {
        public static bool GetHL(ChartObject.Root chart, ChartObject.Note thisNote)
        {
            double targetTime = thisNote.startTime.curTime;

            foreach (var judgeLine in chart.judgeLineList)
            {
                if (judgeLine.notes.Length == 0) continue;

                int left = 0;
                int right = judgeLine.notes.Length - 1;

                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    double midTime = judgeLine.notes[mid].startTime.curTime;

                    if (Math.Abs(midTime - targetTime) <= 1e-9)
                    {
                        if (!judgeLine.notes[mid].Equals(thisNote))
                        {
                            return true;
                        }

                        int leftCheck = mid - 1;
                        while (leftCheck >= 0 && Math.Abs(judgeLine.notes[leftCheck].startTime.curTime - targetTime) <= 1e-9)
                        {
                            if (!judgeLine.notes[leftCheck].Equals(thisNote))
                            {
                                return true;
                            }
                            leftCheck--;
                        }

                        int rightCheck = mid + 1;
                        while (rightCheck < judgeLine.notes.Length && Math.Abs(judgeLine.notes[rightCheck].startTime.curTime - targetTime) <= 1e-9)
                        {
                            if (!judgeLine.notes[rightCheck].Equals(thisNote))
                            {
                                return true;
                            }
                            rightCheck++;
                        }

                        break;
                    }
                    else if (midTime < targetTime)
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

        public static float GetCurFloorPosition(double t, ChartObject.SpeedEvent[] events)
        {
            double floorPosition = 0d;
            var e = events[GetEventIndex(t, events)];
            if (t <= e.endTime.curTime)
            {
                floorPosition = e.floorPosition +
                        (e.start + e.start + (e.end - e.start) * (t - e.startTime.curTime) / (e.endTime.curTime - e.startTime.curTime))
                                                            * (t - e.startTime.curTime) / 2d;
            }
            if (t > e.endTime.curTime)
            {
                floorPosition = e.floorPosition + (e.start + e.end) * (e.endTime.curTime - e.startTime.curTime) / 2d + (t - e.endTime.curTime) * e.end;
            }
            return (float)floorPosition;
        }
        private static int GetEventIndex(double curTime, ChartObject.SpeedEvent[] events)
        {
            int left = 0;
            int right = events.Length - 1;
            int index = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (events[mid].startTime.curTime <= curTime)
                {
                    index = mid;
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return index;
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
        public static string GetScoreText(double score)
        {
            score = double.IsFinite(score += 0.5) ? (int)score : int.MaxValue;
            if (score >= 1000000.0) return "1000000";
            return "0" + (score / 100000.0).ToString("F5").Replace(".", "");
        }

        public static int RgbaToInt(byte r, byte g, byte b, byte a) => (r << 24) | (g << 16) | (b << 8) | a;

        public static Color IntToColor(int colorInt) => new(((byte)(colorInt >> 24)) / 255f, ((byte)(colorInt >> 16)) / 255f, ((byte)(colorInt >> 8)) / 255f, ((byte)colorInt) / 255f);

        public static bool GetHoldVisable(Vector2 a, Vector2 b, float xMin, float yMin, float xMax, float yMax)
        {
            if (a == b)
                return a.x > xMin && a.x < xMax && a.y > yMin && a.y < yMax;

            bool aInX = a.x > xMin && a.x < xMax;
            bool bInX = b.x > xMin && b.x < xMax;
            bool aInY = a.y > yMin && a.y < yMax;
            bool bInY = b.y > yMin && b.y < yMax;

            if (!aInX && !bInX && ((a.x <= xMin && b.x <= xMin) || (a.x >= xMax && b.x >= xMax)))
                return false;

            if (!aInY && !bInY && ((a.y <= yMin && b.y <= yMin) || (a.y >= yMax && b.y >= yMax)))
                return false;

            float dx = b.x - a.x;
            float dy = b.y - a.y;

            if ((aInX && aInY) || (bInX && bInY))
                return true;

            if (dx != 0)
            {
                float t = (xMin - a.x) / dx;
                if (t >= 0f && t <= 1f)
                {
                    float yAtX = a.y + t * dy;
                    if (yAtX > yMin && yAtX < yMax)
                        return true;
                }

                t = (xMax - a.x) / dx;
                if (t >= 0f && t <= 1f)
                {
                    float yAtX = a.y + t * dy;
                    if (yAtX > yMin && yAtX < yMax)
                        return true;
                }
            }

            if (dy != 0)
            {
                float t = (yMin - a.y) / dy;
                if (t >= 0f && t <= 1f)
                {
                    float xAtY = a.x + t * dx;
                    if (xAtY > xMin && xAtY < xMax)
                        return true;
                }

                t = (yMax - a.y) / dy;
                if (t >= 0f && t <= 1f)
                {
                    float xAtY = a.x + t * dx;
                    if (xAtY > xMin && xAtY < xMax)
                        return true;
                }
            }

            return false;
        }
    }
}
