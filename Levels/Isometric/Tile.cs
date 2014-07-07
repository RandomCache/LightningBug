using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.Isometric
{
    static class Tile
    {
        static public Texture2D TileSetTexture;
        static public int TileWidth = 64;
        static public int TileHeight = 64;
        static public int TileStepX = 64;
        static public int TileStepY = 16;
        static public int OddRowXOffset = 32;
        static public int HeightTileOffset = 32;

        static public Rectangle GetSourceRectangle(int tileIndex)
        {
            int numInRow = TileSetTexture.Width / TileWidth;
            return new Rectangle((tileIndex % numInRow) * TileWidth, (tileIndex / numInRow) * TileHeight,
                TileWidth, TileHeight);
        }
    }
}
