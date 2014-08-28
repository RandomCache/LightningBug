using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightningBug.Isometric
{
    public class MapCell
    {
        /*
        public int TileID
        {
            get { return BaseTiles.Count > 0 ? BaseTiles[0] : 0; }
            set
            {
                if (BaseTiles.Count > 0)
                    BaseTiles[0] = value;
                else
                    AddBaseTile(value);
            }
        }*/

        public bool Walkable { get; set; }

        //public List<int> BaseTiles = new List<int>();
        public int BaseTile { get; set; }
        public List<int> HeightTiles = new List<int>();
        public List<int> Props = new List<int>();

        public MapCell(int tileID)
        {
            //TileID = tileID;
            BaseTile = tileID;
            Walkable = true;
        }
        /*
        public void AddBaseTile(int tileID)
        {
            BaseTiles.Add(tileID);
        }
        */
        public void SetBaseTile(int tileId)
        {
            BaseTile = tileId;
        }
        public void AddHeightTile(int tileId)
        {
            HeightTiles.Add(tileId);
        }

        public void AddProp(int tileId)
        {
            Props.Add(tileId);
        }
    }
}
