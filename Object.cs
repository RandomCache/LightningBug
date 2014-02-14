using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug
{
    public class Object
    {
        private static long newObjectId = long.MinValue;
        protected long id;
        protected Texture2D texture;
        // scaledOrigin exists because of the shifting spriteBatch.Draw does to the position the sprite is displayed
        protected Vector2 position, size, rotationOrigin, scale, scaledOrigin; // position
        protected Vector2 direction, velocity; // movement
        //protected Vector2 oldPos; // Position of the object from before it was last changed
        protected float maxSpeed, accelerationRate, rotationSpeed, maxRotationSpeed, rotationRate, rotationAngleRads;
        public Color testColor = Color.White;

        public List<Physics.Polygon> collisionPolygons;
        public int numVertices;
        public Vector2[] nonRotatedVertices; // CCW oriented.  These are used to calculate the rotated vertice every frame
        public Vector2[] vertices; // CCW oriented
        public Vector2[] normals; // vertices[i] = v[i+1] - v[i]

        //public float Speed { get { return speed; } set { speed = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Direction { get { return direction; } }

        public long GetId() { return id; }
        public Vector2 GetSize() { return size; }
        public float GetRotation() { return rotationAngleRads; }
        public float GetRotationSpeed() { return rotationSpeed; }
        public Vector2 GetPosition() { return position; }
        public Vector2 GetCenter() { return position + scaledOrigin; }

        public Object()
        {
            id = newObjectId;
            Interlocked.Increment(ref newObjectId);

            numVertices = 4;
            vertices = new Vector2[numVertices];
            normals = new Vector2[numVertices];
            collisionPolygons = new List<Physics.Polygon>();
        }

        public void SetPosition(Vector2 newPos) 
        {
            position = newPos;
            vertices[0] = position;

            foreach (Physics.Polygon poly in collisionPolygons)
                poly.SetPosition(position, scaledOrigin, rotationAngleRads);
        }

        // Gives impulse to the object in it's current direction
        public void ChangeSpeed(TimeSpan timeSpan, bool increase)
        {
            float speed = Velocity.Length();
            int sign = 1;
            if (!increase)
                sign = -1;
            //speed += timeSpan.Milliseconds * accelerationRate * sign;
            speed = timeSpan.Milliseconds * accelerationRate * sign;
            if (speed > maxSpeed)
                speed = maxSpeed;
            else if (speed < -maxSpeed)
                speed = -maxSpeed;
            velocity += direction * speed;
        }

        // Sets the length of the velocity vector
        public void SetSpeed(float newSpeed)
        {
            if(newSpeed <= 0)
            {
                velocity = Vector2.Zero;
                return;
            }
            float perX = velocity.X / newSpeed;
            float perY = velocity.Y / newSpeed;
            velocity.X = newSpeed * perX;
            velocity.Y = newSpeed * perY;
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

        public void SetRotationSpeed(float newSpeed)
        {
            rotationSpeed = newSpeed;
        }

        protected void UpdateRotation()
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

        public void Update()
        {
            // Add the lines of collision polygons for later drawing
            foreach (Physics.Polygon poly in collisionPolygons)
            {
                for (int i = 0; i < poly.vertices.Length; ++i)
                {
                    //TODO change nonRotatedVertices to vertices when rotation is working
                    if (i == 0)
                        Globals.gPrimitives.AddLine(poly.vertices[poly.vertexOffests.Length - 1], poly.vertices[i]);
                    else
                        Globals.gPrimitives.AddLine(poly.vertices[i - 1], poly.vertices[i]);
                    /*if (i == 0)
                        Globals.gPrimitives.AddLine(poly.vertices[poly.vertexOffests.Length - 1], poly.vertexOffests[i]);
                    else
                        Globals.gPrimitives.AddLine(poly.vertexOffests[i - 1], poly.vertexOffests[i]);                    */
                }
            }

            // Cap this objects speed
            float curSpeedSq = velocity.LengthSquared();
            if (curSpeedSq > maxSpeed * maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
        }

#if DEBUG
        // Used only when setting up debug scenarios
        public void DebugSetup(Vector2 pos, Vector2 dir, Vector2 vel, float rotAngle, float rSpeed)
        {
            position = pos;
            direction = dir;
            velocity = vel;
            rotationAngleRads = rotAngle;
            rotationSpeed = rSpeed;
        }
#endif
    }
}
