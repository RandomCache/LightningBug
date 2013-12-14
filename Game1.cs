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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        // TODO: Move graphics to it's own class.  duh
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        string startupPath, slnDir;

        private Texture2D background;

        private Level curLevel;
        private Ship playerShip; // Move player and/or enemy ships to a controlling class?

        //uint screenWidth, screenHeight;
        Vector2 curScreenCenter;
        Rectangle curScreen;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            playerShip = null;
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
            curScreenCenter = new Vector2();

            curLevel = new Level(Content);

            // Get current resolution of the viewport
            //Game.GraphicsDevice.Viewport
            curScreen.Width = GraphicsDevice.Viewport.Width;
            curScreen.Height = GraphicsDevice.Viewport.Height;            

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
            background = Content.Load<Texture2D>("Art\\blank"); // change these names to the names of your images
            //ship = Content.Load<Texture2D>("Art\\SampleShip");  // if you are using your own images.

            playerShip = new Ship();
            playerShip.Load(Content, "Art\\SampleShip", true);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            /*else if (Keyboard.GetState().IsKeyDown(Keys.L) && curLevel != null)
            {
                ChangeLevel("test");
            }*/

            // TODO: Add your update logic here
            if (playerShip != null)
            {
                playerShip.Update();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // If we're in a level draw that level
            if (curLevel != null && curLevel.IsLevelLoaded())
            {
                curLevel.DrawLevel(spriteBatch);
                //spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
                //spriteBatch.Draw(ship, new Vector2(800, 800), Color.Azure, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());
            }

            if (playerShip != null)
            {
                playerShip.Draw(spriteBatch, curScreen, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeLevel(string newLevel)
        {
            if (curLevel != null)
                curLevel.UnloadLevel();
            curLevel.LoadLevel("..\\..\\..\\Content\\Levels\\TestLevel.xml", ref curScreenCenter);
            // Set the current screen position
            curScreen.X = (int)(curScreenCenter.X - (curScreen.Width / 2));
            curScreen.Y = (int)(curScreenCenter.Y - (curScreen.Height / 2));
            if (playerShip != null)
            {
                Vector2 newShipPos = curScreenCenter;
                // Go from the center of the screen to the ship position, top left of it.
                newShipPos.X -= playerShip.GetWidth() / 2;
                newShipPos.Y -= playerShip.GetHeight() / 2;
                playerShip.Reset(newShipPos);
            }
        }
    }
}
