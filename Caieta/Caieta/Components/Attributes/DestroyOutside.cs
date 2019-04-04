using System;

namespace Caieta
{
    public class DestroyOutside : Component
    {
        public DestroyOutside()
        {
        }

        public override void Update()
        {
            base.Update();

            if(Engine.SceneManager.CurrScene != null)
            {
                if (!Engine.SceneManager.CurrScene.Layout.Contains(Entity.Transform.Position))
                    Entity.Destroy();
            }
        }
    }
}
