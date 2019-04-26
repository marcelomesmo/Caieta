using System;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public static class Calc
    {

        #region operations
        /* XNA MathHelper has it already
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }*/

        // Floor Vector2 to explicit (int) cast Vector2
        public static Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)Math.Floor(val.X), (int)Math.Floor(val.Y));
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        /*
         *           ANGLE ROTARION & ORIENTATION
         * 
         *   Engine angles start at: 0º   pointing  right  (1,0)
         *      spins down and goes: 90º  pointing  down   (0,1)
         *  keep spinning clockwise: 180º pointing  left  (-1,0)
         *             follow up to: 270º pointing  up    (0,-1)
         *      and finish back to 360º/0º.
         */
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
        /*
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }
        */
        #endregion


        #region Time

        public static string GetHumanReadableTime(TimeSpan timeSpan)
        {
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if(minutes < 10)
            {
                if (seconds < 10)
                    return "0" + minutes + ":0" + seconds;
                else
                    return "0" + minutes + ":" + seconds;
            }
            else
            {
                if (seconds < 10)
                    return minutes + ":0" + seconds;
                else
                    return minutes + ":" + seconds;
            }
        }

        #endregion
    }
}
