using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class ScoreController : MonoBehaviour
    {
        public static int combo = 0;
        public static float score = 0;
        public static int noteCount = 0;
        public static int maxCombo = 0;

        public static int perfectCount = 0, goodCount = 0, badCount = 0, missCount = 0;

        public static void ResetScore()
        {
            combo = 0;
            score = 0;
            noteCount = 0;
            maxCombo = 0;

            perfectCount = 0;
            goodCount = 0;
            badCount = 0;
            missCount = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="offset">ms</param>
        public static void Hit(HitType type, float offset)
        {
            if (combo >= maxCombo)
                maxCombo = combo;

            switch (type)
            {
                case HitType.Perfect:
                    perfectCount++;
                    combo++;
                    break;
                case HitType.Good:
                    goodCount++;
                    combo++;
                    break;
                case HitType.Bad:
                    badCount++;
                    combo = 0;
                    break;
                case HitType.Miss:
                    missCount++;
                    combo = 0;
                    break;
            }

            if (noteCount == 0)
                return;

            float baseNoteScore = noteCount > 0 ? 900000f / noteCount : 0;

            if (perfectCount == noteCount && noteCount > 0)
            {
                score = 1000000f;
                return;
            }

            float comboBonus = noteCount > 0 ? (maxCombo / (float)noteCount) * 100000f : 0;
            float perfectScore = perfectCount * baseNoteScore;
            float goodScore = goodCount * baseNoteScore * 0.65f;

            score = comboBonus + perfectScore + goodScore;
        }

        public static void CheckMissCount()
        {
            if(perfectCount + goodCount + badCount + missCount != noteCount)
                missCount = noteCount - (perfectCount + goodCount + badCount);
        }
    }
}