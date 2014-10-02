using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightningBug.Levels
{
    public class TileSet
    {
        public string fileName;
        public int elemWidth, elemHeight, itemsPerRow;
        public int numRows;
        public int[] rows; // used for loading the tileIds.
        public int[][] tileIds;
    }
}
