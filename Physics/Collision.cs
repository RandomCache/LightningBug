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
        public void CheckShip(Object toCheck, Object checkAgainst)
        {
            Vector2 centerDiff, translationVector1, translationVector2, relativeVelocity;
            translationVector1 = translationVector2 = Vector2.Zero;
            toCheck.testColor = Color.White;
            /*
            // First check this ship against the players ship
            Ship playerShip = homeClass.GetPlayersShip();
            if (playerShip.GetId() != toCheck.GetId())
            {
                //centerDiff = playerShip.GetCenter() - toCheck.GetCenter();
                //relativeVelocity = toCheck.GetVelocity() - playerShip.GetVelocity();
                if (CheckIntersection(toCheck.collisionPolygons, playerShip.collisionPolygons, centerDiff, ref translationVector))
                    toCheck.testColor = Color.Green;
            }
            //enemyShips
            foreach(Ship enemy in homeClass.enemyShips)
            {
                if (enemy.GetId() == toCheck.GetId())
                    continue;
                //centerDiff = enemy.GetCenter() - toCheck.GetCenter();
                //relativeVelocity = toCheck.GetVelocity() - enemy.GetVelocity();
                if (CheckIntersection(toCheck.collisionPolygons, enemy.collisionPolygons, centerDiff, ref translationVector))
                    toCheck.testColor = Color.Green;
            }
            */

            if (CheckIntersection(toCheck, checkAgainst, ref translationVector1, ref translationVector2))
            {
                toCheck.testColor = Color.Green;
                float toCheckSpeed = toCheck.Velocity.Length();
                float checkAgainstSpeed = checkAgainst.Velocity.Length();
                toCheck.Velocity = translationVector1;
                checkAgainst.Velocity = translationVector2;
                
                // TODO Update to use mass
                float totalSpeed = toCheckSpeed + checkAgainstSpeed;
                if (totalSpeed <= 0)
                {
                    Logging.Instance(Logging.DEFAULTLOG).Log("Collision::CheckShip - Collision between two options with zero velocities");
                }
                float per1 = toCheckSpeed / totalSpeed;
                float per2 = checkAgainstSpeed / totalSpeed;
                toCheck.SetSpeed(totalSpeed * per1);
                checkAgainst.SetSpeed(totalSpeed * per2);
            }
            /*
            // Lastly check against the level boundries
            Level curLevel = homeClass.GetCurLevel();*/
            //return translationVector;
        }

        private bool CheckIntersection(Object obj1, Object obj2, ref Vector2 translationVector1, ref Vector2 translationVector2)
        {
            Vector2 centerDiff;
            //Check if the two objects are intersecting, then if they will
            centerDiff = obj1.GetCenter() - obj2.GetCenter();
            if (CheckIntersectionSub(obj1.collisionPolygons, obj2.collisionPolygons, centerDiff, ref translationVector1))
            {
                translationVector2 = -translationVector1;
                obj1.SetPosition(new Vector2(obj1.GetPosition().X + translationVector1.X, obj1.GetPosition().Y + translationVector1.Y));
                obj2.SetPosition(new Vector2(obj2.GetPosition().X + translationVector2.X, obj2.GetPosition().Y + translationVector2.Y));
                return true;
            }
            
            //If they are not currently intersecting, check if they will intersect
            Vector2 oldPosition1 = obj1.GetPosition();
            Vector2 oldPosition2 = obj2.GetPosition();
            obj1.SetPosition(new Vector2(oldPosition1.X + obj1.Velocity.X, oldPosition1.Y + obj1.Velocity.Y));
            obj2.SetPosition(new Vector2(oldPosition2.X + obj2.Velocity.X, oldPosition2.Y + obj2.Velocity.Y));
            bool futureRet = CheckIntersectionSub(obj1.collisionPolygons, obj2.collisionPolygons, centerDiff, ref translationVector1);

            if(futureRet)
            {
                translationVector2 = -translationVector1;
                // Set the position back now that the future collision has been checked
                obj1.SetPosition(oldPosition1);
                obj2.SetPosition(oldPosition2);

                return true;
            }
            
            return false;
        }

        private bool CheckIntersectionSub(List<Physics.Polygon> objectPolys1, List<Physics.Polygon> objectPolys2,
            Vector2 centerDiffs, ref Vector2 translationVector)
        {
            Vector2 curPoint, curDirection;
            float minIntervalDistance, curMinDistance;
            Vector2 minAxis = Vector2.Zero;
            minIntervalDistance = curMinDistance = float.PositiveInfinity;
            foreach(Physics.Polygon poly1 in objectPolys1)
            {
                foreach(Physics.Polygon poly2 in objectPolys2)
                {
                    // min and max intervals for these two polygons
                    float min1, max1, min2, max2;
                    min1 = min2 = float.PositiveInfinity;
                    max1 = max2 = float.NegativeInfinity;
                    int i, j;
                    for (i = poly1.numVertices - 1, j = 0; j < poly1.numVertices; i = j++)
                    {
                        curPoint = poly1.vertices[j];
                        curDirection = poly1.normals[i];
                        curMinDistance = float.PositiveInfinity;
                        if (WhichSide(poly2, curPoint, curDirection, ref curMinDistance) > 0)
                            return false;
                        if(curMinDistance < minIntervalDistance)
                        {
                            minIntervalDistance = curMinDistance;
                            minAxis = curDirection;

                            if (Vector2.Dot(centerDiffs, minAxis) < 0)
                                minAxis = -minAxis;
                        }
                    }

                    for (i = poly2.numVertices - 1, j = 0; j < poly2.numVertices; i = j++)
                    {
                        curPoint = poly2.vertices[j];
                        curDirection = poly2.normals[i];
                        curMinDistance = float.PositiveInfinity;
                        if (WhichSide(poly1, curPoint, curDirection, ref curMinDistance) > 0)
                            return false;
                        if (curMinDistance < minIntervalDistance)
                        {
                            minIntervalDistance = curMinDistance;
                            minAxis = curDirection;
                        }
                    }
                }
            }


            //minAxis.Y = minAxis.Y * -1;
            /*
            //Vector d = polygonA.Center - polygonB.Center;
            //if (centerDiffs.DotProduct(minAxis) < 0)
            if (Vector2.Dot(centerDiffs, minAxis) < 0)
                minAxis = -minAxis;
            */
            minAxis.Normalize();
            translationVector = minAxis * minIntervalDistance;
            return true;
        }

        private int WhichSide(Physics.Polygon poly, Vector2 point, Vector2 direction, ref float minDistance)
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
                if (Math.Abs(time) < minDistance)
                    minDistance = Math.Abs(time);
                
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
