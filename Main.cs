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

namespace LightningBug
{
    public class Globals 
    { 
        public static Collision gCollision;
        public static Dictionary<string, SpriteFont> gFonts;
        public static Graphics.Primitives gPrimitives;
#if DEBUG
        public static Debug gDebug;
#endif
    }
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LightningBug : Game
    {
        // TODO: Move graphics to it's own class.  duh
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UI.UIManager uiManager;
        private ResolutionRenderer irr;
        private Camera2D camera;

        string startupPath, slnDir;

        private Texture2D background;

        private Level curLevel;
        private Ship playerShip; // Move player and/or enemy ships to a controlling class?
        public List<Ship> enemyShips;

        Vector2 curScreenCenter, curScreenPos, screenDimensions, virtualScreenDimensions;

#region properties
        public Level GetCurLevel()  { return curLevel; }
        public Ship GetPlayersShip() { return playerShip; }
#endregion

        public LightningBug()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //Globals
            Globals.gCollision = Collision.Instance(this);
            Globals.gFonts = new Dictionary<string, SpriteFont>();
            Globals.gPrimitives = new Graphics.Primitives();
#if DEBUG
            Globals.gDebug = new Debug();
#endif

            uiManager = new UI.UIManager();
            playerShip = new Ship();
            enemyShips = new List<Ship>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            startupPath = Environment.CurrentDirectory;
            slnDir = startupPath + "\\..\\..\\..";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            virtualScreenDimensions.X = 1920;
            virtualScreenDimensions.Y = 1080;

            // Camera and Independent Resolution Renderer Setup
            irr = new ResolutionRenderer(this, (int)virtualScreenDimensions.X, (int)virtualScreenDimensions.Y, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            camera = new Camera2D(irr) { MaxZoom = 2f, MinZoom = 1f, Zoom = 1f };
            camera.SetPosition(Vector2.Zero);
            camera.RecalculateTransformationMatrices();
            // Move the window.  TODO center the window in the users screen
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

            graphics.ApplyChanges();

            curScreenCenter = new Vector2();
            curLevel = new Level(Content);

            // Get current resolution of the viewport         
            screenDimensions.X = GraphicsDevice.Viewport.Width;
            screenDimensions.Y = GraphicsDevice.Viewport.Height;

            Globals.gPrimitives.Init(graphics.GraphicsDevice);

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
            string test = slnDir + "\\Art\\blank.bmp";
            test = "\\..\\..\\..Art\\blank.bmp";
            //@TODO try catch around the loading
            Globals.gFonts["Miramonte"] = Content.Load<SpriteFont>("Fonts\\Miramonte");
            uiManager.Load(Content, "test");
            ChangeLevel("test");
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
            // Clear previous frame data
            Globals.gPrimitives.ClearAll();

            HandleInput(gameTime);

            if (playerShip != null)
            {
                playerShip.UpdatePlayersShip();
            }
            foreach (Ship enemy in enemyShips)
            {
                enemy.Update();
            }

            MoveObjects();

            camera.Update(gameTime, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());
            //camera.StopFollow();
            uiManager.UpdateAll(irr);
            base.Update(gameTime);
        }

        // Moves all objects.  Performs collision check beforehand involving objects current and future positions
        void MoveObjects()
        {
            Level curLevel = GetCurLevel();
            //TODO - Optimize so I'm not checking every object against every other object
            if (playerShip != null)
            {
                // Check against all enemies
                foreach (Ship enemy in enemyShips)
                {
                    Globals.gCollision.CheckShip(playerShip, enemy);
                }
                // Lastly check against the level boundries
                Globals.gCollision.CheckShip(playerShip, curLevel);

                playerShip.SetPosition(new Vector2(playerShip.GetPosition().X + playerShip.Velocity.X, playerShip.GetPosition().Y + playerShip.Velocity.Y));
            }
            foreach (Ship enemy in enemyShips)
            {
                //check against the player then all enemies
                Globals.gCollision.CheckShip(enemy, playerShip);
                foreach (Ship enemy2 in enemyShips)
                {
                    if (enemy.GetId() == enemy2.GetId())
                        continue;
                    Globals.gCollision.CheckShip(enemy, enemy2);
                }
                // Lastly check against the level boundries
                Globals.gCollision.CheckShip(enemy, curLevel);

                enemy.SetPosition(new Vector2(enemy.GetPosition().X + enemy.Velocity.X, enemy.GetPosition().Y + enemy.Velocity.Y));
            }
#if DEBUG
            if (float.IsNaN(playerShip.GetPosition().X))
                System.Diagnostics.Debugger.Break();
            Globals.gDebug.player.states[1] = Globals.gDebug.player.states[0];
            Globals.gDebug.player.SetCurrentState(playerShip);
            Globals.gDebug.enemy.states[1] = Globals.gDebug.enemy.states[0];
            Globals.gDebug.enemy.SetCurrentState(enemyShips[0]);
#endif
        }

        void HandleInput(GameTime gameTime)
        {
            // TODO make controls customizable
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            /*else if (Keyboard.GetState().IsKeyDown(Keys.L) && curLevel != null)
            {
                ChangeLevel("test");
            }*/
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                playerShip.ChangeSpeed(gameTime.ElapsedGameTime, true);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                playerShip.ChangeSpeed(gameTime.ElapsedGameTime, false);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                playerShip.ChangeRotationSpeed(gameTime.ElapsedGameTime, false);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                playerShip.ChangeRotationSpeed(gameTime.ElapsedGameTime, true);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                playerShip.SetSpeed(0);
                playerShip.SetRotationSpeed(0);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Prepare IRR call
            irr.Draw();

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewTransformationMatrix());
            //IRR only - mBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, renderer.GetTransformationMatrix());
            // If we're in a level draw that level
            if (curLevel != null && curLevel.IsLevelLoaded())
            {
                curLevel.DrawLevel(spriteBatch, camera, curScreenPos, screenDimensions);
            }

            if (playerShip != null)
                playerShip.Draw(spriteBatch, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());

            foreach (Ship enemy in enemyShips)
            {
                enemy.Draw(spriteBatch, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());
            }

            Globals.gPrimitives.DrawAllPrimitives(spriteBatch);
            spriteBatch.End();

            //For Drawing UI: Reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            irr.SetupFullViewport();
            spriteBatch.Begin();
            uiManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeLevel(string newLevel)
        {
            if (curLevel != null)
                curLevel.UnloadLevel();
            curLevel.LoadLevel("..\\..\\..\\Content\\Levels\\TestLevel.xml", ref curScreenCenter);
            // Set the current screen position
            curScreenPos.X = (int)(curScreenCenter.X - (virtualScreenDimensions.X / 2));
            curScreenPos.Y = (int)(curScreenCenter.Y - (virtualScreenDimensions.Y / 2));
            background = Content.Load<Texture2D>("Art\\blank"); // change these names to the names of your images

            if (playerShip == null)
            playerShip = new Ship();
            playerShip.Load(Content, "Art\\Vehicles\\SampleShip", true);

            Vector2 newShipPos = curScreenCenter;
            // Go from the center of the screen to the ship position, top left of it.
            newShipPos.X -= playerShip.GetSize().X / 2;
            newShipPos.Y -= playerShip.GetSize().Y / 2;
            playerShip.Reset(newShipPos);
            camera.StartFollow(playerShip);

            // Load enemies
            Ship tempShip = new Ship();
            tempShip.Load(Content, "Art\\Vehicles\\2dAlienUfo", false);
            tempShip.SetPosition(new Vector2(3000, 2500));
            enemyShips.Add(tempShip);
            /*
            tempShip = new Ship();
            tempShip.Load(Content, "Art\\Vehicles\\2dAlienUfo", false);
            tempShip.SetPosition(new Vector2(3050, 2200));
            enemyShips.Add(tempShip);
            */
#if DEBUG
            //playerShip.DebugSetup(new Vector2(2890.771f, 2477.647f), new Vector2(0.8470135f, -0.5315714f), Vector2.Zero, -5.27282f, 0.002f);
#endif
        }
    }
}
