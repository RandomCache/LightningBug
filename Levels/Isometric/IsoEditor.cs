using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightningBug.UI;

namespace LightningBug.Levels
{
    public class IsoEditor
    {
        /*
        struct TileType
        {
            public string file;
            public int tileId;
        }
        public Dictionary<string, int> tileTypes; // file, tile id
        */
        // Starting with just the id
//        int tileId
        UIManager uiManager;

        public IsoEditor(UIManager ui)
        {
            uiManager = ui;
        }

        public UIManager GetUIManager()
        {
            return uiManager;
        }
    }
}
