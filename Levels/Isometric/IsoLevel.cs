using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.Levels
{
    public class IsoLevel
    {
        private ContentManager contentManager;
        private Isometric.TileMap myMap;
        int squaresAcross; //TODO remove? myMap.MapWidth
        int squaresDown; //TODO remove? myMap.MapHeight
        int baseOffsetX;
        int baseOffsetY;
        float heightRowDepthMod;
        string levelName;

#region Properties
        public Isometric.TileMap Tiles
        {
            get { return myMap; }
            set { myMap = value; }
        }

        public int TilesWide
        {
            get { return squaresAcross; }
        }

        public int TilesHigh
        {
            get { return squaresDown; }
        }

        public int BaseOffsetX
        {
            get { return baseOffsetX; }
            set { baseOffsetX = value; }
        }

        public int BaseOffsetY
        {
            get { return baseOffsetY; }
            set { baseOffsetY = value; }
        }

        public float HeightRowDepthMod
        {
            get { return heightRowDepthMod; }
            set { heightRowDepthMod = value; }
        }

        public string LevelName
        {
            get { return levelName; }
        }
#endregion

        public IsoLevel(ContentManager cm)
        {
            contentManager = cm;
        }

        public bool LoadLevel(XDocument xDoc, ref Vector2 startingCenterScreenPos)
        {
            // temp test
            squaresAcross = 17;
            squaresDown = 37;
            baseOffsetX = baseOffsetY = -14;
            heightRowDepthMod = 0.0000001f;

            // Load the tiles from the level file.
            XElement curElement, childElement;
            curElement = xDoc.Root.Element("BasicInfo");
            levelName = curElement.Element("Name").Value;
            if (!Int32.TryParse(curElement.Element("Width").Value, out myMap.MapWidth) || !Int32.TryParse(curElement.Element("Height").Value, out myMap.MapHeight))
                return false;
            childElement = curElement.Element("StartingScreenPos");
            startingCenterScreenPos.X = uint.Parse(childElement.Element("X").Value);
            startingCenterScreenPos.Y = uint.Parse(childElement.Element("Y").Value);

            curElement = xDoc.Root.Element("Tiles");
            int row, column, id;
            foreach (XElement curTile in curElement.Descendants("Tile"))
            {
                if (!Int32.TryParse(curTile.Element("Row").Value, out row) ||
                    !Int32.TryParse(curTile.Element("Column").Value, out column) ||
                    !Int32.TryParse(curTile.Element("Id").Value, out id))
                    return false;
                //myMap.Rows[row].Columns[column].TileID = id;
                myMap.Rows[row].Columns[column].BaseTile = id;
            }
            foreach (XElement curTile in curElement.Descendants("HeightTile"))
            {
                if (!Int32.TryParse(curTile.Element("Row").Value, out row) ||
                    !Int32.TryParse(curTile.Element("Column").Value, out column) ||
                    !Int32.TryParse(curTile.Element("Id").Value, out id))
                    return false;
                myMap.Rows[row].Columns[column].AddHeightTile(id);
            }

            return true;
        }
    }
}
