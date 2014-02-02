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

        public Polygon(Vector2 newPos, Vector2[] offests, int numberOfVertices)
        {
            numVertices = numberOfVertices;
            vertexOffests = new Vector2[numVertices];
            nonRotatedVertices = new Vector2[numVertices];
            vertices = new Vector2[numVertices];
            normals = new Vector2[numVertices];
            if (offests.Length != numVertices)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Shapes::Shapes(Vector2 newPos, Vector2[] offests, int numberOfVertices): offests.Length != numVertices\n");
                System.Environment.Exit(0);
            }
            for (int i = 0; i < numVertices; ++i)
            {
                vertexOffests[i] = offests[i];
            }
        }
        public void SetPosition(Vector2 newPos, Point center, float radAngle)
        {
            //if we define dx=x2-x1 and dy=y2-y1, then the normals are (-dy, dx) and (dy, -dx).
            for (int i = 0; i < numVertices; ++i)
            {
                nonRotatedVertices[i] = newPos + vertexOffests[i];

                double cosAngle = Math.Cos(radAngle);
                double sinAngle = Math.Sin(radAngle);
                double dx = (newPos.X - center.X);
                double dy = (newPos.Y - center.Y);

                vertices[i].X = center.X + (int)(dx * cosAngle - dy * sinAngle);
                vertices[i].Y = center.Y + (int)(dx * sinAngle + dy * cosAngle);
                float nx = 0, ny = 0;
                if (i < numVertices - 1)
                {
                    nx = vertices[i+1].X - vertices[i].X;
                    ny = vertices[i+1].Y - vertices[i].Y;
                }
                else if (i == numVertices - 1)
                {
                    nx = vertices[0].X - vertices[i].X;
                    ny = vertices[0].Y - vertices[i].Y;
                }
                else
                    Logging.Instance(Logging.DEFAULTLOG).Log("Shapes::SetPosition(): Index >= numVertices\n");
                normals[i].X = -ny;
                normals[i].Y = nx;
            }
        }
    }
}
