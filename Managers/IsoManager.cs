using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightningBug
{
    public class IsoManager
    {
        Isometric.TileMap myMap;
        int squaresAcross;
        int squaresDown;
        int baseOffsetX;
        int baseOffsetY;
        float heightRowDepthMod;
        Texture2D hilight;
        Levels.IsoLevel curLevel;

        public IsoManager()
        {
            curLevel = null;
        }

        public void Initialize()
        {            
            squaresAcross = 17;
            squaresDown = 37;
            baseOffsetX = baseOffsetY = -14;
            heightRowDepthMod = 0.0000001f;
        }

        public bool LoadIsoContent(GraphicsDeviceManager gdm, ContentManager cm)
        {
            Isometric.Tile.TileSetTexture = cm.Load<Texture2D>(@"Textures\TileSets\temp_groung_tileset_01.png");
            hilight = cm.Load<Texture2D>(@"Textures\TileSets\hilight");
            myMap = new Isometric.TileMap();

            if (gdm == null || myMap == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadIsoContent - No graphics device manager or myMap. \n");
                return false;
            }

            Isometric.IsoCamera.ViewWidth = gdm.PreferredBackBufferWidth;
            Isometric.IsoCamera.ViewHeight = gdm.PreferredBackBufferHeight;
            Isometric.IsoCamera.WorldWidth = ((myMap.MapWidth - 2) * Isometric.Tile.TileStepX);
            Isometric.IsoCamera.WorldHeight = ((myMap.MapHeight - 2) * Isometric.Tile.TileStepY);
            Isometric.IsoCamera.DisplayOffset = new Vector2(baseOffsetX, baseOffsetY);

            return true;
        }

        public void UpdateLevel(Levels.IsoLevel newLevel)
        {
            curLevel = newLevel;
        }

        public void HandleInput(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void MoveObjects()
        {
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime, ScreenInfo screenInfo)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Vector2 firstSquare = new Vector2(Isometric.IsoCamera.Location.X / Isometric.Tile.TileStepX, Isometric.IsoCamera.Location.Y / Isometric.Tile.TileStepY);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Isometric.IsoCamera.Location.X % Isometric.Tile.TileStepX, Isometric.IsoCamera.Location.Y % Isometric.Tile.TileStepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            float maxdepth = ((myMap.MapWidth + 1) + ((myMap.MapHeight + 1) * Isometric.Tile.TileWidth)) * 10;
            float depthOffset;

            for (int y = 0; y < squaresDown; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Isometric.Tile.OddRowXOffset;

                for (int x = 0; x < squaresAcross; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Isometric.Tile.TileWidth)) / maxdepth);
                    
                    if ((mapx >= myMap.MapWidth) || (mapy >= myMap.MapHeight))
                        continue;
                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                            mapy * Isometric.Tile.TileStepY)), Isometric.Tile.GetSourceRectangle(tileID), Color.White,
                            0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    int heightRow = 0;

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].Props)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    /*
                    // Draws the coordinates for each tile
                    spriteBatch.DrawString(Globals.gFonts["Miramonte"], (x + firstX).ToString() + "," + (y + firstY).ToString(),
                        new Vector2((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX + 20,
                        (y * Tile.TileStepY) - offsetY + baseOffsetY + 38), Color.White, 0f, Vector2.Zero,
                        1.0f, SpriteEffects.None, 0.0f);
                    */
                }
            }
            
            // Characters
            Vector2 tempOut = Vector2.Zero;
            /*Vector2 vladStandingOn = myMap.WorldToMapCell(vlad.Position, out tempOut);
            int vladHeight = myMap.Rows[(int)vladStandingOn.Y].Columns[(int)vladStandingOn.X].HeightTiles.Count * Tile.HeightTileOffset;
            vlad.Draw(spriteBatch, 0, -vladHeight);
            */
            Vector2 hilightLoc = Isometric.IsoCamera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            Vector2 hilightPoint = myMap.WorldToMapCell(new Vector2((int)hilightLoc.X, (int)hilightLoc.Y), out tempOut);

            int hilightrowOffset = 0;
            if ((hilightPoint.Y) % 2 == 1)
                hilightrowOffset = Isometric.Tile.OddRowXOffset;

            spriteBatch.Draw(hilight, Isometric.IsoCamera.WorldToScreen(new Vector2((hilightPoint.X * Isometric.Tile.TileStepX) + hilightrowOffset,
                                    (hilightPoint.Y + 2) * Isometric.Tile.TileStepY)),
                            new Rectangle(0, 0, 64, 32), Color.White * 0.3f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}
