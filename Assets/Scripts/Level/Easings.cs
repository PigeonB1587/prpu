using System;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public static class Easings
    {
        public const float invalidValueDefault = 114514f;
        public static float Lerp(int type, double nowTime, double startTime, double endTime, float valueStart, float valueEnd, float el = 0, float er = 1, bool bezier = false, float[] bezierPoint = null)
        {
            if (nowTime <= startTime || nowTime >= endTime) return valueEnd;

            double progress = (nowTime - startTime) / (endTime - startTime);
            double x = (er - el) * progress + el;

            double fxX = bezier
                ? Ease.CubicBezier(bezierPoint[0], bezierPoint[1], bezierPoint[2], bezierPoint[3], x)
                : EasingFunction(type, x);

            double fxEl = bezier
                ? Ease.CubicBezier(bezierPoint[0], bezierPoint[1], bezierPoint[2], bezierPoint[3], el)
                : EasingFunction(type, el);

            double fxEr = bezier
                ? Ease.CubicBezier(bezierPoint[0], bezierPoint[1], bezierPoint[2], bezierPoint[3], er)
                : EasingFunction(type, er);

            double t = (fxX - fxEl) / (fxEr - fxEl);
            float result = Mathf.Lerp(valueStart, valueEnd, (float)t);

            return float.IsNaN(result) || float.IsInfinity(result) ? invalidValueDefault : result;
        }

        public static readonly Func<double, double>[] _easingFunctions = new Func<double, double>[]
        {
    Ease.Linear,

    Ease.InSine,
    Ease.OutSine,
    Ease.InOutSine,

    Ease.InQuad,
    Ease.OutQuad,
    Ease.InOutQuad,

    Ease.InCubic,
    Ease.OutCubic,
    Ease.InOutCubic,

    Ease.InQuart,
    Ease.OutQuart,
    Ease.InOutQuart,

    Ease.InQuint,
    Ease.OutQuint,
    Ease.InOutQuint,

    Ease.InExpo,
    Ease.OutExpo,
    Ease.InOutExpo,

    Ease.InCirc,
    Ease.OutCirc,
    Ease.InOutCirc,

    Ease.InBack,
    Ease.OutBack,
    Ease.InOutBack,

    Ease.InElastic,
    Ease.OutElastic,
    Ease.InOutElastic,

    Ease.InBounce,
    Ease.OutBounce,
    Ease.InOutBounce
        };

        public static double EasingFunction(int type, double t)
        {
            if (type - 1 >= 0 && type - 1 < _easingFunctions.Length)
            {
                return _easingFunctions[type - 1](t);
            }
            else
            {
                return Ease.Linear(t);
            }
        }

        public static class Ease
        {
            public static double Linear(double x) => x;
            public static double OutSine(double x) => Math.Sin((x * Math.PI) / 2);
            public static double InSine(double x) => 1 - Math.Cos((x * Math.PI) / 2);
            public static double OutQuad(double x) => 1 - (1 - x) * (1 - x);
            public static double InQuad(double x) => x * x;
            public static double InOutSine(double x) => (-(Math.Cos(Math.PI * x) - 1) / 2.0);
            public static double InOutQuad(double x) => x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2.0;
            public static double OutCubic(double x) => 1 - (Math.Pow(1 - x, 3));
            public static double InCubic(double x) => (x * x * x);
            public static double OutQuart(double x) => 1 - Math.Pow(1 - x, 4);
            public static double InQuart(double x) => (x * x * x * x);
            public static double InOutCubic(double x) => x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2.0;
            public static double InOutQuart(double x) => x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2.0;
            public static double InOutQuint(double x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2.0;
            public static double InQuint(double x) => x * x * x * x * x;
            public static double OutQuint(double x) => 1 - Math.Pow(1 - x, 5);
            public static double OutExpo(double x) => x == 1 ? 1 : 1 - Math.Pow(2, -10 * x);
            public static double InOutExpo(double x) => x == 0.0 ? 0.0 : x == 1.0 ? 1.0 : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2.0 : (2 - Math.Pow(2, -20 * x + 10)) / 2.0;
            public static double InExpo(double x) => x == 0 ? 0 : Math.Pow(2, 10 * x - 10);
            public static double OutCirc(double x) => Math.Sqrt(1 - Math.Pow(x - 1, 2));
            public static double InCirc(double x) => 1 - Math.Sqrt(1 - Math.Pow(x, 2));
            public static double OutBack(double x) => 1 + (1.70158f + 1) * Math.Pow(x - 1, 3) + 1.70158f * Math.Pow(x - 1, 2);
            public static double InBack(double x) => (1.70158f + 1) * x * x * x - 1.70158f * x * x;
            public static double InOutCirc(double x) => x < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2.0;
            public static double InOutBack(double x) => x < 0.5 ? (Math.Pow(2 * x, 2) * (((1.70158 * 1.525) + 1) * 2 * x - (1.70158 * 1.525))) / 2.0 : (Math.Pow(2 * x - 2, 2) * (((1.70158 * 1.525) + 1) * (x * 2 - 2) + (1.70158 * 1.525)) + 2) / 2.0;
            public static double OutElastic(double x) => x == 0 ? 0 : x == 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * ((2 * Math.PI) / 3)) + 1;
            public static double InOutElastic(double x) => x == 0.0 ? 0.0 : x == 1.0 ? 1.0 : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * (2 * Math.PI / 4.5))) / 2 : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * (2 * Math.PI / 4.5))) / 2 + 1;
            public static double InElastic(double x) => x == 0 ? 0 : x == 1 ? 1 : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * ((2 * Math.PI) / 3));
            public static double OutBounce(double x)
            {
                double n1 = 7.5625;
                double d1 = 2.75;
                if (x < 1 / d1)
                {
                    return n1 * x * x;
                }
                else if (x < 2 / d1)
                {
                    return n1 * (x -= 1.5 / d1) * x + 0.75;
                }
                else if (x < 2.5 / d1)
                {
                    return n1 * (x -= 2.25 / d1) * x + 0.9375;
                }
                else
                {
                    return n1 * (x -= 2.625 / d1) * x + 0.984375;
                }
            }
            public static double InBounce(double x)
            {
                double n1 = 7.5625;
                double d1 = 2.75;
                double fxO(double x)
                {
                    if (x < 1 / d1)
                    {
                        return n1 * x * x;
                    }
                    else if (x < 2 / d1)
                    {
                        return n1 * (x -= 1.5 / d1) * x + 0.75;
                    }
                    else if (x < 2.5 / d1)
                    {
                        return n1 * (x -= 2.25 / d1) * x + 0.9375;
                    }
                    else
                    {
                        return n1 * (x -= 2.625 / d1) * x + 0.984375;
                    }
                }
                return 1 - fxO(1 - x);
            }
            public static double InOutBounce(double x)
            {
                double n1 = 7.5625;
                double d1 = 2.75;
                double fxobc(double x)
                {
                    if (x < 1 / d1)
                    {
                        return n1 * x * x;
                    }
                    else if (x < 2 / d1)
                    {
                        return n1 * (x -= 1.5 / d1) * x + 0.75;
                    }
                    else if (x < 2.5 / d1)
                    {
                        return n1 * (x -= 2.25 / d1) * x + 0.9375;
                    }
                    else
                    {
                        return n1 * (x -= 2.625 / d1) * x + 0.984375;
                    }
                }
                double fx(double x)
                {
                    return x < 0.5 ? (1 - fxobc(1 - 2 * x)) / 2.0 : (1 + fxobc(2 * x - 1)) / 2.0;
                }
                return fx(x);
            }
            public static double CubicBezier(double a, double b, double c, double d, double e)
            {
                double z = e;
                z = z < 0.0 ? 0.0 : z;
                if (z > 1.0) z = 1.0;
                if (z <= 0.0) return 0.0;
                double f = z;
                int g = 0;
                while (g < 8)
                {
                    double h = 1 - f;
                    double i = h * h;
                    double j = f * f;
                    double k = 3 * a * f * i + 3 * c * j * h + j * f;
                    double l = 3 * a * i - 6 * a * f * h + 3 * c * f * (2 - 3 * f) + 3 * j;
                    if (System.Math.Abs(l) < 0.000001) break;
                    double m = k - z;
                    f = f - m / l;
                    if (f < 0) f = 0; else if (f > 1) f = 1;
                    g = g + 1;
                }
                double n = 1 - f;
                double o = n * n;
                double p = f * f;
                return 3 * b * f * o + 3 * d * p * n + p * f;
            }
        }
    }
}
