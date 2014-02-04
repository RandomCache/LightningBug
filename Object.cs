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
        protected Vector2 pos, size, rotationOrigin, scale, direction, scaledOrigin;
        //protected Vector2 oldPos; // Position of the object from before it was last changed
        protected float speed, maxSpeed, accelerationRate, rotationSpeed, maxRotationSpeed, rotationRate, rotationAngleRads;
        
        public List<Physics.Polygon> collisionPolygons;
        public int numVertices;
        public Vector2[] nonRotatedVertices; // CCW oriented.  These are used to calculate the rotated vertice every frame
        public Vector2[] vertices; // CCW oriented
        public Vector2[] normals; // vertices[i] = v[i+1] - v[i]



        public long GetId() { return id; }
        public Vector2 GetSize() { return size; }

        public float GetSpeed() { return speed; }

        public Vector2 GetPosition() { return pos; }

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
            pos = newPos;
            vertices[0] = pos;

            foreach (Physics.Polygon poly in collisionPolygons)
                poly.SetPosition(pos, scaledOrigin, rotationAngleRads);
        }

        public void Rotate()
        {

        }
    }
}
