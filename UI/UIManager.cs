﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightningBug.Isometric;
using LightningBug.Levels;

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
        GraphicsDevice graphicsDevice;
        UIBase curSelected;

        // Hover & tooltip
        DisplayRect curHoverElement; // The element, if not null, that the mouse is hovering over.
        TimeSpan startHoverTime; // The time the hover on curHoverElement began
        TimeSpan toolTipHoverTime = TimeSpan.FromMilliseconds(3000); // 3000 ms
        DisplayRect toolTip;

        // Editor elements
        Listbox tileSetListbox; // show all the tile sets
        Listbox tileTypeListbox; // show the tiles from the current tile set
        int selectedCellType;
        // Managers used for level type data when editing
        SpaceManager spaceManager;
        IsoManager isoManager;

        public int SelectedCellType
        {
            get { return selectedCellType; }
            set { selectedCellType = value; }
        }

        public UIManager(GraphicsDevice gd, SpaceManager sm, IsoManager im)
        {
            graphicsDevice = gd;
            UIRects = new List<UI.DisplayRect>();
            UIListBoxes = new List<Listbox>();
            curScene = string.Empty;
            sorter = new UIElementSorter();
            curHoverElement = null;
            selectedCellType = 6;
            spaceManager = sm;
            isoManager = im;
            curSelected = null;
        }

        public void Load(ContentManager content, string newScene, GameMode gameMode, LevelType levelType)
        {
            // TODO load UI from a file or get rid of newScene.  May just keep it simple and hardcoded to save dev time
            if (newScene == curScene)
                return;
            ClearScene();
            // Test scene code
            if (gameMode == GameMode.Main)
            {                
                background = content.Load<Texture2D>("Art\\UI\\Backgrounds\\MainMenu");
                //DisplayRect textureTest = new DisplayRect(content, "Art\\UI\\TestButton", new Vector2(200, 100), new Vector2(-00, 10), ScreenPositions.TopRight);
                DisplayRect bothTest = new DisplayRect(content, "Art\\UI\\TestButton", new Vector2(200, 100), new Vector2(20, 20), ScreenPositions.TopLeft, "both temp bothtemp tempbothtemp2both2temp3both3 dkfjakiekjdf agqd");
                DisplayRect stringTest = new DisplayRect("stringstring1 string2 stringstringstringstring3 stringstringstring4 stringstringstring5 stringstring6 stringstring7", new Vector2(200, 100), new Vector2(10, 10), ScreenPositions.BottomLeft);
                //both temp bothtemp tempbothtemp2both2temp3both3 dkfjakiekjdf agqd
                //textureTest.Depth = 0.2f;
                stringTest.Depth = 0;
                bothTest.Depth = 0.3f;
                //UIRects.Add(textureTest);
                UIRects.Add(stringTest);
                UIRects.Add(bothTest);
                UIRects.Sort(sorter);
            }
            else
            {
                tileSetListbox = new Listbox(this, new Vector2(-10, 200), new Vector2(150, 20), new Vector2(150, 100), ScreenPositions.TopRight);
                UIListBoxes.Add(tileSetListbox);
                tileTypeListbox = new Listbox(this, new Vector2(-10, 300), new Vector2(150, 20), new Vector2(150, 200), ScreenPositions.TopRight);
                UIListBoxes.Add(tileTypeListbox);
                SetupEditorTileBoxes();
                
                DisplayRect editorSelectBackground = new DisplayRect(content, "Art\\UI\\Editor\\TestItemBackground", new Vector2(100, 100), new Vector2(-10, 10), ScreenPositions.TopRight);
                UIRects.Add(editorSelectBackground);
            }
            // End Test
        }

        public void ClearScene()
        {
            UIRects.Clear();
            UIListBoxes.Clear();
        }

        // Returns true if the user selected an UI element.
        public bool HandleInput(ResolutionRenderer irr, GameTime gameTime, MouseState ms, KeyboardState ks, MouseState prevMs, KeyboardState prevKs)
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
            } // End of hovering
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
            if (ms.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed)
            {
                int findReturn = -1;
                foreach (UI.Listbox box in UIListBoxes)
                {
                    /*if (box.FindSelectedItem(ms.Position))
                    {
                    }*/
                    
                    if(box.FindSelectedItem(ms.Position, out findReturn))
                    {
                        // This is the list box the user selects their currently selected tile type.
                        if(box == tileTypeListbox)
                        {                            
                            selectedCellType = findReturn;
                            curSelected = box;
                        }
                    }
                }
            } // if (ms.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed)
            
            // If a list box is selected and the user presses the page down or up keys to scroll
            if(UIListBoxes.Count > 0)
            {
                if(ks.IsKeyUp(Keys.PageDown) && prevKs.IsKeyDown(Keys.PageDown))
                {
                    if (curSelected.GetType() == UIListBoxes.First().GetType())
                    {
                        ((Listbox)curSelected).Scroll(false, 3);
                    }
                }
                if(ks.IsKeyUp(Keys.PageUp) && prevKs.IsKeyDown(Keys.PageUp))
                {
                    if (curSelected.GetType() == UIListBoxes.First().GetType())
                    {
                        ((Listbox)curSelected).Scroll(true, 3);
                    }
                }
            }
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
            foreach (UI.Listbox box in UIListBoxes)
            {
                if (box != null)
                    box.Update(irr);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(UI.DisplayRect dRect in UIRects)
            {
                if (dRect != null)
                    dRect.Draw(sb);
            }
            
            foreach (UI.Listbox box in UIListBoxes)
            {
                if (box != null)
                    box.Draw(sb);
            }
        }

        // Setup the tileSetListbox and tileTypeListbox.  tileSetListbox needs to contain all the tile sets and 
        // tileTypeListbox needs to contain the tile sof the selected tile set.  The first one will be the default
        public void SetupEditorTileBoxes()
        {
            if(isoManager == null)
                return;
            Dictionary<string, TileSet> tileSetList = isoManager.GetTileSets();
            if (tileSetList.Count <= 0)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("UIManager::SetupEditorTileBoxes - No tile sets\n");
                return;
            }
            foreach (KeyValuePair<string, TileSet> tileSet in tileSetList)
            {
                tileSetListbox.AddItem(tileSet.Value.fileName);
            }
            // Default the selection to the first tileSet
            isoManager.isoEditor.selectedTileSet = tileSetList.First().Value;                        
                
            // Now setup tileTypeListbox with each tile type of the current set.
            // Fill with row and column for each item in this tileSet
            //isoManager.isoEditor.curTileSetIds
            for(int row = 0; row < isoManager.isoEditor.selectedTileSet.tileIds.Length; ++row)
            {
                for (int col = 0; col< isoManager.isoEditor.selectedTileSet.tileIds[row].Length; ++col)
                {
                    tileTypeListbox.AddItem(row + ", " + col);
                    //todo
                }
            }
        }

        public GraphicsDevice GetGraphics(string caller)
        {
            Logging.Instance(Logging.DEFAULTLOG).Log("UIManager::GetGraphics - Called from. " + caller + "\n");
            return graphicsDevice;
        }
    }
}
