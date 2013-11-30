using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug
{
    class BackgroundTile
    {
        private uint tileX, tileY; //TODO: Evaluate if these are necessary
        private Texture2D background;

        public int getTileWidth() { return (background != null) ? background.Width : -1; }
        public int getTileHeight() { return (background != null) ? background.Height : -1; }
        public Texture2D getTexture() { return background; }

        public BackgroundTile(ContentManager cm, string tileFileName, uint x, uint y)
        {
            try
            {
                background = cm.Load<Texture2D>(tileFileName); // change these names to the names of your images
            }
            catch (Exception ex)
            {
                //TODO log
                throw ex;
            }

            tileX = x;
            tileY = y;
        }

        public BackgroundTile(Texture2D twoDTexture, uint x, uint y)
        {
            background = twoDTexture;
            tileX = x;
            tileY = y;
        }
    }
}
