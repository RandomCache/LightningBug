using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightningBug
{
    public class Collision
    {
        LightningBug homeClass;
        private static Collision collision;

        private Collision(LightningBug main)
        {
            homeClass = main;
        }

        public static Collision Instance(LightningBug main)
        {
            if (collision == null)
                collision = new Collision(main);

            return collision;
        }

        // Checks the given ship against the edges of the level and all colliding objects
        public void CheckShip(Ship toCheck)
        {
            toCheck.testColor = Color.White;
            // First check this ship against the players ship
            Ship playerShip = homeClass.GetPlayersShip();
            if (playerShip.GetId() != toCheck.GetId())
            {
                if (CheckIntersection(toCheck.collisionPolygons, playerShip.collisionPolygons))
                    toCheck.testColor = Color.Green;
            }
            //enemyShips
            foreach(Ship enemy in homeClass.enemyShips)
            {
                if (enemy.GetId() == toCheck.GetId())
                    continue;
                if (CheckIntersection(toCheck.collisionPolygons, enemy.collisionPolygons))
                    toCheck.testColor = Color.Green;
            }

            // Lastly check against the level
            Level curLevel = homeClass.GetCurLevel();
        }

        private bool CheckIntersection(List<Physics.Polygon> objectPolys1, List<Physics.Polygon> objectPolys2)
        {
            Vector2 curPoint, curDirection;
            foreach(Physics.Polygon poly1 in objectPolys1)
            {
                foreach(Physics.Polygon poly2 in objectPolys2)
                {
                    int i, j;
                    for (i = poly1.numVertices - 1, j = 0; j < poly1.numVertices; i = j++)
                    {
                        curPoint = poly1.vertices[j];
                        curDirection = poly1.normals[i];

                        if (WhichSide(poly2, curPoint, curDirection) > 0)
                            return false;
                    }

                    for (i = poly2.numVertices - 1, j = 0; j < poly2.numVertices; i = j++)
                    {
                        curPoint = poly2.vertices[j];
                        curDirection = poly2.normals[i];

                        if (WhichSide(poly1, curPoint, curDirection) > 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private int WhichSide(Physics.Polygon poly, Vector2 point, Vector2 direction)
        {
            int posCount, negCount, zeroCount;
            posCount = negCount = zeroCount = 0;
            for (int i = 0; i < poly.numVertices; ++i)
            {
                float time = Vector2.Dot(direction, poly.vertices[i] - point);
                if (time > 0)
                    posCount++;
                else if (time < 0)
                    negCount++;
                else
                    zeroCount++;
                
                //if (posCount > 0 && negCount > 0)//cd
                if ((posCount > 0 && negCount > 0) || zeroCount > 0)
                    return 0;
            }
            
            if (posCount > 0)
                return 1;
            else
                return -1;
            
            //return (zeroCount == 0 ? (posCount > 0 ? 1 : -1) : 0);//cd
        }
    }
}
