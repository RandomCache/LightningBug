using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightningBug
{
    class LBMath
    {
        const float degreeAdjust = 0.01745329251994329576923690768489f; // π / 180
        
        public static float DegreesToRad(float degree)
        {
           return degree * degreeAdjust;
        }
    }
}
