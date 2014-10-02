using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.UI
{
    // Position of element relative to a spot on the screen, none to use just an absolute position
    public enum ScreenPositions
    {
        TopLeft, TopCenter, TopRight, RightCenter,
        BottomRight, BottomCenter, BottomLeft, LeftCenter, None
    };
    // UI Rectangle used for displaying a texture, a string, or both.
    public class DisplayRect
    {
        string displayString;
        Vector2 positionOffset, position, size, scale;
        float depth; // Used for draw order. 0 = On Top - 1 = At the Back
        protected Texture2D texture;
        SpriteFont font;
        ScreenPositions screenRelative;
        Color textColor;//, bgColor;

        public float Depth { get { return depth; } set { depth = value; } }

        public Vector2 Position { get { return position; } set { position = value; } }

        public Vector2 PositionOffset { get { return positionOffset; } set { positionOffset = value; } }

        public Vector2 Size { get { return size; } set { size = value; } }

        public DisplayRect(ContentManager content, string texturePath, Vector2 endSize, Vector2 offset, ScreenPositions screenRelativePos = ScreenPositions.None, string str = null, float pDepth = 1.0f)
        {
            if(texturePath != string.Empty)
                texture = content.Load<Texture2D>(texturePath);
            positionOffset = offset;
            displayString = str;
            screenRelative = screenRelativePos;
            size = endSize;
            font = Globals.gFonts["Miramonte"];
            if (texture != null)
                scale = new Vector2(size.X / texture.Width, size.Y / texture.Height);
            else
                scale = Vector2.One;
            textColor = Color.Green;
            //bgColor = Color.Transparent;
            depth = pDepth;

            if(displayString != null && displayString != string.Empty)
                displayString = Text.WrapText(font, displayString, size.X);
            if (screenRelativePos == ScreenPositions.None)
                position = offset;
        }

        public DisplayRect(string str, Vector2 endSize, Vector2 offset, ScreenPositions posRelative = ScreenPositions.None, float pDepth = 1.0f)
            : this(null, string.Empty, endSize, offset, posRelative, str, pDepth)
        {
        }

        Vector2 GetAbsolutePosition(ResolutionRenderer irr)
        {
            Vector2 ret = Vector2.Zero;
            switch (screenRelative)
            {
                case ScreenPositions.TopLeft:
                    ret += positionOffset;
                    break;
                case ScreenPositions.TopCenter:
                    ret.X += irr.ScreenWidth / 2;
                    ret.X -= size.X / 2;
                    ret += positionOffset;
                    break;
                case ScreenPositions.TopRight:
                    ret.X += irr.ScreenWidth;
                    ret.X -= size.X;
                    ret += positionOffset;
                    break;
                case ScreenPositions.RightCenter:
                    ret.X += irr.ScreenWidth;
                    ret.X -= size.X;
                    ret.Y += irr.ScreenHeight / 2;
                    ret.X += positionOffset.X;
                    ret.Y -= size.Y / 2;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomRight:
                    ret.X += irr.ScreenWidth;
                    ret.X -= size.X;
                    ret.X -= positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= size.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomCenter:
                    ret.X += irr.ScreenWidth / 2;
                    ret.X -= size.X / 2;
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= size.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomLeft:
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= size.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.LeftCenter:
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight / 2;
                    ret.Y -= size.Y / 2;
                    ret.Y += positionOffset.Y;
                    break;
            };
            return ret;
        }

        public void Update(ResolutionRenderer irr, Vector2? newOffset = null, Vector2? newSize = null)
        {
            bool changed = false;
            if (newOffset != null)
            {
                positionOffset.X = newOffset.Value.X;
                positionOffset.Y = newOffset.Value.Y;
                changed = true;
            }
            if (newSize != null)
            {
                size.X = newSize.Value.X;
                size.Y = newSize.Value.Y;
                changed = true;
            }
            if (displayString != null && displayString != string.Empty && changed)
            {
                displayString = Text.WrapText(font, displayString, size.X);
            }

            // Get the position of the DisplayRect.  If it's not relative to the screen it's absolute position will be good now
            if (screenRelative != ScreenPositions.None)
                position = GetAbsolutePosition(irr);
            else
                position = positionOffset;
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw the texture if it's there, otherwise draw the background color
            if (texture != null)
                sb.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, depth);
            /*else
            {
                sb.Draw(
            }*/
            if (displayString != null && displayString != string.Empty)
            {
                sb.DrawString(font, displayString, position, textColor);
            }
        }

        public bool IsPointInside(Point point)
        {
            if (point.X >= position.X && point.X <= position.X + size.X &&
                point.Y >= position.Y && point.Y <= position.Y + size.Y)
            {
                return true;
            }
             
            return false;
        }

        // Used to draw a solid color background
        public void CreateSolidBackgroundTexture(GraphicsDevice gd, Color bgColor)
        {
            //Texture2D rect = new Texture2D(graphics.GraphicsDevice, 80, 30);
            if (texture == null)
                texture = new Texture2D(gd, (int)size.X, (int)size.Y);
            Color[] data = new Color[(int)(size.X * size.Y)];
            for (int i = 0; i < data.Length; ++i)
                data[i] = bgColor;
            texture.SetData(data);
        }

        public void RemoveSolidBackgroundTexture()
        {
            texture = null;
        }
    }
}