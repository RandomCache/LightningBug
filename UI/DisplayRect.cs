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
        Vector2 positionOffset, absPosition, size, scale;
        uint depth; // Used for draw order. 0 = On Top
        protected Texture2D texture;
        SpriteFont font;
        ScreenPositions screenRelative;
        Color color; // TODO Use Color

        public uint Depth { get { return depth; } set { depth = value; } }

        public DisplayRect(ContentManager content, string texturePath, Vector2 endSize, Vector2 offset, ScreenPositions scrennRelativePos = ScreenPositions.TopLeft, string str = null)
        {
            if(texturePath != string.Empty)
                texture = content.Load<Texture2D>(texturePath);
            positionOffset = offset;
            displayString = str;
            screenRelative = scrennRelativePos;
            size = endSize;
            font = Globals.gFonts["Miramonte"];
            if (texture != null)
                scale = new Vector2(size.X / texture.Width, size.Y / texture.Height);
            else
                scale = Vector2.One;
            color = Color.Green;

            if(displayString != null && displayString != string.Empty)
                displayString = Text.WrapText(font, displayString, size.X);
            if (scrennRelativePos == ScreenPositions.None)
                absPosition = offset;
        }

        public DisplayRect(string str, Vector2 endSize, Vector2 offset, ScreenPositions posRelative = ScreenPositions.TopLeft)
            : this(null, string.Empty, endSize, offset, posRelative, str)
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
            if(screenRelative != ScreenPositions.None)
                absPosition = GetAbsolutePosition(irr);
        }

        public void Draw(SpriteBatch sb)
        {
            if (texture != null)
                sb.Draw(texture, absPosition, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (displayString != null && displayString != string.Empty)
            {
                sb.DrawString(font, displayString, absPosition, color);
            }
        }

        public bool IsPointInside(Point point)
        {
            if (point.X >= absPosition.X && point.X <= absPosition.X + size.X &&
                point.Y >= absPosition.Y && point.Y <= absPosition.Y + size.Y)
            {
                return true;
            }
             
            return false;
        }
    }
}