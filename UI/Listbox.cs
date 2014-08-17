using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.UI
{
    public class Listbox
    {
        List<DisplayRect> mainList;
        Vector2 itemSize;
        Vector2 position;

        public Listbox(Vector2 pos, Vector2 itemsize)
        {
            mainList = new List<DisplayRect>();
            position = pos;
            itemSize = itemsize;
        }

        public void AddItem(string newItem)
        {
            Vector2 newPos = position;
            newPos.Y += itemSize.Y * mainList.Count;
            mainList.Add(new DisplayRect(newItem, itemSize, newPos, ScreenPositions.None));
        }

        public void Move()
        {
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (DisplayRect dRect in mainList)
                dRect.Draw(sb);
        }

        public bool IsPointInside(Point point)
        {
            float height = itemSize.Y * mainList.Count;
            if (point.X >= position.X && point.X <= position.X + itemSize.X &&
                point.Y >= position.Y && point.Y <= position.Y + height)
            {
                return true;
            }

            return false;
        }
    }
}
