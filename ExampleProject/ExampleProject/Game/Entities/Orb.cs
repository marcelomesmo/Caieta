using System;
using Caieta;
using Caieta.Components.Utils;
using Caieta.Entities;

namespace ExampleProject
{
    public class Orb : Entity
    {
        public Sprite image;
        public BoxCollider collider;

        public Orb(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            Add(image);

            Add(collider);

            //Add(new Bullet());
            //Get<Bullet>().Angle = 0;

            if(Name == "Blue_Orb") 
                Add(new Tween(TweenMode.Loop, TweenProperty.X, Transform.Position.X + 100, EaseFunction.ElasticOut, 2500));
            //Add(new Tween(TweenMode.Reverse, TweenProperty.X, Transform.Position.X + 150, EaseFunction.ElasticIn, 2500));
           
           if(Name == "Red_Orb") 
                Add(new Tween(TweenMode.Yoyo, TweenProperty.Opacity, 0, EaseFunction.Linear, 1000));
        }
    }
}
