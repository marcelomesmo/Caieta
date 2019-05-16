using System;
using Microsoft.Xna.Framework;

namespace Caieta.Components.Attributes
{
    public class ShakeCamera : Component
    {
        private Vector2 _shakeDirection;
        private Vector2 _shakeOffset;
        private float _shakeIntensity;
        private float _shakeDegredation = 0.95f;

        public ShakeCamera()
        {
        }

        public override void Update()
        {
            base.Update();

            if (Math.Abs(_shakeIntensity) > 0f)
            {
                //_shakeDirection = ShakeVector();
                _shakeOffset = _shakeDirection;
                if (_shakeOffset.X != 0f || _shakeOffset.Y != 0f)
                {
                    _shakeOffset.Normalize();
                }
                else
                {
                    _shakeOffset.X = _shakeOffset.X + Calc.RandomFloat() - 0.5f;
                    _shakeOffset.Y = _shakeOffset.Y + Calc.RandomFloat() - 0.5f;
                }

                // TODO: this needs to be multiplied by camera zoom so that less shake gets applied when zoomed in
                _shakeOffset *= _shakeIntensity;
                _shakeIntensity *= -_shakeDegredation;
                if (Math.Abs(_shakeIntensity) <= 0.01f)
                {
                    _shakeIntensity = 0f;
                    IsActive = false;
                }
            }

            Engine.SceneManager.Camera.Position += _shakeOffset;
        }

        public void Shake(float intensity = 15f, float degredation = 0.9f, Vector2 direction = default(Vector2))
        {
            IsActive = true;

            // Avoid subscribing stronger shakes
            if (_shakeIntensity < intensity)
            {
                _shakeDirection = direction;
                _shakeIntensity = intensity;
                _shakeDegredation = MathHelper.Clamp(degredation, 0f, 0.95f);
            }
        }

        public void StopShake()
        {

        }

        private int[] shakeVectorOffsets = new int[] { -1, -1, 0, 1, 1 };
        public Vector2 ShakeVector()
        {
            return new Vector2(Calc.Choose(shakeVectorOffsets), Calc.Choose(shakeVectorOffsets));
        }

        public override string ToString()
        {
            return string.Format("[ShakeCamera]: Direction {0} Offset {1} Intensity {2} Degredation {3} Camera Position {4}",
                                _shakeDirection, _shakeOffset, _shakeIntensity, _shakeDegredation, Engine.SceneManager.Camera.Position);
        }
    }
}
