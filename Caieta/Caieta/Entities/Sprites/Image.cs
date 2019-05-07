using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta.Entities.Sprites
{
    // Base entity with a sprite image.
    public class Image : Entity
    {
        public Sprite DisplayImage;
        public BoxCollider Collider;
        private string _ImagePath;

        public Image(string entityname, string image_path = "", bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            _ImagePath = image_path;
        }

        public override void Create()
        {
            base.Create();

            if (_ImagePath != "")
            {
                Texture2D Texture = Resources.Get<Texture2D>(_ImagePath);

                DisplayImage = new Sprite(Texture);
                //DisplayImage.SetOrigin(Animation.Anchor.TOP_LEFT);
            }

            Add(DisplayImage);

            if (DisplayImage != null)
                Collider = new BoxCollider(DisplayImage);

            Collider.IsTrigger = true;

            Add(Collider);

        }


    }
}
