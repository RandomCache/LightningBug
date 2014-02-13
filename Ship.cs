using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace LightningBug
{
    public class Ship : Object // TODO Componentize
    {
        private bool mainPlayerShip;
        public string Load(ContentManager content, string texturePath, bool isMainPlayer)
        {
            texture = content.Load<Texture2D>(texturePath);
            pos = new Vector2(0, 0);
            size.X = 100;
            size.Y = 100;
            
            scale = new Vector2(100f/texture.Width, 100f/texture.Height);
            rotationOrigin = new Vector2(texture.Width / 2, texture.Width / 2); // Spritebatch.draw uses the texture size when given a rotation origin, not the destination rect size
            scaledOrigin = new Vector2(rotationOrigin.X * scale.X, rotationOrigin.Y * scale.Y);
            mainPlayerShip = isMainPlayer;
            direction = new Vector2(0, -1);
            velocity = Vector2.Zero;
            maxSpeed = 5f;
            maxRotationSpeed = 2.0f;
            accelerationRate = 0.002f; //Increase per second
            rotationRate = 0.002f; // Degrees per second
            
            rotationAngleRads = 0;

            //TEST - Later these will be data driven
            Vector2[] offsets = new Vector2[4];
            offsets[0] = new Vector2(0, 0);
            offsets[1] = new Vector2(0, size.Y);
            offsets[2] = new Vector2(size.X, size.Y);
            offsets[3] = new Vector2(size.X, 0);
            Physics.Polygon tempPoly = new Physics.Polygon(pos, offsets, scaledOrigin, 4);
            collisionPolygons.Add(tempPoly);
            //ENDTEST
            SetPosition(pos);
            return null;
        }

        //public void UpdatePlayersShip(ref Vector2 curScreenPos)
        public void UpdatePlayersShip()
        {
            UpdateRotation();
            //velocity.X = direction.X * speed;
            //velocity.Y = direction.Y * speed;



            // Move the ship based on it's speed and direction
            //Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: direction: " + direction.X + ", " + direction.Y + " speed: " + speed + "\n");
            //pos.X += deltaX;
            //pos.Y += deltaY;
            //Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: position: " + xPos + ", " + yPos + "\n");

            /* Commented out during collision response work - 2/10
            // Check collision
            Vector2 collisionOffset = Globals.gCollision.CheckShip(this);
            UpdateRotation();
            velocity.X = collisionOffset.X;
            velocity.Y = collisionOffset.Y;
            SetPosition(new Vector2(pos.X + velocity.X, pos.Y + velocity.Y));
            */
            Update();
        }

        public string Draw(SpriteBatch spriteBatch, uint levelWidth, uint levelHeight)
        {
            if(texture == null)
                return null;
            // TODO Check if the ship is visible

            // Translate world coordinates to screen coordinates
            /*if (mainPlayerShip) // If this is the players ship we want it at the center of the screen
                spriteBatch.Draw(texture, pos, null, testColor, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(texture, pos, null, testColor, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);*/
            spriteBatch.Draw(texture, pos, null, testColor, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);
            return null;
        }

        public void Reset(Vector2 newPos)
        {
            SetPosition(newPos);
        }
    }
}
