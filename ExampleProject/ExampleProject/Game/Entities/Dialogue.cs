using System;
using Caieta.Components.Renderables.Text;
using Caieta.Entities;

namespace ExampleProject.Game.Entities
{
    public class Dialogue : Entity
    {
        Text texto_teste;

        public Dialogue(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {

        }

        public override void Create()
        {
            base.Create();

            //texto_teste = new Text("");
        }

        public override void Update()
        {
            base.Update();


        }
    }
}
