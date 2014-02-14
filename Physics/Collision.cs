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

        // Checks collision of the given object against the edges of the level
        public void CheckShip(Object toCheck, Level level)
        {
            bool currentHit = false;
            Vector2 translationVector, centerDiff;
            translationVector = Vector2.Zero;
            centerDiff = new Vector2(level.GetLevelWidth() - toCheck.GetCenter().X, level.GetLevelHeight() - toCheck.GetCenter().Y);
            if (CheckIntersection(toCheck.collisionPolygons, level.collisionPolygons, centerDiff, ref translationVector, true))
            {
                toCheck.SetPosition(new Vector2(toCheck.GetPosition().X + translationVector.X, toCheck.GetPosition().Y + translationVector.Y));
                currentHit = true; ;
            }

            if (!currentHit)
            {
                //If they are not currently intersecting, check if they will intersect
                Vector2 oldPosition1 = toCheck.GetPosition();
                toCheck.SetPosition(new Vector2(oldPosition1.X + toCheck.Velocity.X, oldPosition1.Y + toCheck.Velocity.Y));
                bool futureRet = CheckIntersection(toCheck.collisionPolygons, level.collisionPolygons, centerDiff, ref translationVector, true);

                if (futureRet)
                {
                    // Set the position back now that the future collision has been checked
                    toCheck.SetPosition(oldPosition1);
                    currentHit = true;
                }
            }
            if (currentHit)
            {
                float speed = toCheck.Velocity.Length();
                toCheck.Velocity = Vector2.Normalize(translationVector);
                toCheck.Velocity *= speed;
            }
        }

        // Checks collision between the two given objects
        public void CheckShip(Object toCheck, Object checkAgainst)
        {
            bool collided = false;
            Vector2 translationVector1, translationVector2;
            translationVector1 = translationVector2 = Vector2.Zero;
            toCheck.testColor = Color.White;

            Vector2 centerDiff;
            //Check if the two objects are intersecting, then if they will
            centerDiff = toCheck.GetCenter() - checkAgainst.GetCenter();
            if (CheckIntersection(toCheck.collisionPolygons, checkAgainst.collisionPolygons, centerDiff, ref translationVector1))
            {
                translationVector2 = -translationVector1;
                toCheck.SetPosition(new Vector2(toCheck.GetPosition().X + translationVector1.X, toCheck.GetPosition().Y + translationVector1.Y));
                checkAgainst.SetPosition(new Vector2(checkAgainst.GetPosition().X + translationVector2.X, checkAgainst.GetPosition().Y + translationVector2.Y));
                collided = true;
            }
            else
            {
                //If they are not currently intersecting, check if they will intersect
                Vector2 oldPosition1 = toCheck.GetPosition();
                Vector2 oldPosition2 = checkAgainst.GetPosition();
                toCheck.SetPosition(new Vector2(oldPosition1.X + toCheck.Velocity.X, oldPosition1.Y + toCheck.Velocity.Y));
                checkAgainst.SetPosition(new Vector2(oldPosition2.X + checkAgainst.Velocity.X, oldPosition2.Y + checkAgainst.Velocity.Y));
                bool futureRet = CheckIntersection(toCheck.collisionPolygons, checkAgainst.collisionPolygons, centerDiff, ref translationVector1);

                if (futureRet)
                {
                    translationVector2 = -translationVector1;
                    // Set the position back now that the future collision has been checked
                    toCheck.SetPosition(oldPosition1);
                    checkAgainst.SetPosition(oldPosition2);

                    collided = true;
                }
            }

            if(collided)
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
                    toCheck.SetSpeed(toCheckSpeed);
                    checkAgainst.SetSpeed(checkAgainstSpeed);
                }
                float per1 = toCheckSpeed / totalSpeed;
                float per2 = checkAgainstSpeed / totalSpeed;
                toCheck.SetSpeed(totalSpeed * per1);
                checkAgainst.SetSpeed(totalSpeed * per2);
            }
        }

        private bool CheckIntersection(List<Physics.Polygon> objectPolys1, List<Physics.Polygon> objectPolys2,
            Vector2 centerDiffs, ref Vector2 translationVector, bool singleSeg2 = false)
        {
            Vector2 curPoint, curDirection;
            float minIntervalDistance, curMinDistance;
            Vector2 minAxis = Vector2.Zero;
            minIntervalDistance = curMinDistance = float.PositiveInfinity;
            // Project each polygons verts onto each edge of the other polygon
            foreach(Physics.Polygon poly1 in objectPolys1)
            {
                foreach(Physics.Polygon poly2 in objectPolys2)
                {
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
                            if(float.IsNaN(minIntervalDistance) || float.IsNaN(minAxis.X) || float.IsNaN(minAxis.Y))
                                System.Diagnostics.Debugger.Break();

                            if (Vector2.Dot(centerDiffs, minAxis) < 0)
                                minAxis = -minAxis;
                        }
                    }

                    int hitCount = 0;
                    for (i = poly2.numVertices - 1, j = 0; j < poly2.numVertices; i = j++)
                    {
                        curPoint = poly2.vertices[j];
                        curDirection = poly2.normals[i];
                        curMinDistance = float.PositiveInfinity;
                        if (WhichSide(poly1, curPoint, curDirection, ref curMinDistance) > 0)
                        {
                            if(!singleSeg2)
                                return false;                            
                        }
                        else
                            ++hitCount;
                        if (curMinDistance < minIntervalDistance)
                        {
                            minIntervalDistance = curMinDistance;
                            minAxis = curDirection;

                            if (float.IsNaN(minIntervalDistance) || float.IsNaN(minAxis.X) || float.IsNaN(minAxis.Y))
                                System.Diagnostics.Debugger.Break();

                            if (Vector2.Dot(centerDiffs, minAxis) < 0)
                                minAxis = -minAxis;
                        }
                    }
                    if (singleSeg2 && hitCount < 1)
                        return false;
                }
            }

            minAxis.Normalize();
            translationVector = minAxis * minIntervalDistance;
            return true;
        }

        // Does the entire polygon fall to one direction of point on the line defined by point and direction
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
                
                if ((posCount > 0 && negCount > 0) || zeroCount > 0)
                    return 0;
            }
            
            if (posCount > 0)
                return 1;
            else
                return -1;
        }
    }
}
