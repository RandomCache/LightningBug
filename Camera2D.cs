using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

// Advanced Camera2D implementation
// Thanks to: http://blog.roboblob.com/2013/07/27/solving-resolution-independent-rendering-and-2d-camera-using-monogame/
// Modified and enhanced by PantheR (http://www.panthernet.ru)

// Simple usage:
// 1. Create camera
// 2. Call Camera2D.Update() method in your update method
// 3. Provide Camera2D.GetViewTransformationMatrix() to all nedded SpriteBatch.Begin() calls to draw using camera
// 4. Setup camera input that deals with such props as Zoom and Position

namespace LightningBug
{
    public class Camera2D: IDisposable
    {
        #region Variables
        private float _zoom;
        private float _rotation;
        private Vector2 _position;
        private Matrix _transform = Matrix.Identity;
        private bool _isViewTransformationDirty = true;
        private Matrix _camTranslationMatrix = Matrix.Identity;
        private Matrix _camRotationMatrix = Matrix.Identity;
        private Matrix _camScaleMatrix = Matrix.Identity;
        private Matrix _resTranslationMatrix = Matrix.Identity;
        private Object followTarget;

        protected ResolutionRenderer IRR;
        private Vector3 _camTranslationVector = Vector3.Zero;
        private Vector3 _camScaleVector = Vector3.Zero;
        private Vector3 _resTranslationVector = Vector3.Zero;

        /// <summary>
        /// Current camera position
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _isViewTransformationDirty = true;
            }
        }

        /// <summary>
        /// Minimum zoom value (can be no less than 0.1f)
        /// </summary>
        public float MinZoom { get; set; }
        /// <summary>
        /// Maximum zoom value
        /// </summary>
        public float MaxZoom { get; set; }

        /// <summary>
        /// Gets or sets camera zoom value
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < 0.1f)
                    _zoom = 0.1f;
                if (_zoom < MinZoom) _zoom = MinZoom;
                if (_zoom > MaxZoom) _zoom = MaxZoom;
                _isViewTransformationDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets camera rotation value
        /// </summary>
        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                _isViewTransformationDirty = true;
            }
        }
        #endregion

        public Camera2D(ResolutionRenderer irr)
        {
            IRR = irr;
            _zoom = 0.1f;
            _rotation = 0.0f;
            _position = Vector2.Zero;
            MinZoom = 0.1f;
            MaxZoom = 999f;
            followTarget = null;
        }
        /*
        /// <summary>
        /// Center camera and fit the area of specified rectangle
        /// </summary>
        /// <param name="rec">Rectange</param>
        /// <param name="resolution">Screen resolution</param>
        public void CenterOnTarget(Rectangle rec)
        {
            Position = new Vector2(rec.Center.X, rec.Center.Y);
            var fat1 = (float)IRR.VirtualWidth / (float)IRR.VirtualHeight;
            var fat2 = (float)rec.Width / (float)rec.Height;
            float ratio = 0f;
            if (fat2 >= fat1) ratio = (float)IRR.VirtualWidth / (float)rec.Width;
            else ratio = (float)IRR.VirtualHeight / (float)rec.Height;
            Zoom = ratio;
        }
        */
        /// <summary>
        /// Move camera by specified vector
        /// </summary>
        /// <param name="amount">Vector movement</param>
        public void Move(Vector2 amount)
        {
            Position += amount;
        }

        /// <summary>
        /// Set camera position
        /// </summary>
        /// <param name="position">Position</param>
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        /// <summary>
        /// Get camera transformation matrix
        /// </summary>
        public Matrix GetViewTransformationMatrix()
        {
            if (_isViewTransformationDirty)
            {
                _camTranslationVector.X = -_position.X;
                _camTranslationVector.Y = -_position.Y;

                Matrix.CreateTranslation(ref _camTranslationVector, out _camTranslationMatrix);
                Matrix.CreateRotationZ(_rotation, out _camRotationMatrix);

                _camScaleVector.X = _zoom;
                _camScaleVector.Y = _zoom;
                _camScaleVector.Z = 1;

                Matrix.CreateScale(ref _camScaleVector, out _camScaleMatrix);
                
                _resTranslationVector.X = IRR.VirtualWidth * 0.5f;
                _resTranslationVector.Y = IRR.VirtualHeight * 0.5f;
                _resTranslationVector.Z = 0;

                Matrix.CreateTranslation(ref _resTranslationVector, out _resTranslationMatrix);
                
                _transform = _camTranslationMatrix *
                             _camRotationMatrix *
                             _camScaleMatrix *
                             _resTranslationMatrix *
                             IRR.GetTransformationMatrix();

                _isViewTransformationDirty = false;
            }

            return _transform;
        }

        public Matrix GetViewTransformationMatrix2()
        {
            Matrix ret;
            //if (_isViewTransformationDirty)
            {
                _camTranslationVector.X = -_position.X;
                _camTranslationVector.Y = -_position.Y;

                Matrix.CreateTranslation(ref _camTranslationVector, out _camTranslationMatrix);
                Matrix.CreateRotationZ(_rotation, out _camRotationMatrix);

                _camScaleVector.X = _zoom;
                _camScaleVector.Y = _zoom;
                _camScaleVector.Z = 1;

                Matrix.CreateScale(ref _camScaleVector, out _camScaleMatrix);

                _resTranslationVector.X = IRR.VirtualWidth * 0.5f;
                _resTranslationVector.Y = IRR.VirtualHeight * 0.5f;
                _resTranslationVector.Z = 0;

                Matrix.CreateTranslation(ref _resTranslationVector, out _resTranslationMatrix);

                ret = _camTranslationMatrix *
                             _camRotationMatrix *
                             _camScaleMatrix *
                             _resTranslationMatrix;

                _isViewTransformationDirty = false;
            }

            return ret;
        }

        public void RecalculateTransformationMatrices()
        {
            _isViewTransformationDirty = true;
        }

        /// <summary>
        /// Convert screen coordinates to virtual
        /// </summary>
        /// <param name="coord">Coordinates</param>
        public Vector2 ToVirtual(Vector2 coord, bool useIRR = true)
        {
            if (useIRR) coord = coord - new Vector2(IRR.Viewport.X, IRR.Viewport.Y);
            return Vector2.Transform(coord, Matrix.Invert(GetViewTransformationMatrix()));
        }

        public void Dispose()
        {
            IRR = null;
        }

        #region SmoothTransition Logic
        private int _trDuration;
        public bool IsTransitionActive { get; private set; }
        private float _trElapsedTime;
        private Vector2 _trTargetPosition;

        /// <summary>
        /// Start camera transition to specified position
        /// </summary>
        /// <param name="targetPos">Target position</param>
        /// <param name="duration">Expected transition duration</param>
        public void StartTransition(Vector2 targetPos, int duration = 5000)
        {
            if (IsTransitionActive)
                ResetTransition();
            _trTargetPosition = targetPos;
            IsTransitionActive = true;
            _trDuration = duration;
        }
        /// <summary>
        /// Start camera transition to specified position
        /// </summary>
        /// <param name="targetPos">Target position</param>
        /// <param name="duration">Expected transition duration</param>
        public void StartTransition(Point targetPos, int duration = 5000)
        {
            StartTransition(new Vector2(targetPos.X, targetPos.Y), duration);
        }
        /// <summary>
        /// Update transition target position without cancelling current transition
        /// </summary>
        /// <param name="targetPos">Target position</param>
        public void UpdateTransitionTarget(Vector2 targetPos)
        {
            _trTargetPosition = targetPos;
        }

        /// <summary>
        /// Stop and reset transition
        /// </summary>
        public void StopTransition()
        {
            ResetTransition();
            IsTransitionActive = false;
        }
        /// <summary>
        /// Reset current transition
        /// </summary>
        public void ResetTransition()
        {
            _trElapsedTime = 0f;
        }

        private void updateTransition(GameTime gt)
        {
            _trElapsedTime += (float)gt.ElapsedGameTime.TotalMilliseconds;
            var amount = MathHelper.Clamp(_trElapsedTime / _trDuration, 0, 1);

            Vector2 result;
            Vector2 cpos = Position;
            Vector2.Lerp(ref cpos, ref _trTargetPosition, amount, out result);
            SetPosition(result);
            if (amount >= 1f) 
                StopTransition();
        }
        #endregion

        /// <summary>
        /// Update logic (must be called within main update method)
        /// </summary>
        /// <param name="gt">Game time</param>
        public void Update(GameTime gt, uint levelWidth, uint levelHeight)
        {
            if (followTarget != null)
            {
                //if we are already transitioning then just update target coordinates
                if (IsTransitionActive)
                    UpdateTransitionTarget(followTarget.GetPosition());
                else
                    Position = followTarget.GetPosition();
                
                bool changed = false;
                // Don't let the camera show parts outside of the level
                Vector2 finalPosition = followTarget.GetPosition();
                Vector2 screenTopLeft = ToVirtual(Vector2.Zero, false);
                Vector2 screenBottomRight = ToVirtual(new Vector2(IRR.ScreenWidth, IRR.ScreenHeight), false);
                float halfWidth = (screenBottomRight.X - screenTopLeft.X) / 2;
                float halfHeight = (screenBottomRight.Y - screenTopLeft.Y) / 2;
                
                if (_position.X - halfWidth < 0)
                {
                    finalPosition.X -= _position.X - halfWidth; 
                    changed = true;
                }
                else if (screenBottomRight.X > levelWidth)
                {
                    finalPosition.X -= screenBottomRight.X - levelWidth;
                    changed = true;
                }
                if (_position.Y - halfHeight < 0)
                {
                    finalPosition.Y -= _position.Y - halfHeight;
                    changed = true;
                }
                else if (screenBottomRight.Y > levelHeight)
                {
                    finalPosition.Y -= screenBottomRight.Y - levelHeight;
                    changed = true;
                }
                if (changed)
                {
                    if (IsTransitionActive)
                        UpdateTransitionTarget(finalPosition);
                    else
                        Position = finalPosition;
                }
            }

            if(IsTransitionActive)
                updateTransition(gt);
        }

        public void StartFollow(Object target)
        {
            followTarget = target;
        }

        public void StopFollow()
        {
            followTarget = null;
        }
    }
}