using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightningBug.Physics
{
    public class Polygon
    {
        public int numVertices;
        public Vector2[] vertexOffests; // CCW oriented.  Offset for each vertex from the objects new position.  Created in the ship editor
        public Vector2[] nonRotatedVertices; // CCW oriented.  These are used to calculate the rotated vertice every frame
        public Vector2[] vertices; // CCW oriented
        public Vector2[] normals; // vertices[i] = v[i+1] - v[i]
        bool reverseNormals; // If this is true the normals will face inward

        public Polygon(Vector2 newPos, Vector2[] offsets, Vector2 scaledOrigin, int numberOfVertices, bool reverseNormal = false)
        {
            numVertices = numberOfVertices;
            vertexOffests = new Vector2[numVertices];
            nonRotatedVertices = new Vector2[numVertices];
            vertices = new Vector2[numVertices];
            normals = new Vector2[numVertices];
            reverseNormals = reverseNormal;

            if (offsets.Length != numVertices)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Shapes::Shapes(Vector2 newPos, Vector2[] offests, int numberOfVertices): offests.Length != numVertices\n");
                System.Environment.Exit(0);
            }
            
            for (int i = 0; i < numVertices; ++i)
            {
                vertexOffests[i] = offsets[i];
            }
        }
        public void SetPosition(Vector2 newPos, Vector2 scaledOrigin, float radAngle)
        {
            // newPos is used as the center of rotation here because of the shifting spriteBatch.Draw does to the position the sprite is displayed
            for (int i = 0; i < numVertices; ++i)
            {
                // Subtracting the scaled origin because of the shifting spriteBatch.Draw does to the position the sprite is displayed
                nonRotatedVertices[i] = newPos + vertexOffests[i] - scaledOrigin;
            }
            //if we define dx=x2-x1 and dy=y2-y1, then the normals are (-dy, dx) and (dy, -dx).
            float nx, ny;
            for (int i = 0; i < numVertices; ++i)
            {
                double cosAngle = Math.Cos(radAngle);
                double sinAngle = Math.Sin(radAngle);
                double dx = (nonRotatedVertices[i].X - newPos.X);
                double dy = (nonRotatedVertices[i].Y - newPos.Y);

                vertices[i].X = newPos.X + (int)(dx * cosAngle - dy * sinAngle);
                vertices[i].Y = newPos.Y + (int)(dx * sinAngle + dy * cosAngle);
            }
            for (int i = 0; i < numVertices; ++i)
            {
                nx = ny = 0;
                if (i < vertices.Length - 1)
                {
                    if (!reverseNormals)
                    {
                        nx = vertices[i + 1].X - vertices[i].X;
                        ny = vertices[i + 1].Y - vertices[i].Y;
                    }
                    else
                    {
                        nx = vertices[i].X - vertices[i + 1].X;
                        ny = vertices[i].Y - vertices[i + 1].Y;
                    }
                    normals[i].X = -ny;
                    normals[i].Y = nx;
                    normals[i].Normalize();
                }
                else
                {
                    if (!reverseNormals)
                    {
                        nx = vertices[0].X - vertices[i].X;
                        ny = vertices[0].Y - vertices[i].Y;
                    }
                    else
                    {
                        nx = vertices[i].X - vertices[0].X;
                        ny = vertices[i].Y - vertices[0].Y;
                    }
                    normals[i].X = -ny;
                    normals[i].Y = nx;
                    normals[i].Normalize();
                }
            }
        }
    }
}
