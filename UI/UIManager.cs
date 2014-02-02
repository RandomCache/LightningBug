using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        List<DisplayRect> UIObjects;
        Texture2D background;
        string curScene;
        UIElementSorter sorter;

        public UIManager()
        {
            UIObjects = new List<UI.DisplayRect>();
            curScene = string.Empty;
            sorter = new UIElementSorter();
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
            //both temp bothtemp tempbothtemp2both2temp3both3 dkfjakiekjdf agqd
            textureTest.Depth = 2;
            stringTest.Depth = 1;
            bothTest.Depth = 3;
            UIObjects.Add(textureTest);
            UIObjects.Add(stringTest);
            UIObjects.Add(bothTest);
            UIObjects.Sort(sorter);
            // End Test
        }

        public void ClearScene()
        {
            UIObjects.Clear();
        }

        public void UpdateAll(ResolutionRenderer irr)
        {
            foreach (UI.DisplayRect dRect in UIObjects)
            {
                dRect.Update(irr);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(UI.DisplayRect dRect in UIObjects)
            {
                dRect.Draw(sb);
            }
        }
    }
}
