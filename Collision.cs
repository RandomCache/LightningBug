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
            // Lastly check against the level
            Level curLevel = homeClass.GetCurLevel();

        }

        public void RectRect(Rectangle rect1, Rectangle rect2)
        {
        }
    }
}
