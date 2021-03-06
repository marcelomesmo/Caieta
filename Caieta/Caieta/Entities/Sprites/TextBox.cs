﻿using System;
using Caieta.Components.Renderables.Text;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta.Entities.Sprites
{
    public class TextBox : Entity
    {
        public Text Text;
        public BoxCollider Collider;

        public bool FitToBox;

        public TextBox(string entityname, string content, SpriteFont font, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            Text = new Text(content, font);

            Vector2 _textSize = Text.Font.MeasureString(Text.Content);
            Collider = new BoxCollider(_textSize.X, _textSize.Y);
        }

        public override void Create()
        {
            base.Create();

            Add(Text);

            Add(Collider);

            Get<Text>().Align(Collider);
            //Get<Text>().FitText(Collider);

            if (FitToBox)
                Text.Content = Text.FitText(Collider);
        }

        public override void Update()
        {
            base.Update();

        }

    }
}
