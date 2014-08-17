using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightningBug.UI
{
    public class UIManager
    {
        public class UIElementSorter : IComparer<DisplayRect>
        {
            public int Compare(DisplayRect c1, DisplayRect c2)
            {
                return c1.Depth.CompareTo(c2.Depth);
            }
        }

        List<DisplayRect> UIRects;
        List<Listbox> UIListBoxes;
        Texture2D background;
        string curScene;
        UIElementSorter sorter;

        // Hover & tooltip
        DisplayRect curHoverElement; // The element, if not null, that the mouse is hovering over.
        TimeSpan startHoverTime; // The time the hover on curHoverElement began
        TimeSpan toolTipHoverTime = TimeSpan.FromMilliseconds(3000); // 3000 ms
        DisplayRect toolTip;

        public UIManager()
        {
            UIRects = new List<UI.DisplayRect>();
            UIListBoxes = new List<Listbox>();
            curScene = string.Empty;
            sorter = new UIElementSorter();
            curHoverElement = null;
        }

        public void Load(ContentManager content, string newScene)
        {
            if (newScene == curScene)
                return;
            ClearScene();
            // Test scene code
            background = content.Load<Texture2D>("Art\\UI\\Backgrounds\\MainMenu");
            DisplayRect textureTest = new DisplayRect(content, "Art\\UI\\TestButton", new Vector2(200, 100), new Vector2(10, 10), ScreenPositions.TopRight);
            DisplayRect bothTest = new DisplayRect(content, "Art\\UI\\TestButton", new Vector2(200, 100), new Vector2(20, 20), ScreenPositions.TopLeft, "both temp bothtemp tempbothtemp2both2temp3both3 dkfjakiekjdf agqd");
            DisplayRect stringTest = new DisplayRect("stringstring1 string2 stringstringstringstring3 stringstringstring4 stringstringstring5 stringstring6 stringstring7", new Vector2(200, 100), new Vector2(10, 10), ScreenPositions.BottomLeft);
            //Listbox testListbox = new Listbox(new Vector2(500,0), new Vector2(150, 20));
            //both temp bothtemp tempbothtemp2both2temp3both3 dkfjakiekjdf agqd
            textureTest.Depth = 2;
            stringTest.Depth = 1;
            bothTest.Depth = 3;
            UIRects.Add(textureTest);
            UIRects.Add(stringTest);
            UIRects.Add(bothTest);
            UIRects.Sort(sorter);
            //UIListBoxes.Add(testListbox);
            //testListbox.AddItem("test1");
            //testListbox.AddItem("test2");
            // End Test
        }

        public void ClearScene()
        {
            UIRects.Clear();
        }

        // Returns true if the user selected an UI element.
        public bool HandleInput(ResolutionRenderer irr, GameTime gameTime, MouseState ms)
        {
            // Check to see if the mouse is hovering over any UI elements
            bool hovering = false;
            foreach (UI.DisplayRect dRect in UIRects)
            {
                if (dRect.IsPointInside(ms.Position))
                {
                    hovering = true;
                    // Set hover element.  If it's this element see if the hover time has passed the min.
                    if (curHoverElement != null && curHoverElement == dRect)
                    {
                        if (toolTip == null && gameTime.TotalGameTime - startHoverTime >= toolTipHoverTime)
                        {
                            string testHover = "Test Hover";
                            // Create hover element.
                            CreateTooltip(irr, ms.Position, testHover);
                        }
                    }
                    // If it's a new element, set that and reset the timer
                    else
                    {
                        curHoverElement = dRect;
                        startHoverTime = gameTime.TotalGameTime;
                        if (toolTip != null)
                        {
                            UIRects.Remove(toolTip);
                            toolTip = null;
                        }
                    }
                    break;
                }                
            }
            if (!hovering)
            {
                // Reset the hovering timer anytime the cursor isn't hovering over anything
                startHoverTime = gameTime.TotalGameTime;
                // If a tooltip is currently active and the cursor isn't hovering over anything destroy the active tooltip
                if (toolTip != null)
                {
                    UIRects.Remove(toolTip);
                    toolTip = null;
                }
            }

            // If there's a click, activate the appropriate UI element
            return false;
        }

        private void CreateTooltip(ResolutionRenderer irr, Point mousePos, String text)
        {
            // Find the length of the tooptip
            SpriteFont tempFont = Globals.gFonts["Miramonte"];
            if (tempFont == null)
                return;
            Vector2 tempSize = tempFont.MeasureString(text);
            // Find the distance from the current position to the right side of the screen and move the tooltip left to compensate
            float rightOfText = mousePos.X + tempSize.X;
            float xDiff = 0;
            if (rightOfText > irr.ScreenWidth)
                xDiff = rightOfText - irr.ScreenWidth;
            toolTip = new DisplayRect("Test Hover", new Vector2(200, 50), new Vector2(mousePos.X - xDiff, mousePos.Y), ScreenPositions.None);
            toolTip.Depth = 0;
            UIRects.Add(toolTip);
        }

        public void UpdateAll(ResolutionRenderer irr)
        {
            foreach (UI.DisplayRect dRect in UIRects)
            {
                if(dRect != null)
                    dRect.Update(irr);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(UI.DisplayRect dRect in UIRects)
            {
                if (dRect != null)
                    dRect.Draw(sb);
            }

            //foreach (UI.Listbox boxes in UIListBoxes)
            //{
            //    if (boxes != null)
            //        boxes.Draw(sb);
            //}            
        }
    }
}
