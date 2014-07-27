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
    public class SpaceManager
    {
        private Camera2D camera;
        private ResolutionRenderer irr;
        private Ship playerShip; // Move player and/or enemy ships to a controlling class?
        public List<Ship> enemyShips;
        private Levels.SpaceLevel curLevel;

        #region properties
        public Ship GetPlayersShip() { return playerShip; }

        public Levels.SpaceLevel CurLevel
        {
            get { return curLevel; }
            set { curLevel = CurLevel; }
        }
        #endregion

        public SpaceManager()
        {
            curLevel = null;
            playerShip = new Ship();
            enemyShips = new List<Ship>();
        }

        public void Initialize(ResolutionRenderer pIrr, ContentManager cm)
        {
            irr = pIrr;
            camera = new Camera2D(irr) { MaxZoom = 2f, MinZoom = 1f, Zoom = 1f };
            camera.SetPosition(Vector2.Zero);
            camera.RecalculateTransformationMatrices();
            curLevel = new Levels.SpaceLevel(cm);
        }

        public void UpdateLevel(Levels.SpaceLevel newLevel)
        {
            curLevel = newLevel;
        }

        public void Update(GameTime gameTime)
        {
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
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, GameTime gameTime, ScreenInfo screenInfo)
        {
            //Prepare IRR call
            irr.Draw();

            graphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, camera.GetViewTransformationMatrix());
            //IRR only - mBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, renderer.GetTransformationMatrix());
            // If we're in a level draw that level
            if (curLevel != null && curLevel.IsLevelLoaded())
            {
                curLevel.DrawLevel(spriteBatch, camera, screenInfo);
            }

            if (playerShip != null)
                playerShip.Draw(spriteBatch, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());

            foreach (Ship enemy in enemyShips)
            {
                enemy.Draw(spriteBatch, curLevel.GetLevelWidth(), curLevel.GetLevelHeight());
            }

            Globals.gPrimitives.DrawAllPrimitives(spriteBatch);
            spriteBatch.End();
        }

        public void HandleInput(GameTime gameTime, KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.W))
            {
                playerShip.ChangeSpeed(gameTime.ElapsedGameTime, true);
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                playerShip.ChangeSpeed(gameTime.ElapsedGameTime, false);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                playerShip.ChangeRotationSpeed(gameTime.ElapsedGameTime, false);
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                playerShip.ChangeRotationSpeed(gameTime.ElapsedGameTime, true);
            }
            if (keyState.IsKeyDown(Keys.D0))
            {
                playerShip.SetSpeed(0);
                playerShip.SetRotationSpeed(0);
            }
        }

        // Moves all objects.  Performs collision check beforehand involving objects current and future positions
        public void MoveObjects()
        {
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

        public void SetupNewLevel(ContentManager cm, ScreenInfo screenInfo)
        {
            if (playerShip == null)
                playerShip = new Ship();
            playerShip.Load(cm, "Art\\Vehicles\\SampleShip", true);

            Vector2 newShipPos = screenInfo.curScreenCenter;
            // Go from the center of the screen to the ship position, top left of it.
            newShipPos.X -= playerShip.GetSize().X / 2;
            newShipPos.Y -= playerShip.GetSize().Y / 2;
            playerShip.Reset(newShipPos);
            camera.StartFollow(playerShip);

            // Load enemies
            Ship tempShip = new Ship();
            tempShip.Load(cm, "Art\\Vehicles\\2dAlienUfo", false);
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

        public void InitEditor()
        {
        }

        public void DestroyEditor()
        {
        }
    }
}
