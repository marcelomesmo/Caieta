using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class Transform
    {
        public Transform Parent;
        public List<Transform> Children = new List<Transform>();
        // Notes: Where should I update children?

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; UpdateChildrenPosition(); }
        }
        public float X
        {
            get { return _position.X; }
            set { _position.X = value; UpdateChildrenPosition(); }
        }
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; UpdateChildrenPosition(); }
        }
        private Vector2 _position;

        public Vector2 Scale = Vector2.One;

        public float Rotation;

        public Transform()
        {
        }

        #region Pin

        public void PinTo(Transform _parent)
        {
            if (Parent == _parent)
            {
                Debug.Log("[Transform]: Alreadly pinned to object.");
                return;
            }

            // Reset children
            if (Parent != null)
                Parent.Children.Remove(this);
            // Remove from old Parent

            if (_parent != null)
                _parent.Children.Add(this);
            // Add to new Parent

            Parent = _parent;

            UpdateChildren();
        }

        public void UnPin()
        {
            Parent = null;
        }

        #endregion

        #region Fluent setters

        public Transform SetPosition(Vector2 position)
        {
            if (Engine.IsPixelPerfect)
                Calc.Floor(position);

            if (position == Position)
                return this;

            Position = position;

            //UpdateChildrenPosition();

            return this;
        }

        public Transform SetPosition(float x, float y)
        {
            return SetPosition(new Vector2(x, y));
        }

        public Transform SetRotationRadians(float radians)
        {
            Rotation = radians;

            //UpdateChildrenRotation();

            return this;
        }
        public Transform SetRotation(float degrees)
        {
            return SetRotation(MathHelper.ToRadians(degrees));
        }

        public Transform SetScale(Vector2 scale)
        {
            Scale = scale;

            //UpdateChildrenScale();

            return this;
        }

        public Transform SetScale(float scale)
        {
            return SetScale(new Vector2(scale));
        }

        #endregion

        #region Children

        // Force full Update
        public void UpdateChildren()
        { 
            foreach(Transform c in Children)
            {
                c.Position = Position;
                c.Scale = Scale;
                c.Rotation = Rotation;
            }
        }

        private void UpdateChildrenPosition()
        {
            foreach (Transform c in Children)
                c.Position = Position;
        }

        private void UpdateChildrenScale()
        {
            foreach (Transform c in Children)
                c.Scale = Scale;
        }

        private void UpdateChildrenRotation()
        {
            foreach (Transform c in Children)
                c.Rotation = Rotation;
        }

        #endregion

        #region Utils

        public void Clone(Transform transform)
        {
            Position = transform.Position;
            Rotation = transform.Rotation;
            Scale = transform.Scale;
        }

        public override string ToString()
        {
            return string.Format("[Transform]: Parent: {0}\n   at Position: {1}\n   with Rotation: {2}\n   and Scale: {3}]", Parent != null, Position, Rotation, Scale);
        }

        #endregion
    }
}
