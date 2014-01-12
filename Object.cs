using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug
{
    public class Object
    {
        protected Texture2D texture;
        protected Vector2 pos, size, rotationOrigin, scale, direction;
        //protected Vector2 oldPos; // Position of the object from before it was last changed
        protected float speed, maxSpeed, accelerationRate, rotationSpeed, maxRotationSpeed, rotationRate, rotationAngleRads;

        public float GetWidth() { return size.X; }
        public float GetHeight() { return size.Y; }

        public float GetSpeed() { return speed; }

        public Vector2 GetPosition() { return pos; }
        public void SetPosition(Vector2 newPos) { pos = newPos; }

        // If the camera is passed into an Object the camera will move with the given object
        /*public void Update(Camera2D camera = null)
        {
            if (camera != null)
            {
                //Vector2 posChange = pos - oldPos;
                //camera.Position += posChange;
                camera.Position = pos;
                // Check that the object is not near the edge of the world.
            }
        }*/
    }
}
