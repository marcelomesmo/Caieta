using System;

namespace Caieta
{
    public class Entity
    {
        /*
         *      PROPERTIES
         */
        public string Name { get; private set; }
        public bool IsVisible;

        /*
         *      POSITION, ANGLE & SCALE
         */
        //public Transform transform;

        public event Action OnCreate;
        public event Action OnDestroy;

        // Notes: Think about taking initial_visibility out, pattern Constructors and init
        public Entity(string entityname, bool initial_visibility = true)
        {
            Name = entityname;
            IsVisible = initial_visibility;
        }

        public virtual void Create()
        {
            Debug.Log("[Entity]: Entity '" + Name + "' added to Scene.");

            if (OnCreate != null)
            {
                Debug.Log("[Entity]: On Create entity trigger.");

                OnCreate();
                OnCreate = null;
            }
        }

        public virtual void Destroy()
        {
            Debug.Log("[Entity]: Entity '" + Name + "' removed from Scene.");

            if (OnDestroy != null)
            {
                Debug.Log("[Entity]: On Destroy entity trigger.");

                OnDestroy();
                OnDestroy = null;
            }
        }

        public virtual void Update()
        {
            // Components.Update();
        }

        public virtual void Render()
        {
            // Components.Render();
        }


        #region Component Search
        /*
         * 
        public T Get<T>() where T : Component
        {
            return Components.Get<T>();
        }
        */
        #endregion
    }
}
