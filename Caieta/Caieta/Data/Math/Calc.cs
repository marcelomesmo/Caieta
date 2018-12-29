using System;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public static class Calc
    {

        #region operations

        // Floor Vector2 to explicit (int) cast Vector2
        public static Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)Math.Floor(val.X), (int)Math.Floor(val.Y));
        }

        #endregion
    }
}
