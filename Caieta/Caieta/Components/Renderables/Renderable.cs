using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public abstract class Renderable : Component
    {
        /*
         *      Graphics
         */
        public bool IsVisible;

        public Color Color = Color.White;
        public SpriteEffects Effects = SpriteEffects.None;
        public bool IsMirrored
        {
            get { return (Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }

            set { Effects = value ? (Effects | SpriteEffects.FlipHorizontally) : (Effects & ~SpriteEffects.FlipHorizontally); }
        }

        // Notes: the Get will check if Effects contain the Flip tag
        //          while the Set will add or remove the tag.
        public bool IsFlipped
        {
            get { return (Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }

            set { Effects = value ? (Effects | SpriteEffects.FlipVertically) : (Effects & ~SpriteEffects.FlipVertically); }
        }

        protected Renderable()
        {
            IsVisible = true;
        }

        public override void Update()
        {

        }

        public override void Render()
        {

        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[Renderable]: Visibility: {0} ", IsVisible);
        }

        #endregion
    }
}
