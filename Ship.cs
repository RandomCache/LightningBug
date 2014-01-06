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
    public class Ship
    {
        //private int xPos, yPos; 
        private Texture2D texture;
        private bool mainPlayerShip;
        //Rectangle shipRect; // World Ship Coordinates, always top left of the ship, and size
        float xPos, yPos;
        float width, height;
        Vector2 rotationOrigin, scale;
        float speed, maxSpeed, accelerationRate, rotationSpeed, maxRotationSpeed, rotationRate; //TODO implement maximums 
        float rotationAngleRads;
        Vector2 direction; // Normalized vector describing the ships directions

        //public int GetWidth() { return shipRect.Width; }
        //public int GetHeight() { return shipRect.Height; }
        public float GetWidth() { return width; }
        public float GetHeight() { return height; }

        public float GetSpeed() { return speed; }

        public string Load(ContentManager content, string texturePath, bool isMainPlayer)
        {
            texture = content.Load<Texture2D>(texturePath);  // if you are using your own images.
            /*
            shipRect.X = 0;
            shipRect.Y = 0;
            shipRect.Width = 100;
            shipRect.Height = 100;
            */
            
            xPos = 0;
            yPos = 0;
            width = 100;
            height = 100;
            
            scale = new Vector2(100f/texture.Width, 100f/texture.Height);
            rotationOrigin = new Vector2(texture.Width / 2, texture.Width / 2); // Spritebatch.draw uses the texture size when given a rotation origin, not the destination rect size
            mainPlayerShip = isMainPlayer;
            direction = new Vector2(0, -1);
            speed = 0;
            maxSpeed = 5f;
            maxRotationSpeed = 2.0f;
            accelerationRate = 0.002f; //Increase per millisecond
            rotationRate = 0.002f; // Degrees per second
            
            rotationAngleRads = 0;
            return null;
        }

        public void ChangeSpeed(TimeSpan timeSpan, bool increase)
        {
            int sign = 1;
            if(!increase)
                sign = -1;
            speed += timeSpan.Milliseconds * accelerationRate * sign;
            if (speed > maxSpeed)
                speed = maxSpeed;
            else if (speed < -maxSpeed)
                speed = -maxSpeed;
        }

        public void ChangeRotationSpeed(TimeSpan timeSpan, bool clockwise)
        {
            float sign = 1.0f;
            if (!clockwise)
                sign = -1.0f;
            rotationSpeed += (float)timeSpan.Milliseconds * rotationRate * sign;
            if (rotationSpeed > maxRotationSpeed)
                rotationSpeed = maxRotationSpeed;
            else if (rotationSpeed < -maxRotationSpeed)
                rotationSpeed = -maxRotationSpeed;
        }

        void UpdateRotation()
        {
            float theta, cs, sn, newX, newY;

            theta = LBMath.DegreesToRad(rotationSpeed);
            rotationAngleRads += theta;
            cs = (float)Math.Cos(theta);
            sn = (float)Math.Sin(theta);

            newX = direction.X * cs - direction.Y * sn;
            newY = direction.X * sn + direction.Y * cs;
            direction.X = newX;
            direction.Y = newY;
            direction.Normalize();
        }

        //public void UpdatePlayersShip(ref Rectangle curScreen)
        public void UpdatePlayersShip(ref Vector2 curScreenPos, ref Vector2 curScreenDimensions)
        {
            UpdateRotation();
            // Move the ship based on it's speed and direction
            float deltaX, deltaY;
            /* Temp commented out changing curScreen rect to vectors
            deltaX = (int)(direction.X * speed);
            deltaY = (int)(direction.Y * speed);
            */
            deltaX = direction.X * speed;
            deltaY = direction.Y * speed;
            Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: direction: " + direction.X + ", " + direction.Y + " speed: " + speed + "\n");
            //Rectangle oldShip = shipRect;
            //shipRect.X += deltaX;
            //shipRect.Y += deltaY;
            xPos += deltaX;
            yPos += deltaY;
            //curScreen.X += deltaX;
            //curScreen.Y += deltaY;
            curScreenPos.X += deltaX;
            curScreenPos.Y += deltaY;

            Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: position: " + xPos + ", " + yPos + "\n");

            // Check collision
            Globals.gCollision.CheckShip(this);
        }

        //public string Draw(SpriteBatch spriteBatch, Rectangle curScreen, uint levelWidth, uint levelHeight)
        public string Draw(SpriteBatch spriteBatch, Vector2 curScreenPos, Vector2 curScreenDimensions, uint levelWidth, uint levelHeight)
        {
            if(texture == null)
                return null;

            // Translate world coordinates to screen coordinates
            if (mainPlayerShip) // If this is the players ship we want it at the center of the screen
            {
                // We normally want it at the center of the screen, but if near the edge we ensure the
                // edge of the screen does not exceed the edge of the level
                /* Temp commented out changing curScreen rect to vectors
                float halfScreenWidth = curScreen.Width * 0.5f;
                float halfScreenHeight = curScreen.Height * 0.5f;
                float remainingXDist = levelWidth - (curScreen.X + curScreen.Width);
                float remainingYDist = levelHeight - (curScreen.Y + curScreen.Height);
                */
                float halfScreenWidth = curScreenDimensions.X * 0.5f;
                float halfScreenHeight = curScreenDimensions.Y * 0.5f;
                float remainingXDist = levelWidth - (curScreenPos.X + curScreenDimensions.X);
                float remainingYDist = levelHeight - (curScreenPos.Y + curScreenDimensions.Y);

                /*Rectangle screenShip = new Rectangle(curScreen.Width / 2 - shipRect.Width / 2, curScreen.Height / 2 - shipRect.Height / 2,
                    shipRect.Width, shipRect.Height);*/
                /* temp commented out going from ints to floats
                Rectangle screenShip = new Rectangle(curScreen.Width / 2, curScreen.Height / 2,
                    shipRect.Width, shipRect.Height);

                if (curScreen.X < halfScreenWidth - (shipRect.Width * 0.5f))
                    screenShip.X -= (int)((halfScreenWidth - (shipRect.Width * 0.5f)) - curScreen.X);
                else if (remainingXDist < (halfScreenWidth - (shipRect.Width * 0.5f)))
                    screenShip.X += (int)((halfScreenWidth - (shipRect.Width * 0.5f)) - remainingXDist);
                if (curScreen.Y < halfScreenHeight - (shipRect.Height * 0.5f))
                    screenShip.Y -= (int)((halfScreenHeight - (shipRect.Height * 0.5f)) - curScreen.Y);
                else if (remainingYDist < (halfScreenHeight - (shipRect.Height * 0.5f)))
                    screenShip.Y += (int)((halfScreenHeight - (shipRect.Height * 0.5f)) - remainingYDist);
                spriteBatch.Draw(texture, screenShip, null, Color.White, rotationAngleRads, rotationOrigin, SpriteEffects.None, 0);
                */
                
                //Vector2 screenShip = new Vector2(curScreen.Width / 2f - width / 2f, curScreen.Height / 2f - height / 2f);
                /* Temp commented out changing curScreen rect to vectors
                Vector2 screenShip = new Vector2(curScreen.Width / 2f, curScreen.Height / 2f);
                if (curScreen.X < halfScreenWidth - (width * 0.5f))
                    screenShip.X -= (int)((halfScreenWidth - (width * 0.5f)) - curScreen.X);
                else if(remainingXDist < (halfScreenWidth - (width * 0.5f)))
                    screenShip.X += (int)((halfScreenWidth - (width * 0.5f)) - remainingXDist);
                if (curScreen.Y < halfScreenHeight - (height * 0.5f))
                    screenShip.Y -= (int)((halfScreenHeight - (height * 0.5f)) - curScreen.Y);
                else if(remainingYDist < (halfScreenHeight - (height * 0.5f)))
                    screenShip.Y += (int)((halfScreenHeight - (height * 0.5f)) - remainingYDist);
                spriteBatch.Draw(texture, screenShip, null, Color.White, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);
                */
                Vector2 screenShip = new Vector2(curScreenDimensions.X / 2f, curScreenDimensions.Y / 2f);
                if (curScreenPos.X < halfScreenWidth - (width * 0.5f))
                    screenShip.X -= (int)((halfScreenWidth - (width * 0.5f)) - curScreenPos.X);
                else if (remainingXDist < (halfScreenWidth - (width * 0.5f)))
                    screenShip.X += (int)((halfScreenWidth - (width * 0.5f)) - remainingXDist);
                if (curScreenPos.Y < halfScreenHeight - (height * 0.5f))
                    screenShip.Y -= (int)((halfScreenHeight - (height * 0.5f)) - curScreenPos.Y);
                else if (remainingYDist < (halfScreenHeight - (height * 0.5f)))
                    screenShip.Y += (int)((halfScreenHeight - (height * 0.5f)) - remainingYDist);
                spriteBatch.Draw(texture, screenShip, null, Color.White, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);
            }
            else
            {
                // TODO Uncomment and Test this
                /*
                Rectangle screenShip = new Rectangle(shipRect.X - curScreen.X, shipRect.Y - curScreen.Y, width, height);
                // Is the ship visible
                if ((screenShip.Left + screenShip.Width > 0 && screenShip.Left < curScreen.Left + curScreen.Width) &&
                    (screenShip.Y + screenShip.Height > 0 && screenShip.Y < curScreen.Top + curScreen.Height))
                {
                    spriteBatch.Draw(texture, screenShip, Color.White);
                }*/
                
            }
            return null;
        }

        public void Reset(Vector2 newPos)
        {
            //shipRect.X = (int)newPos.X;
            //shipRect.Y = (int)newPos.Y;
            xPos = newPos.X;
            yPos = newPos.Y;
        }
    }
}
