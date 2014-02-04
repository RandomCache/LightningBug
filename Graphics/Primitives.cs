using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.Graphics
{
    public class Primitives
    {
        private VertexPositionColor[] lineVertices;
        private int lineVertsCount;
        private BasicEffect _basicEffect;
        private GraphicsDevice graphicsDevice;
        private Texture2D blank;

        public Primitives(int bufferSize = 100)
        {            
            lineVertices = new VertexPositionColor[bufferSize - bufferSize % 2];
        }

        public void Init(GraphicsDevice device)
        {
            graphicsDevice = device;
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
        }

        public void ClearAll()
        {
            lineVertsCount = 0;
        }

        public void AddLine(Vector2 v1, Vector2 v2)
        {
            AddLine(v1, v2, Color.Red);
        }

        public void AddLine(Vector2 v1, Vector2 v2, Color color)
        {
            // If we're over our max vert count, don't add another
            if (lineVertsCount + 1 >= lineVertices.Length)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Primitives::AddLine: lineVertsCount is greater than lineVertices.Length. \n");
                return;
            }
            lineVertices[lineVertsCount].Position = new Vector3(v1, 0f);
            lineVertices[lineVertsCount + 1].Position = new Vector3(v2, 0f);
            lineVertices[lineVertsCount].Color = lineVertices[lineVertsCount + 1].Color = color;
            lineVertsCount += 2;
        }

        public void DrawAllPrimitives(SpriteBatch sb)
        {
            if (lineVertsCount % 2 != 0)
            {
                Logging.Instance(Logging.DEFAULTLOG).Log("Primitives::DrawAllPrimitives: lineVertsCount is an odd number. \n");
                System.Environment.Exit(1); // Exitting in order to never not notice this error
            }
            Vector2 temp2;
            for(int i = 0; i < lineVertsCount; i+=2)
            {
                float angle = (float)Math.Atan2(lineVertices[i + 1].Position.Y - lineVertices[i].Position.Y,
                    lineVertices[i + 1].Position.X - lineVertices[i].Position.X);
                float length = Vector3.Distance(lineVertices[i].Position, lineVertices[i + 1].Position);
                temp2 = new Vector2(lineVertices[i].Position.X, lineVertices[i].Position.Y);
                sb.Draw(blank, temp2, null, lineVertices[i].Color, angle, Vector2.Zero, new Vector2(length, 2), SpriteEffects.None, 0);
            }
        }
    }
}
