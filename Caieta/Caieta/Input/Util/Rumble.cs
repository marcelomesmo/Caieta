using System;

namespace Caieta
{
    public class Rumble
    {
        //public const int MAX_RUMBLE_MOTORS = 2;
        // Notes: Create more Motors if necessary.
        public float StrengthL;
        public float StrengthR;

        public float Duration;

        public Rumble(float strength_left, float strength_right, float time)
        {
            StrengthL = strength_left;
            StrengthR = strength_right;

            Duration = time;
        }

        /*
         * Notes: We can later extend this object to add ExtendDuration(int increase_duration), 
         * PowerUp(float strength_left, ...) and PowerDown(float strength_left, ...) methods.
         */
    }
}
