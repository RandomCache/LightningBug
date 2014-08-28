using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightningBug.Isometric
{
    public class TileMap
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int MapWidth = 50;
        public int MapHeight = 50;

        public void ChangeBaseTile(Vector2 coords, int newTileId)
        {
            Rows[(int)coords.Y].Columns[(int)coords.X].BaseTile = newTileId;
        }

        // Clear all the height tiles from a specific tile
        public void ClearHeightTiles(Vector2 coords)
        {
            Rows[(int)coords.Y].Columns[(int)coords.X].HeightTiles.Clear();
        }

        public Vector2 WorldToMapCell(Vector2 worldPoint, out Vector2 localPoint)
        {
            Vector2 mapCell = new Vector2((int)(worldPoint.X / Tile.TileWidth),
               ((int)(worldPoint.Y / Tile.HeightTileOffset)) * 2);

            float localPointX = worldPoint.X % Tile.TileWidth;
            float localPointY = worldPoint.Y % Tile.HeightTileOffset;
            localPoint = new Vector2(localPointX, localPointY);
            int dx = 0;
            int dy = 0;

            int width = Tile.TileStepX;
            int height = Tile.TileStepY * 2;

            Vector2 topLeft, topRight, bottomRight, bottomLeft;
            topLeft = Vector2.Zero;
            topRight.X = width; topRight.Y = 0;
            bottomRight.X = width; bottomRight.Y = height;
            bottomLeft.X = 0; bottomLeft.Y = height;

            Vector2 topCenter, rightCenter, bottomCenter, leftCenter;
            topCenter.X = width / 2; topCenter.Y = 0;
            rightCenter.X = width; rightCenter.Y = height / 2;
            bottomCenter.X = width / 2; bottomCenter.Y = height;
            leftCenter.X = 0; leftCenter.Y = height / 2;

            // Now check to see if the point is in any of the corners.
            if (PointInTriangle(localPoint, topLeft, topCenter, leftCenter)) // top left
            {
                dx = -1;
                dy = -1;
            }
            else if (PointInTriangle(localPoint, topCenter, topRight, rightCenter)) // top right
            {
                dy = -1;
            }
            else if (PointInTriangle(localPoint, rightCenter, bottomRight, bottomCenter)) // bottom right
            {
                dy = 1;
            }
            else if (PointInTriangle(localPoint, bottomCenter, bottomLeft, leftCenter)) // bottom left
            {
                dx = -1;
                dy = 1;
            }

            mapCell.X += dx;
            mapCell.Y += dy - 2;
            
            return mapCell;
        }

        private bool PointInTriangle(Vector2 point, Vector2 vertex1, Vector2 vertex2, Vector2 vertex3)
        {
            float c1, c2, c3;
            c1 = CrossProduct(vertex1 - vertex2, vertex1 - point);
            c2 = CrossProduct(vertex2 - vertex3, vertex2 - point);
            c3 = CrossProduct(vertex3 - vertex1, vertex3 - point);
            if ((c1 <= 0 && c2 <= 0 && c3 <= 0) ||
                (c1 >= 0 && c2 >= 0 && c3 >= 0))
            {
                return true;
            }
            else
                return false;
        }

        float CrossProduct(Vector2 v1, Vector2 v2)
        {
            return (v1.X *v2.Y) - (v1.Y*v2.X);
        }

        public MapCell GetCellAtWorldPoint(Vector2 worldPoint)
        {
            Vector2 tempOut;
            Vector2 mapPoint = WorldToMapCell(worldPoint, out tempOut);
            return Rows[(int)mapPoint.Y].Columns[(int)mapPoint.X];
        }

        public TileMap()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < MapWidth; x++)
                {
                    thisRow.Columns.Add(new MapCell(0));
                }
                Rows.Add(thisRow);
            }

            /*
            Moved into the level xml


            // Create Sample Map Data
            Rows[0].Columns[3].TileID = 3;
            Rows[0].Columns[4].TileID = 3;
            Rows[0].Columns[5].TileID = 1;
            Rows[0].Columns[6].TileID = 1;
            Rows[0].Columns[7].TileID = 1;

            Rows[1].Columns[3].TileID = 3;
            Rows[1].Columns[4].TileID = 1;
            Rows[1].Columns[5].TileID = 1;
            Rows[1].Columns[6].TileID = 1;
            Rows[1].Columns[7].TileID = 1;

            Rows[2].Columns[2].TileID = 3;
            Rows[2].Columns[3].TileID = 1;
            Rows[2].Columns[4].TileID = 1;
            Rows[2].Columns[5].TileID = 1;
            Rows[2].Columns[6].TileID = 1;
            Rows[2].Columns[7].TileID = 1;

            Rows[3].Columns[2].TileID = 3;
            Rows[3].Columns[3].TileID = 1;
            Rows[3].Columns[4].TileID = 1;
            Rows[3].Columns[5].TileID = 2;
            Rows[3].Columns[6].TileID = 2;
            Rows[3].Columns[7].TileID = 2;

            Rows[4].Columns[2].TileID = 3;
            Rows[4].Columns[3].TileID = 1;
            Rows[4].Columns[4].TileID = 1;
            Rows[4].Columns[5].TileID = 2;
            Rows[4].Columns[6].TileID = 2;
            Rows[4].Columns[7].TileID = 2;

            Rows[5].Columns[2].TileID = 3;
            Rows[5].Columns[3].TileID = 1;
            Rows[5].Columns[4].TileID = 1;
            Rows[5].Columns[5].TileID = 2;
            Rows[5].Columns[6].TileID = 2;
            Rows[5].Columns[7].TileID = 2;

            // Height tiles
            Rows[16].Columns[4].AddHeightTile(54);

            Rows[17].Columns[3].AddHeightTile(54);

            Rows[15].Columns[3].AddHeightTile(54);
            Rows[16].Columns[3].AddHeightTile(53);

            Rows[15].Columns[4].AddHeightTile(54);
            Rows[15].Columns[4].AddHeightTile(54);
            Rows[15].Columns[4].AddHeightTile(51);

            Rows[18].Columns[3].AddHeightTile(51);
            Rows[19].Columns[3].AddHeightTile(50);
            Rows[18].Columns[4].AddHeightTile(55);

            Rows[14].Columns[4].AddHeightTile(54);

            Rows[14].Columns[5].AddHeightTile(62);
            Rows[14].Columns[5].AddHeightTile(61);
            Rows[14].Columns[5].AddHeightTile(63);
            
            // Topper Tiles
            Rows[17].Columns[4].AddProp(114);
            Rows[16].Columns[5].AddProp(115);
            Rows[14].Columns[4].AddProp(125);
            Rows[15].Columns[5].AddProp(91);
            Rows[16].Columns[6].AddProp(94);

            Rows[15].Columns[5].Walkable = false;
            Rows[16].Columns[6].Walkable = false;
            // End Create Sample Map Data

            */
        }
    }

    public class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }
}
