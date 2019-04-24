using System;
using Caieta.Components.Renderables.Text;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class TextBox : Entity
    {
        public Text Text;
        public BoxCollider Collider;

        public TextBox(string entityname, string content, string font = "", bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            _content = content;
            _fontName = font;
        }

        private Vector2 _textSize;
        private string _content, _fontName;

        public override void Create()
        {
            base.Create();

            Text = new Text(_content, _fontName);
            Add(Text);

            _textSize = Text.Font.MeasureString(_content);
            Collider = new BoxCollider(_textSize.X, _textSize.Y);
            Add(Collider);

            Get<Text>().Align(Collider);
        }

    }
}
