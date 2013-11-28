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
        private int tileX, tileY;
        private Texture2D background;
        private ContentManager contentManager;

        public int getTileWidth() { return (background != null) ? background.Width : -1; }
        public int getTileHeight() { return (background != null) ? background.Height : -1; }

        public string Init(ContentManager cm, string tileFileName)
        {
            contentManager = cm;

            try
            {
                background = contentManager.Load<Texture2D>(tileFileName); // change these names to the names of your images
            }
            catch (Exception ex)
            {
                //TODO log
                return ex.Message;
            }

            return null;
        }
    }
}
