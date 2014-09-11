#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace TileStart
{
    public class Globals
    {
        public static Dictionary<string, SpriteFont> gFonts;
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileMap myMap;
        int squaresAcross;
        int squaresDown;
        int baseOffsetX;
        int baseOffsetY;
        float heightRowDepthMod;
        Texture2D hilight;
        SpriteAnimation vlad;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Globals.gFonts = new Dictionary<string, SpriteFont>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            //myMap = new TileMap();
            squaresAcross = 17;
            squaresDown = 37;
            baseOffsetX = baseOffsetY = -14;
            heightRowDepthMod = 0.0000001f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Tile.TileSetTexture = Content.Load<Texture2D>(@"Textures\TileSets\part4_tileset");
            Globals.gFonts["Miramonte"] = Content.Load<SpriteFont>("Fonts\\Miramonte");
            myMap = new TileMap(Content.Load<Texture2D>(@"Textures\TileSets\mousemap"));
            hilight = Content.Load<Texture2D>(@"Textures\TileSets\hilight");

            // Camera
            Camera.ViewWidth = this.graphics.PreferredBackBufferWidth;
            Camera.ViewHeight = this.graphics.PreferredBackBufferHeight;
            Camera.WorldWidth = ((myMap.MapWidth - 2) * Tile.TileStepX);
            Camera.WorldHeight = ((myMap.MapHeight - 2) * Tile.TileStepY);
            Camera.DisplayOffset = new Vector2(baseOffsetX, baseOffsetY);

            // Vlad
            vlad = new SpriteAnimation(Content.Load<Texture2D>(@"Textures\Characters\T_Vlad_Sword_Walking_48x48"));

            vlad.AddAnimation("WalkEast", 0, 48 * 0, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorth", 0, 48 * 1, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorthEast", 0, 48 * 2, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkNorthWest", 0, 48 * 3, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouth", 0, 48 * 4, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouthEast", 0, 48 * 5, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkSouthWest", 0, 48 * 6, 48, 48, 8, 0.1f);
            vlad.AddAnimation("WalkWest", 0, 48 * 7, 48, 48, 8, 0.1f);

            vlad.AddAnimation("IdleEast", 0, 48 * 0, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorth", 0, 48 * 1, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorthEast", 0, 48 * 2, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleNorthWest", 0, 48 * 3, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouth", 0, 48 * 4, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouthEast", 0, 48 * 5, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleSouthWest", 0, 48 * 6, 48, 48, 1, 0.2f);
            vlad.AddAnimation("IdleWest", 0, 48 * 7, 48, 48, 1, 0.2f);

            vlad.Position = new Vector2(100, 100);
            vlad.DrawOffset = new Vector2(-24, -38);
            vlad.CurrentAnimation = "WalkEast";
            vlad.IsAnimating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Vector2 moveVector = Vector2.Zero;
            Vector2 moveDir = Vector2.Zero;
            string animation = "";

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.W) && ks.IsKeyDown(Keys.A))
            {
                moveDir = new Vector2(-2, -1);
                animation = "WalkNorthWest";
                moveVector += new Vector2(-2, -1);
            }
            else if (ks.IsKeyDown(Keys.W) && ks.IsKeyDown(Keys.D))
            {
                moveDir = new Vector2(2, -1);
                animation = "WalkNorthEast";
                moveVector += new Vector2(2, -1);
            }
            else if (ks.IsKeyDown(Keys.S) && ks.IsKeyDown(Keys.D))
            {
                moveDir = new Vector2(2, 1);
                animation = "WalkSouthEast";
                moveVector += new Vector2(2, 1);
            }
            else if (ks.IsKeyDown(Keys.S) && ks.IsKeyDown(Keys.A))
            {
                moveDir = new Vector2(-2, 1);
                animation = "WalkSouthWest";
                moveVector += new Vector2(-2, 1);
            }
            else if(ks.IsKeyDown(Keys.W))
            {
                moveDir = new Vector2(0, -1);
                animation = "WalkNorth";
                moveVector += new Vector2(0, -1);
            }
            else if (ks.IsKeyDown(Keys.A))
            {
                moveDir = new Vector2(-2, 0);
                animation = "WalkWest";
                moveVector += new Vector2(-2, 0);
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                moveDir = new Vector2(2, 0);
                animation = "WalkEast";
                moveVector += new Vector2(2, 0);
            }
            else if (ks.IsKeyDown(Keys.S))
            {
                moveDir = new Vector2(0, 1);
                animation = "WalkSouth";
                moveVector += new Vector2(0, 1);
            }
            // Don't move into a non-walkable tile
            if (myMap.GetCellAtWorldPoint(vlad.Position + moveDir).Walkable == false)
            {
                moveDir = Vector2.Zero;
            }

            if (moveDir.Length() != 0)
            {
                vlad.MoveBy((int)moveDir.X, (int)moveDir.Y);
                if (vlad.CurrentAnimation != animation)
                    vlad.CurrentAnimation = animation;
            }
            else
            {
                vlad.CurrentAnimation = "Idle" + vlad.CurrentAnimation.Substring(4);
            }

            // Keep Vlad in the game world
            float vladX = MathHelper.Clamp(
            vlad.Position.X, 0 + vlad.DrawOffset.X, Camera.WorldWidth);
            float vladY = MathHelper.Clamp(
            vlad.Position.Y, 0 + vlad.DrawOffset.Y, Camera.WorldHeight);

            vlad.Position = new Vector2(vladX, vladY);

            // Move the camera with Vlad
            Vector2 testPosition = Camera.WorldToScreen(vlad.Position);
            if (testPosition.X < 100)
            {
                Camera.Move(new Vector2(testPosition.X - 100, 0));
            }

            if (testPosition.X > (Camera.ViewWidth - 100))
            {
                Camera.Move(new Vector2(testPosition.X - (Camera.ViewWidth - 100), 0));
            }

            if (testPosition.Y < 100)
            {
                Camera.Move(new Vector2(0, testPosition.Y - 100));
            }

            if (testPosition.Y > (Camera.ViewHeight - 100))
            {
                Camera.Move(new Vector2(0, testPosition.Y - (Camera.ViewHeight - 100)));
            }

            vlad.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            Vector2 firstSquare = new Vector2(Camera.Location.X / Tile.TileStepX, Camera.Location.Y / Tile.TileStepY);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Camera.Location.X % Tile.TileStepX, Camera.Location.Y % Tile.TileStepY);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            float maxdepth = ((myMap.MapWidth + 1) + ((myMap.MapHeight + 1) * Tile.TileWidth)) * 10;
            float depthOffset;  

            for (int y = 0; y < squaresDown; y++)
            {
                int rowOffset = 0;
                if ((firstY + y) % 2 == 1)
                    rowOffset = Tile.OddRowXOffset;

                for (int x = 0; x < squaresAcross; x++)
                {
                    int mapx = (firstX + x);
                    int mapy = (firstY + y);
                    depthOffset = 0.7f - ((mapx + (mapy * Tile.TileWidth)) / maxdepth);
                    /*
                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        spriteBatch.Draw(Tile.TileSetTexture, new Rectangle((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                            (y * Tile.TileStepY) - offsetY + baseOffsetY, Tile.TileWidth, Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                    }

                    int heightRow = 0;

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(
                            Tile.TileSetTexture,
                            new Rectangle(
                                (x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                                (y * Tile.TileStepY) - offsetY + baseOffsetY - (heightRow * Tile.HeightTileOffset),
                                Tile.TileWidth, Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }
                    */
                    if ((mapx >= myMap.MapWidth) || (mapy >= myMap.MapHeight))
                        continue;
                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].BaseTiles)
                    {
                        /*
                        spriteBatch.Draw(Tile.TileSetTexture, new Rectangle((x * Tile.TileStepX) - offsetX + rowOffset + baseOffsetX,
                            (y * Tile.TileStepY) - offsetY + baseOffsetY, Tile.TileWidth, Tile.TileHeight),
                            Tile.GetSourceRectangle(tileID), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                        */
                        spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(new Vector2((mapx * Tile.TileStepX) + rowOffset,
                            mapy * Tile.TileStepY)), Tile.GetSourceRectangle(tileID), Color.White,
                            0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    int heightRow = 0;

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].HeightTiles)
                    {
                        spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(new Vector2((mapx * Tile.TileStepX) + rowOffset,
                                    mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))), Tile.GetSourceRectangle(tileID),
                            Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depthOffset - ((float)heightRow * heightRowDepthMod));
                        heightRow++;
                    }

                    foreach (int tileID in myMap.Rows[mapy].Columns[mapx].Props)
                    {
                        spriteBatch.Draw(Tile.TileSetTexture, Camera.WorldToScreen(new Vector2((mapx * Tile.TileStepX) + rowOffset,
                                    mapy * Tile.TileStepY - (heightRow * Tile.HeightTileOffset))), Tile.GetSourceRectangle(tileID),
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
            Vector2 vladStandingOn = myMap.WorldToMapCell(vlad.Position, out tempOut);
            int vladHeight = myMap.Rows[(int)vladStandingOn.Y].Columns[(int)vladStandingOn.X].HeightTiles.Count * Tile.HeightTileOffset;
            vlad.Draw(spriteBatch, 0, -vladHeight);

            Vector2 hilightLoc = Camera.ScreenToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));            
            Vector2 hilightPoint = myMap.WorldToMapCell(new Vector2((int)hilightLoc.X, (int)hilightLoc.Y), out tempOut);

            int hilightrowOffset = 0;
            if ((hilightPoint.Y) % 2 == 1)
                hilightrowOffset = Tile.OddRowXOffset;

            spriteBatch.Draw(hilight, Camera.WorldToScreen(new Vector2((hilightPoint.X * Tile.TileStepX) + hilightrowOffset,
                                    (hilightPoint.Y + 2) * Tile.TileStepY)),
                            new Rectangle(0, 0, 64, 32), Color.White * 0.3f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
