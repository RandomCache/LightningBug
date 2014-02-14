using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

// Class for debugging.

#if DEBUG

namespace LightningBug
{
    public class Debug
    {
        public ObjectTrack player, enemy;

        public struct ObjectState
        {
            public Vector2 position, velocity, center, direction;
            public float rotationSpeed, rotationAngleRads;
        }

        public class ObjectTrack
        {
            // States of the object. [0] is the current frame
            public ObjectState[] states;

            public ObjectTrack()
            {
                states = new ObjectState[2];
            }

            public void SetCurrentState(Object obj)
            {
                states[0].position = obj.GetPosition();
                states[0].velocity = obj.Velocity;
                states[0].center = obj.GetCenter();
                states[0].direction = obj.Direction;
                states[0].rotationSpeed = obj.GetRotationSpeed();
                states[0].rotationAngleRads = obj.GetRotation();
            }
        }

        public Debug()
        {
            player = new ObjectTrack();
            enemy = new ObjectTrack();
        }
    }
}

#endif
