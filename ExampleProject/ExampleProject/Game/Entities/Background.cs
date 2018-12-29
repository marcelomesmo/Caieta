using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleProject
{
    public class Background : Entity
    {
        public Background(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            Transform.Position = new Vector2(Graphics.Width / 2, Graphics.Height / 2);

            var bg = Engine.Instance.Content.Load<Texture2D>("BgResTest");
            Add(new Sprite(bg));
            // Add(new Sprite(Resources.Get<Image>("bg")));

            // Notes:


            //////
            /// 
            /// //
            /// 
            ///         A Origem do BoxCollider sendo 0,0 quer dizer que
            ///         vai ser no ponto de origem do transform, ou seja, no centro.
            /// 
            /// 
            ///         
            Add(new BoxCollider(100, Get<Sprite>().Height, 0, 0));
        }
    }
}
