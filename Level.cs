using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
/*
 * Level and Tile psuedocode
 
Level Data Structure
BackgroundTile Backgrounds[num of layers][x][y]
LevelTile levelTiles[x][y]
String levelName

BackgroundTile
Image*
TileSize

Level Tile Structure
TileSize
Objects*[]
*/

namespace LightningBug
{
    public class Level
    {
        private ContentManager contentManager;
        BackgroundTile[][][] backgrounds; //[num of layers][num of tiles in a row][num of tiles in a column]
        uint[] backgroundRowTiles; // num tiles in the row for the given layer
        uint[] backgroundRowColumns; // num tiles in the column for the given layer

        //LevelTile levelTiles[x][y]
        String levelName;
        uint pxWidth, pxHeight, startScreenCenterX, startScreenCenterY;
        uint numBackgroundLayers;//, numRows, numColumns;
        bool isLevelLoaded = false;

        public bool IsLevelLoaded() { return isLevelLoaded; }

        public uint GetLevelWidth() { return pxWidth; }
        public uint GetLevelHeight() { return pxHeight; }

        public Level(ContentManager cm)
        {
            contentManager = cm;
        }

        public string LoadLevel(string fileName, ref Vector2 startingCenterScreenPos)
        {
            isLevelLoaded = false;
            try
            {
                XDocument xDoc = XDocument.Load(fileName);
                XElement curElement, childElement;
                curElement = xDoc.Root.Element("BasicInfo");
                levelName = curElement.Element("Name").Value;
                pxWidth = uint.Parse(curElement.Element("Width").Value);
                pxHeight = uint.Parse(curElement.Element("Height").Value);
                childElement = curElement.Element("StartingScreenPos");
                startingCenterScreenPos.X = uint.Parse(childElement.Element("X").Value);
                startingCenterScreenPos.Y = uint.Parse(childElement.Element("Y").Value);

                curElement = xDoc.Root.Element("Backgrounds");
                numBackgroundLayers = (uint)curElement.Attribute("NumLayers");
                backgroundRowTiles = new uint[numBackgroundLayers];
                backgroundRowColumns = new uint[numBackgroundLayers];
                backgrounds = new BackgroundTile[numBackgroundLayers][][];
                if (numBackgroundLayers > 0)
                {
                    IEnumerable<XElement> bgList = curElement.Elements("Background");
                    foreach (XElement bgElem in bgList)
                    {
                        uint layerNum, numRows, numColumns;
                        string texturePath;
                        layerNum = uint.Parse(bgElem.Element("Layer").Value);
                        texturePath = bgElem.Element("TexturePath").Value;
                        numColumns = uint.Parse(bgElem.Element("NumColumns").Value);
                        numRows = uint.Parse(bgElem.Element("NumRows").Value);
                        Texture2D tempTexture = contentManager.Load<Texture2D>(texturePath);
                        backgroundRowTiles[layerNum] = numColumns;
                        backgroundRowColumns[layerNum] = numRows;
                        //Initialize the tiles for this layer
                        backgrounds[layerNum] = new BackgroundTile[numColumns][];
                        for (uint x = 0; x < numColumns; ++x)
                        {
                            backgrounds[layerNum][x] = new BackgroundTile[numRows];
                            for (uint y = 0; y < numRows; ++y)
                            {
                                backgrounds[layerNum][x][y] = new BackgroundTile(tempTexture, x, y);
                            }
                        }
                    }
                } // if (numBackgroundLayers > 0)
                // TODO verify starting position is in level
                // TODO verify total level width and height are greater than our max resolution
            }
            catch (Exception ex)
            {
                //TODO log
                //Error loading the level
                //TODO show messagebox
                Environment.Exit(0);
            }


            bool readResult = false;
            /*
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                while(reader.Read() == true)
                {
                    switch(reader.Name)
                    {
                        case "LightningBugLevel":
                            break;
                        case "BasicInfo":
                            if(reader.Read())//<Name>
                            {
                                levelName = reader.Value;
                            }
                            break;
                    }
                }
            }
            */
            /*
            isLevelLoaded = false;
            //TEMP TEST CODE
            numBackgroundLayers = 1;
            //uint numRows, numColumns;
            numRows = numColumns = 20;
            Texture2D tempStarfield = contentManager.Load<Texture2D>("Art\\Backgrounds\\Starfield");
            backgrounds = new BackgroundTile[numBackgroundLayers][][];
            //END TEMP TEST CODE
            //Load the level info from a file
            for (int i = 0; i < numBackgroundLayers; ++i)
            {
                backgrounds[i] = new BackgroundTile[numColumns][];
                //TODO variable to tell if the tiles for this layer all have the same image
                //Initialize the tiles for this layer
                for (uint x = 0; x < numColumns; ++x)
                {
                    backgrounds[i][x] = new BackgroundTile[numRows];
                    for (uint y = 0; y < numRows; ++y)
                    {
                        backgrounds[i][x][y] = new BackgroundTile(tempStarfield, x, y);
                    }
                }
            }
            */
            isLevelLoaded = true;
            return null;
        }

        public string UnloadLevel()
        {
            return null;
        }

        //public string DrawLevel(SpriteBatch sb, Rectangle curScreen)
        public string DrawLevel(SpriteBatch sb, Vector2 curScreenPos, Vector2 curScreenDimensions)
        {
            if (sb == null)
                return "Level.DrawLevel - Error: Null SpriteBatch\n";
            uint numColumns, numRows;
            int curTileWidth = 0, curTileHeight = 0;
            for (int i = 0; i < numBackgroundLayers; ++i)
            {
                numColumns = backgroundRowTiles[i];
                numRows = backgroundRowColumns[i];
                if(numColumns > 0 && numRows > 0)
                {
                    curTileWidth = backgrounds[i][0][0].getTileWidth();
                    curTileHeight = backgrounds[i][0][0].getTileHeight();
                }
                // TODO don't run through the tiles not near the screen
                for (uint x = 0; x < numColumns; ++x)
                {
                    for (uint y = 0; y < numRows; ++y)
                    {
                        //Is this tile in the screen?
                        /* Temp commented out changing curScreen rect to vectors
                        if ((((x + 1) * curTileWidth) < curScreen.X || (x * curTileWidth) > curScreen.X + curScreen.Width) &&
                            (((y + 1) * curTileHeight) < curScreen.Y || (y * curTileHeight) > curScreen.Y + curScreen.Height))
                            continue;
                        */

                        if ((((x + 1) * curTileWidth) < curScreenPos.X || (x * curTileWidth) > curScreenPos.X + curScreenDimensions.X) &&
                            (((y + 1) * curTileHeight) < curScreenPos.Y || (y * curTileHeight) > curScreenPos.Y + curScreenDimensions.Y))
                            continue;

                        //Translate the tiles world coordinates to scren coordinates
                        // World = (x * curTileWidth, y * curTileHeight)
                        // Screen = curscreen - world
                        /* Temp commented out changing curScreen rect to vectors
                        sb.Draw(backgrounds[i][x][y].getTexture(), new Vector2((x * curTileWidth) - curScreen.X,
                            (y * curTileHeight) - curScreen.Y));
                        */
                        sb.Draw(backgrounds[i][x][y].getTexture(), new Vector2((x * curTileWidth) - curScreenPos.X,
                            (y * curTileHeight) - curScreenPos.Y));
                    }
                }
            }
            return null;
        }
    }
}
