using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class Level
    {
        private ContentManager contentManager;
        BackgroundTile[][][] backgrounds; //[num of layers][num of tiles in a row][num of tiles in a column]
        int[] backgroundRowTiles; // num tiles in the row for the given layer
        int[] backgroundRowColumns; // num tiles in the column for the given layer

        //LevelTile levelTiles[x][y]
        String levelName;
        int pxWidth, pxHeight;
        uint numBackgroundLayers, numRows, numColumns;
        bool isLevelLoaded = false;

        public bool IsLevelLoaded() { return isLevelLoaded; }

        public Level(ContentManager cm)
        {
            contentManager = cm;
        }

        public string LoadLevel()
        {
            isLevelLoaded = false;
            //TEMP TEST CODE
            numBackgroundLayers = 1; numRows = numColumns = 20;
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
            isLevelLoaded = true;
            return null;
        }

        public string UnloadLevel()
        {
            return null;
        }

        public string DrawLevel(SpriteBatch sb)
        {
            if (sb == null)
                return "Level.DrawLevel - Error: Null SpriteBatch\n";
            for (int i = 0; i < numBackgroundLayers; ++i)
            {
                for (uint x = 0; x < numColumns; ++x)
                {
                    for (uint y = 0; y < numRows; ++y)
                    {
                        sb.Draw(backgrounds[i][x][y].getTexture(), new Vector2(x * backgrounds[i][x][y].getTileWidth(),
                            y * backgrounds[i][x][y].getTileHeight()));
                    }
                }
            }
            return null;
        }
    }
}
