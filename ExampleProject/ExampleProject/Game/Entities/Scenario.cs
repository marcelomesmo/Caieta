using System;
using Caieta;
using Caieta.Entities;

namespace ExampleProject
{
    public class Scenario : TiledMap
    {
        public Scenario(string entityname, string map, bool initial_visibility = true) : base(entityname, map, initial_visibility)
        {

        }

        public override void Create()
        {
            base.Create();

            //Add(new TiledMap("BG", "Content/Tiled/untitled.tmx"));
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
