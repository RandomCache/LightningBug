using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
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
        public Vector2 SelectedCell { get; set; }

        //public int[][] curTileSetIds;
        public TileSet selectedTileSet;

        public IsoEditor(UIManager ui)
        {
            uiManager = ui;
            SelectedCell = Vector2.Zero;
        }

        public UIManager GetUIManager()
        {
            return uiManager;
        }
    }
}
