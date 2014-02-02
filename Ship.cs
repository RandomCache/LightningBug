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
        public Color testColor = Color.White;
        public string Load(ContentManager content, string texturePath, bool isMainPlayer)
        {
            texture = content.Load<Texture2D>(texturePath);
            pos = new Vector2(0, 0);
            size.X = 100;
            size.Y = 100;
            
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

            //TEST - Later these will be data driven
            Vector2[] offsets = new Vector2[4];
            offsets[0] = new Vector2(0, 0);
            offsets[1] = new Vector2(0, size.Y);
            offsets[2] = new Vector2(size.X, size.Y);
            offsets[3] = new Vector2(size.X, 0);
            Physics.Polygon tempPoly = new Physics.Polygon(pos, offsets, 4);
            collisionPolygons.Add(tempPoly);
            //ENDTEST
            SetPosition(pos);
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

        //public void UpdatePlayersShip(ref Vector2 curScreenPos)
        public void UpdatePlayersShip()
        {
            UpdateRotation();
            // Move the ship based on it's speed and direction
            float deltaX, deltaY;
            deltaX = direction.X * speed;
            deltaY = direction.Y * speed;
            //Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: direction: " + direction.X + ", " + direction.Y + " speed: " + speed + "\n");
            //pos.X += deltaX;
            //pos.Y += deltaY;
            SetPosition(new Vector2(pos.X + deltaX, pos.Y + deltaY));
            //Logging.Instance(Logging.DEFAULTLOG).Log("UpdatePlayersShip: position: " + xPos + ", " + yPos + "\n");

            // Check collision
            Globals.gCollision.CheckShip(this);
        }

        public string Draw(SpriteBatch spriteBatch, Camera2D camera, Vector2 curScreenPos, Vector2 curScreenDimensions, uint levelWidth, uint levelHeight)
        {
            if(texture == null)
                return null;
            // TODO Check if the ship is visible

            // Translate world coordinates to screen coordinates
            if (mainPlayerShip) // If this is the players ship we want it at the center of the screen
                spriteBatch.Draw(texture, pos, null, testColor, rotationAngleRads, rotationOrigin, scale, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(texture, pos, testColor);
            return null;
        }

        public void Reset(Vector2 newPos)
        {
            SetPosition(newPos);
        }
    }
}
