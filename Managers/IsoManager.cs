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
        Texture2D hilight;
        Levels.IsoLevel curLevel;
        Vector2 hilightPoint;

        Levels.IsoEditor isoEditor;

        public Levels.IsoLevel CurLevel
        {
            get { return curLevel; }
            set { curLevel = CurLevel; }
        }

        public IsoManager()
        {
            curLevel = null;
        }

        public void Initialize(ContentManager cm)
        {
            curLevel = new Levels.IsoLevel(cm);
        }

        public bool LoadIsoContent(GraphicsDeviceManager gdm, ContentManager cm)
        {
            Isometric.Tile.TileSetTexture = cm.Load<Texture2D>(@"TileSets\temp_groung_tileset_01");
            //texture = content.Load<Texture2D>(Art\\Vehicles\\SampleShip);
            hilight = cm.Load<Texture2D>(@"TileSets\hilight");
            curLevel.Tiles = new Isometric.TileMap();

            if (gdm == null || curLevel.Tiles == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("IsoManager::LoadIsoContent - No graphics device manager or myMap. \n");
                return false;
            }

            Isometric.IsoCamera.ViewWidth = gdm.PreferredBackBufferWidth;
            Isometric.IsoCamera.ViewHeight = gdm.PreferredBackBufferHeight;
            Isometric.IsoCamera.WorldWidth = ((curLevel.Tiles.MapWidth - 2) * Isometric.Tile.TileStepX);
            Isometric.IsoCamera.WorldHeight = ((curLevel.Tiles.MapHeight - 2) * Isometric.Tile.TileStepY);
            Isometric.IsoCamera.DisplayOffset = new Vector2(curLevel.BaseOffsetX, curLevel.BaseOffsetY);

            return true;
        }

        public void UpdateLevel(Levels.IsoLevel newLevel)
        {
            curLevel = newLevel;
        }

        public void HandleInput(GameTime gameTime, KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Left))
            {
                Isometric.IsoCamera.Move(new Vector2(-2, 0));
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                Isometric.IsoCamera.Move(new Vector2(2, 0));
            }

            if (keyState.IsKeyDown(Keys.Up))
            {
                Isometric.IsoCamera.Move(new Vector2(0, -2));
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                Isometric.IsoCamera.Move(new Vector2(0, 2));
            }
        }

        public void Update(GameTime gameTime)
        {
            Vector2 tempOut = Vector2.Zero;
            Vector2 hilightLoc = Isometric.IsoCamera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            hilightPoint = curLevel.Tiles.WorldToMapCell(new Vector2((int)hilightLoc.X, (int)hilightLoc.Y), out tempOut);
            if (Globals.curMode == GameMode.Main)
            {

            }
            else
            {

            }
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

            float maxdepth = ((curLevel.Tiles.MapWidth + 1) + ((curLevel.Tiles.MapHeight + 1) * Isometric.Tile.TileWidth)) * 10;
            float depthOffset;

            for (int y = 0; y < curLevel.TilesHigh; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Isometric.Tile.OddRowXOffset;

                for (int x = 0; x < curLevel.TilesWide; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Isometric.Tile.TileWidth)) / maxdepth);

                    if ((mapx >= curLevel.Tiles.MapWidth) || (mapy >= curLevel.Tiles.MapHeight))
                        continue;
                    foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                            mapy * Isometric.Tile.TileStepY)), Isometric.Tile.GetSourceRectangle(tileID), Color.White,
                            0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    int heightRow = 0;

                    foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * curLevel.HeightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in curLevel.Tiles.Rows[mapy].Columns[mapx].Props)
                    {
                        spriteBatch.Draw(Isometric.Tile.TileSetTexture, Isometric.IsoCamera.WorldToScreen(new Vector2((mapx * Isometric.Tile.TileStepX) + rowOffset,
                                    mapy * Isometric.Tile.TileStepY - (heightRow * Isometric.Tile.HeightTileOffset))), Isometric.Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * curLevel.HeightRowDepthMod));
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
            /*Vector2 vladStandingOn = myMap.WorldToMapCell(vlad.Position, out tempOut);
            int vladHeight = myMap.Rows[(int)vladStandingOn.Y].Columns[(int)vladStandingOn.X].HeightTiles.Count * Tile.HeightTileOffset;
            vlad.Draw(spriteBatch, 0, -vladHeight);
            */


            int hilightrowOffset = 0;
            if ((hilightPoint.Y) % 2 == 1)
                hilightrowOffset = Isometric.Tile.OddRowXOffset;

            spriteBatch.Draw(hilight, Isometric.IsoCamera.WorldToScreen(new Vector2((hilightPoint.X * Isometric.Tile.TileStepX) + hilightrowOffset,
                                    (hilightPoint.Y + 2) * Isometric.Tile.TileStepY)),
                            new Rectangle(0, 0, 64, 32), Color.White * 0.3f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        public void LoadTileTypes()
        {
        }

        public void InitEditor()
        {
            isoEditor = new Levels.IsoEditor();
        }

        public void DestroyEditor()
        {
        }
    }
}
