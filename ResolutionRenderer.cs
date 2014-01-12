//////////////////////////////////////////////////////////////////////////
////License:  The MIT License (MIT)
////Copyright (c) 2010 David Amador (http://www.david-amador.com)
////
////Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
////
////The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
////
////THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//////////////////////////////////////////////////////////////////////////
/*
 MODIFIED & REWORKED BY PANTHER(http://www.panthernet.ru)
 - Added support for all aspect ratios
 - Added resource 
 - Simplified usage
 */


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug
{
    public class ResolutionRenderer: IDisposable
    {
        private readonly Game _game;
        public Viewport Viewport { get; protected set; }
        /// <summary>
        /// Ratio between real and virtual screens width
        /// </summary>
        private float _ratioX;
        /// <summary>
        /// Ratio between real and virtual screens height
        /// </summary>
        private float _ratioY;

        private static Matrix _scaleMatrix;
        /// <summary>
        /// Indicates that matrix update is needed
        /// </summary>
        private bool _dirtyMatrix = true;
        /// <summary>
        /// Game BG color
        /// </summary>
        public Color BackgroundColor = Color.Black;
        /// <summary>
        /// Not implemented
        /// </summary>
        public Color ViewportColor = Color.LightSkyBlue;

        /// <summary>
        /// Gets virtual screen center
        /// </summary>
        public Vector2 VirtualScreenCenter
        {
            get { return new Vector2(VirtualWidth /2, VirtualHeight / 2); }
        }

        /// <summary>
        /// Gets or sets virtual screen height
        /// </summary>
        public int VirtualHeight;
        /// <summary>
        /// Gets or set virtual screen width
        /// </summary>
        public int VirtualWidth;
        /// <summary>
        /// Gets or sets real screen width
        /// </summary>
        public int ScreenWidth;
        /// <summary>
        /// Gets or sets real screen height
        /// </summary>
        public int ScreenHeight;

        public ResolutionRenderer(Game game, int virtualWidth, int virtualHeight, int realWidth, int realHeight)
        {
            _game = game;
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;

            ScreenWidth = realWidth;
            ScreenHeight = realHeight;
            initialize();
        }

        /// <summary>
        /// Initializes resolution renderer and marks it for refresh
        /// </summary>
        private void initialize()
        {
            SetupVirtualScreenViewport();
            //calculate new ratio
            _ratioX = (float)Viewport.Width / VirtualWidth;
            _ratioY = (float)Viewport.Height / VirtualHeight;
            //mark for refresh
            _dirtyMatrix = true;
        }

        /// <summary>
        /// Setup viewport to real screen size
        /// </summary>
        public void SetupFullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = ScreenWidth;
            vp.Height = ScreenHeight;
            _game.GraphicsDevice.Viewport = vp;
            _dirtyMatrix = true;
        }

        /// <summary>
        /// Draw call
        /// </summary>
        public void Draw()
        {
            //set full viewport
            SetupFullViewport();
            //clear screen with BG color
            _game.GraphicsDevice.Clear(BackgroundColor);
            //set virtual viewport
            SetupVirtualScreenViewport();
        }

        /// <summary>
        /// Get modified matrix for sprite rendering
        /// </summary>
        public Matrix GetTransformationMatrix()
        {
            if (_dirtyMatrix)
                recreateScaleMatrix();

            return _scaleMatrix;
        }

        private void recreateScaleMatrix()
        {
            if(!_invertScale)
                Matrix.CreateScale((float)ScreenWidth / VirtualWidth, (float)ScreenWidth / VirtualWidth, 1f, out _scaleMatrix);
            else Matrix.CreateScale((float)ScreenHeight / VirtualHeight, (float)ScreenHeight / VirtualHeight, 1f, out _scaleMatrix);
            _dirtyMatrix = false;
        }

        bool _invertScale = false;
        public void SetupVirtualScreenViewport()
        {
            var targetAspectRatio = VirtualWidth / (float)VirtualHeight;
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            var width = ScreenWidth;
            var height = (int)(width / targetAspectRatio + .5f);

            if (height > ScreenHeight)
            {
                _invertScale = true;
                height = ScreenHeight;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
            }
            else _invertScale = false;

            // set up the new viewport centered in the backbuffer
            Viewport = new Viewport
            {
                X = (ScreenWidth / 2) - (width / 2),
                Y = (ScreenHeight / 2) - (height / 2),
                Width = width,
                Height = height
            };

            _game.GraphicsDevice.Viewport = Viewport;
        }

        /// <summary>
        /// Converts screen coordinates to virtual coordinates
        /// </summary>
        /// <param name="screenPosition">Screen coordinates</param>
        public Vector2 ToVirtual(Vector2 screenPosition)
        {
            var realX = screenPosition.X - Viewport.X;
            var realY = screenPosition.Y - Viewport.Y;

            return new Vector2(realX / _ratioX, realY / _ratioY);
        }

        /// <summary>
        /// Converts screen coordinates to virtual coordinates
        /// </summary>
        /// <param name="screenPosition">Screen coordinates</param>
        public Point ToVirtual(Point screenPosition)
        {
            var realX = screenPosition.X - Viewport.X;
            var realY = screenPosition.Y - Viewport.Y;

            return new Point((int)(realX / _ratioX), (int)(realY / _ratioY));
        }

        /// <summary>
        /// Optional dispose routine
        /// </summary>
        public void Dispose()
        {
        }

        ~ResolutionRenderer()
        {
            Dispose();
        }

    }
}
