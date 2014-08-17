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
        UIManager uiManager;
        List<DisplayRect> mainList;
        Vector2 itemSize;
        Vector2 position;
        DisplayRect selectedItem;

        public Listbox(UIManager uim, Vector2 pos, Vector2 itemsize)
        {
            uiManager = uim;
            mainList = new List<DisplayRect>();
            position = pos;
            itemSize = itemsize;
            selectedItem = null;
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
            {
                if (dRect == selectedItem)
                {

                }
                dRect.Draw(sb);
            }
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

        //public bool FindSelectedItem(Point point, out DisplayRect newSelected)
        public bool FindSelectedItem(Point point)
        {
            //newSelected = null;
            float height = itemSize.Y * mainList.Count;
            if (point.X >= position.X && point.X <= position.X + itemSize.X &&
                point.Y >= position.Y && point.Y <= position.Y + height)
            {
                // The point is in the listbox, now to find the item selected.
                foreach (DisplayRect dRect in mainList) // It's a list so I have to run through them all anyway... for now
                {
                    if (dRect.IsPointInside(point))
                    {
                        //newSelected = dRect;
                        Select(dRect);
                    }
                }
                return true;
            }

            return false;
        }

        public void Select(DisplayRect dRect)
        {
            UnSelect();
            selectedItem = dRect;
            selectedItem.CreateSolidBackgroundTexture(uiManager.GetGraphics("Listbos::Select - Calling CreateSolidBackgroundTexture()"), Color.Violet);
        }

        public void UnSelect()
        {
            if (selectedItem != null)
                selectedItem.RemoveSolidBackgroundTexture();
            selectedItem = null;
        }
    }
}
