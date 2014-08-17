#define DEV

#region Using Statements
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace LightningBug
{
    public enum LevelType { Space, Planet };
    public enum GameMode { Main, Editor};

    public struct ScreenInfo
    {
        public Vector2 curScreenCenter, curScreenPos, screenDimensions, virtualScreenDimensions;
    }

    public class Globals 
    { 
        public static Collision gCollision;
        public static Dictionary<string, SpriteFont> gFonts;
        public static Graphics.Primitives gPrimitives;
#if DEBUG
        public static Debug gDebug;
#endif
        public static GameMode curMode; // Are we playing the game or editing it?
    }
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LightningBug : Game
    {
        // TODO: Move graphics to it's own class.
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UI.UIManager uiManager;
        private ResolutionRenderer irr;
        ScreenInfo screenInfo;

        string startupPath, slnDir;

        private Texture2D background;

        //private Levels.SpaceLevel spaceLevel;
        //private Levels.IsoLevel isoLevel;
        LevelType curLevelType;

        private SpaceManager spaceManager;
        private IsoManager isoManager;

        KeyboardState previousKeyState;
        MouseState previousMouseState;

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

            screenInfo.virtualScreenDimensions.X = 1920;
            screenInfo.virtualScreenDimensions.Y = 1080;

            // Camera and Independent Resolution Renderer Setup
            irr = new ResolutionRenderer(this, (int)screenInfo.virtualScreenDimensions.X,
                (int)screenInfo.virtualScreenDimensions.Y, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // Move the window.  TODO center the window in the users screen
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

            graphics.ApplyChanges();

            screenInfo.curScreenCenter = new Vector2();
            //spaceLevel = new Levels.SpaceLevel(Content);
            //isoLevel = new Levels.IsoLevel(Content);

            spaceManager = new SpaceManager();
            isoManager = new IsoManager();

            uiManager = new UI.UIManager(GraphicsDevice);

            // Get current resolution of the viewport         
            screenInfo.screenDimensions.X = GraphicsDevice.Viewport.Width;
            screenInfo.screenDimensions.Y = GraphicsDevice.Viewport.Height;

            Globals.gPrimitives.Init(graphics.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        // TODO implement a level/area content loading system
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            string test = slnDir + "\\Art\\blank.bmp";
            test = "\\..\\..\\..Art\\blank.bmp";

            //@TODO try catch around the loading
            Globals.gFonts["Miramonte"] = Content.Load<SpriteFont>("Fonts\\Miramonte");
            uiManager.Load(Content, "test");
            isoManager.LoadTileTypes();
            //ChangeLevel("..\\..\\..\\Content\\Levels\\TestLevel.xml");
            ChangeLevel("..\\..\\..\\Content\\Levels\\TestIsoLevel.xml");
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

            if (curLevelType == LevelType.Space)
                spaceManager.Update(gameTime);
            else if (curLevelType == LevelType.Planet)
                isoManager.Update(gameTime);

 
            //camera.StopFollow();
            uiManager.UpdateAll(irr);
            base.Update(gameTime);
        }
        /*
        // Moves all objects.  Performs collision check beforehand involving objects current and future positions
        void MoveObjects()
        {
            if (curLevelType == LevelType.Space)
                spaceManager.MoveObjects();
            else if (curLevelType == LevelType.Planet)
                isoManager.MoveObjects();
        }*/

        void HandleInput(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            if(previousKeyState == null)
                previousKeyState = ks;            

            // TODO make controls customizable
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || ks.IsKeyDown(Keys.Escape))
                Exit();
            /*else if (Keyboard.GetState().IsKeyDown(Keys.L) && curLevel != null)
            {
                ChangeLevel("test");
            }*/
#if DEV
            if(ks.IsKeyDown(Keys.F2) && previousKeyState.IsKeyUp(Keys.F2))
            {
                if(Globals.curMode == GameMode.Main)
                {
                    Globals.curMode = GameMode.Editor;
                    //Remove current UI
                    uiManager.ClearScene();
                    // Initialize the editor
                    if (curLevelType == LevelType.Space)
                        spaceManager.InitEditor();
                    if (curLevelType == LevelType.Planet)
                        isoManager.InitEditor();
                }
                else if(Globals.curMode == GameMode.Editor)
                {
                    Globals.curMode = GameMode.Main;
                    if (curLevelType == LevelType.Space)
                        spaceManager.DestroyEditor();
                    if (curLevelType == LevelType.Planet)
                        isoManager.DestroyEditor();
                }
            }                
#endif

            // First check the UI. We don't want any mouse input to go through the UI
            uiManager.HandleInput(irr, gameTime, ms, ks, previousMouseState, previousKeyState);

            if (curLevelType == LevelType.Space)
                spaceManager.HandleInput(gameTime, ks);
            if (curLevelType == LevelType.Planet)
                isoManager.HandleInput(gameTime, ks);

            previousKeyState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (curLevelType == LevelType.Space)
                spaceManager.Draw(spriteBatch, GraphicsDevice, gameTime, screenInfo);
            if (curLevelType == LevelType.Planet)
                isoManager.Draw(spriteBatch, GraphicsDevice, gameTime, screenInfo);

            //For Drawing UI: Reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            irr.SetupFullViewport();
            spriteBatch.Begin();
            uiManager.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public LevelType CheckLevelTypeFromFile(XDocument xDoc)
        {
            XElement curElement;
            curElement = xDoc.Root.Element("BasicInfo");
            if (curElement == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Main::CheckLevelTypeFromFile - Bad level loaded. \n");
                Exit();
            }
            string type = curElement.Element("Type").Value;
            if (type == "Space")
                return LevelType.Space;
            else if (type == "Planet")
                return LevelType.Planet;
            else
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Main::CheckLevelTypeFromFile - Unrecognized level type. \n");
                Exit();
                return LevelType.Space;
            }
        }

        public void ChangeLevel(string newLevel)
        {
            // Determine what type of level the new one is
            XDocument xDoc = XDocument.Load(newLevel);
            if (xDoc == null)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Main::ChangeLevel - Bad level loaded. \n");
                Exit();
            }
            LevelType levelType = CheckLevelTypeFromFile(xDoc);
            curLevelType = levelType;
            if (levelType == LevelType.Space)
            {
                spaceManager.Initialize(irr, Content);
                if (spaceManager.CurLevel != null)
                    spaceManager.CurLevel.UnloadLevel();
                spaceManager.CurLevel.LoadLevel(xDoc, ref screenInfo.curScreenCenter);

                // Set the current screen position
                screenInfo.curScreenPos.X = (int)(screenInfo.curScreenCenter.X - (screenInfo.virtualScreenDimensions.X / 2));
                screenInfo.curScreenPos.Y = (int)(screenInfo.curScreenCenter.Y - (screenInfo.virtualScreenDimensions.Y / 2));
                background = Content.Load<Texture2D>("Art\\blank"); // change these names to the names of your images

                spaceManager.UpdateLevel(spaceManager.CurLevel);
                spaceManager.SetupNewLevel(Content, screenInfo);
            }
            else if (levelType == LevelType.Planet)
            {
                isoManager.Initialize(Content);
                if (!isoManager.LoadIsoContent(graphics, Content))
                    Exit();
                isoManager.CurLevel.LoadLevel(xDoc, ref screenInfo.curScreenCenter);
                isoManager.UpdateLevel(isoManager.CurLevel);
            }                
        }
    }
}
