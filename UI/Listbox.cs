﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.UI
{
    public class Listbox : UIBase
    {
        UIManager uiManager;
        List<DisplayRect> mainList;
        Vector2 itemSize, maxSize;
        Vector2 position, positionOffset;
        DisplayRect selectedItem;
        int topItemDisplayed;
        ScreenPositions screenRelative;

        public Listbox(UIManager uim, Vector2 pos, Vector2 itemsize, Vector2 maxsize, ScreenPositions screenRelativePos = ScreenPositions.None)
        {
            uiManager = uim;
            mainList = new List<DisplayRect>();
            position = positionOffset = pos;
            itemSize = itemsize;
            maxSize = maxsize;
            selectedItem = null;
            screenRelative = screenRelativePos;
        }

        public void AddItem(string newItem)
        {
            Vector2 newPos = position;
            newPos.Y += itemSize.Y * mainList.Count;
            mainList.Add(new DisplayRect(newItem, itemSize, newPos, ScreenPositions.None));
            if (mainList.Count == 1)
                topItemDisplayed = 0;
        }

        public void Update(ResolutionRenderer irr)
        {
            // Get the position of the DisplayRect.  If it's not relative to the screen it's absolute position will be good now
            if(screenRelative != ScreenPositions.None)
                position = GetAbsolutePosition(irr);
            Vector2 newPos = position;
            int count = 0;
            // Start at the top item to be displayed.  When drawing, that's where Draw will start.
            for(int i = topItemDisplayed; i < mainList.Count; ++i)
            {
                newPos.Y = position.Y + (itemSize.Y * count);
                ++count;
                mainList[i].Update(irr, newPos);
            }
        }

        Vector2 GetAbsolutePosition(ResolutionRenderer irr)
        {
            Vector2 ret = Vector2.Zero;
            Vector2 totalSize = GetTotalSize();
            switch (screenRelative)
            {
                case ScreenPositions.TopLeft:
                    ret += positionOffset;
                    break;
                case ScreenPositions.TopCenter:
                    ret.X += irr.ScreenWidth / 2;
                    ret.X -= totalSize.X / 2;
                    ret += positionOffset;
                    break;
                case ScreenPositions.TopRight:
                    ret.X += irr.ScreenWidth;
                    ret.X -= totalSize.X;
                    ret += positionOffset;
                    break;
                case ScreenPositions.RightCenter:
                    ret.X += irr.ScreenWidth;
                    ret.X -= totalSize.X;
                    ret.Y += irr.ScreenHeight / 2;
                    ret.X += positionOffset.X;
                    ret.Y -= totalSize.Y / 2;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomRight:
                    ret.X += irr.ScreenWidth;
                    ret.X -= totalSize.X;
                    ret.X -= positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= totalSize.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomCenter:
                    ret.X += irr.ScreenWidth / 2;
                    ret.X -= totalSize.X / 2;
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= totalSize.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.BottomLeft:
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight;
                    ret.Y -= totalSize.Y;
                    ret.Y -= positionOffset.Y;
                    break;
                case ScreenPositions.LeftCenter:
                    ret.X += positionOffset.X;
                    ret.Y += irr.ScreenHeight / 2;
                    ret.Y -= totalSize.Y / 2;
                    ret.Y += positionOffset.Y;
                    break;
            };
            return ret;
        }

        public void Move()
        {
        }

        public void Draw(SpriteBatch sb)
        {
            bool haveStartedDrawing = false;
            // Start drawing with topItemDisplayed
            int count = 0;
            foreach (DisplayRect dRect in mainList)
            {
                if (topItemDisplayed != null && !haveStartedDrawing && count == topItemDisplayed)
                    haveStartedDrawing = true;
                if (haveStartedDrawing)
                {
                    if ((dRect.Position.X - position.X) + dRect.Size.X > maxSize.X ||
                        (dRect.Position.Y - position.Y) + dRect.Size.Y > maxSize.Y)
                    {
                        break;
                    }
                    /*if (dRect == selectedItem)
                    {

                    }*/
                    dRect.Draw(sb);
                }
                count++;
            } // foreach (DisplayRect dRect in mainList)
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
        public bool FindSelectedItem(Point point, out int numFound)
        {
            //newSelected = null;
            numFound = 0;
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
                        break;
                    }
                    numFound++;
                }
                return true;
            }

            return false;
        }

        // Scroll the list box the number of entries in count in the direction of up
        public void Scroll(bool up, int count)
        {
            if (mainList.Count <= 0)
                return;
            if (up)
            {
                topItemDisplayed -= count;
                if (topItemDisplayed < 0)
                    topItemDisplayed = 0;
            }
            else
            {
                topItemDisplayed += count;
                if (topItemDisplayed >= mainList.Count)
                    topItemDisplayed = mainList.Count - 1;
            }
        }

        public void ClearListBox()
        {
            UnSelect();
            mainList.Clear();
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

        Vector2 GetTotalSize()
        {
            Vector2 ret = new Vector2();
            ret.X = itemSize.X;
            ret.Y = itemSize.Y * mainList.Count;
            return ret;
        }

        public int GetNumItems()
        {
            if (mainList != null)
                return mainList.Count;
            else
                return 0;
        }
    }
}
