using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace ExampleProject
{
    public class Floor : Entity
    {
        public Floor(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            Transform.Position = new Vector2(Graphics.Width / 2, Graphics.Height / 2);

            // Notes:
            Add(new BoxCollider(500, 50, -150, 0));
        }
    }
}
