using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Camera
    {
        public Viewport Viewport;

        public Camera()
        {
            Viewport = new Viewport();
            Viewport.Width = Graphics.ViewWidth;
            Viewport.Height = Graphics.ViewHeight;
            UpdateMatrices();
        }

        public Camera(int width, int height)
        {
            Viewport = new Viewport();
            Viewport.Width = width;
            Viewport.Height = height;
            UpdateMatrices();
        }

        public void Resize(int width, int height)
        {
            Viewport.Width = width;
            Viewport.Height = height;
            UpdateMatrices();
        }

        public override string ToString()
        {
            return "[Camera]:\n\tViewport: { " + Viewport.X + ", " + Viewport.Y + ", " + Viewport.Width + ", " + Viewport.Height +
                " }\n\tPosition: { " + _position.X + ", " + _position.Y +
                " }\n\tOrigin: { " + _origin .X + ", " + _origin .Y +
                " }\n\tZoom: { " + _zoom.X + ", " + _zoom.Y +
                " }\n\tAngle: " + _angle;
        }

        private void UpdateMatrices()
        {
            _matrix = Matrix.Identity *
                    //Matrix.CreateTranslation(new Vector3(-new Vector2((int)Math.Round(_position.X) * _parallax.X, (int)Math.Round(_position.Y) * _parallax.Y), 0)) *
                    Matrix.CreateTranslation(new Vector3(-_position * _parallax, 0.0f)) *
                    //Matrix.CreateTranslation(new Vector3(-_origin, 0.0f)) *
                    Matrix.CreateRotationZ(_angle) *
                    Matrix.CreateScale(new Vector3(_zoom, 1)) *
                    //Matrix.CreateTranslation(new Vector3(new Vector2((int)Math.Round(_origin.X), (int)Math.Round(_origin.Y)), 0));
                    Matrix.CreateTranslation(new Vector3(_origin, 0.0f));

            _inverse = Matrix.Invert(_matrix);

            _changed = false;
        }

        public void Reset()
        {
            Origin = Vector2.Zero;
            Position = Vector2.Zero;
            Parallax = Vector2.One;
            Zoom = 1;
            Angle = 0;
            UpdateMatrices();
        }

        public void CopyFrom(Camera other)
        {
            _position = other._position;
            _origin  = other._origin ;
            _angle = other._angle;
            _zoom = other._zoom;
            _changed = true;
        }

        public Matrix Matrix
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return _matrix;
            }
        }
        private Matrix _matrix = Matrix.Identity;
        private bool _changed;

        public Matrix Inverse
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return _inverse;
            }
        }
        private Matrix _inverse = Matrix.Identity;

        /*
         *      ORIGIN
         */
        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _changed = true;
                _origin = value;
            }
        }
        private Vector2 _origin = Vector2.Zero;

        public Vector2 LerpStrength
        {
            get { return _lerp; }
            set
            {
                _changed = true;
                _lerp = value;
            }
        }
        private Vector2 _lerp = Vector2.One;

        /*
         *      POSITION
         */
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _changed = true;
                _position = value;
            }
        }
        private Vector2 _position = Vector2.Zero;

        public float X
        {
            get { return _position.X; }
            set
            {
                _changed = true;
                _position.X = value;
            }
        }

        public float Y
        {
            get { return _position.Y; }
            set
            {
                _changed = true;
                _position.Y = value;
            }
        }

        public Vector2 Parallax
        {
            get { return _parallax; }
            set
            {
                _changed = true;
                _parallax = value;
            }
        }
        private Vector2 _parallax = Vector2.One;

        /*
         *      ZOOM
         */
        public float Zoom
        {
            get { return _zoom.X; }
            set
            {
                _changed = true;
                _zoom.X = _zoom.Y = value;
            }
        }
        private Vector2 _zoom = Vector2.One;
        public float MinZoom
        {
            get { return _minZoom; }
            set
            {
                _minZoom = value;

                if (Zoom < _minZoom)
                    Zoom = _minZoom;
            }
        }
        private float _minZoom = 0.3f;
        public float MaxZoom
        {
            get { return _maxZoom; }
            set
            {
                _maxZoom = value;

                if (Zoom > _maxZoom)
                    Zoom = _maxZoom;
            }
        }
        private float _maxZoom = 3f;

        /*
         *      ANGLE
         */
        public float Angle
        {
            get { return _angle; }
            set
            {
                _changed = true;
                _angle = value;
            }
        }
        private float _angle = 0;

        /*
         *      BOUNDS
         */
        public float Left
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return Vector2.Transform(Vector2.Zero, Inverse).X;
            }

            set
            {
                if (_changed)
                    UpdateMatrices();
                X = Vector2.Transform(Vector2.UnitX * value, Matrix).X;
            }
        }

        public float Right
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return Vector2.Transform(Vector2.UnitX * Viewport.Width, Inverse).X;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public float Top
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return Vector2.Transform(Vector2.Zero, Inverse).Y;
            }

            set
            {
                if (_changed)
                    UpdateMatrices();
                Y = Vector2.Transform(Vector2.UnitY * value, Matrix).Y;
            }
        }

        public float Bottom
        {
            get
            {
                if (_changed)
                    UpdateMatrices();
                return Vector2.Transform(Vector2.UnitY * Viewport.Height, Inverse).Y;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /*
         *  Utils
         */
        public void CenterOrigin()
        {
            _origin  = new Vector2((float)Viewport.Width / 2, (float)Viewport.Height / 2);
            _changed = true;
        }

        public void RoundPosition()
        {
            _position.X = (float)Math.Round(_position.X);
            _position.Y = (float)Math.Round(_position.Y);
            _changed = true;
        }

        public void ClampToLayout()
        {
            _position.X = MathHelper.Clamp(_position.X, Engine.SceneManager.CurrScene.Layout.Left, Engine.SceneManager.CurrScene.Layout.Right - Graphics.Width);
            _position.Y = MathHelper.Clamp(_position.Y, Engine.SceneManager.CurrScene.Layout.Top, Engine.SceneManager.CurrScene.Layout.Bottom - Graphics.Height);
            _changed = true;
        }

        public Vector2 ScreenToCamera(Vector2 position)
        {
            return Vector2.Transform(position, Inverse);
        }

        public Vector2 CameraToScreen(Vector2 position)
        {
            return Vector2.Transform(position, Matrix);
        }

        public void Approach(Vector2 position, float ease)
        {
            Position += (position - Position) * ease;
        }

        public void Approach(Vector2 position, float ease, float maxDistance)
        {
            Vector2 move = (position - Position) * ease;
            if (move.Length() > maxDistance)
                Position += Vector2.Normalize(move) * maxDistance;
            else
                Position += move;
        }

    }
}