using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        BackgroundTile[][][] backgrounds; //[num of layers][num of tiles in a row][num of tiles in a column]
        int[] backgroundRowTiles; // num tiles in the row for the given layer
        int[] backgroundRowColumns; // num tiles in the column for the given layer

        //LevelTile levelTiles[x][y]
        String levelName;
        int pxWidth, pxHeight;

        public string LoadLevel()
        {
            return null;
        }

        public string UnloadLevel()
        {
            return null;
        }

        public string DrawLevel()
        {
            return null;
        }
    }
}
