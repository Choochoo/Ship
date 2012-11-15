using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class RiverCoord
    {
        public RiverCoord(int pointX, int pointY, byte riverSize) 
        { 
            PointX = pointX;
            PointY = pointY;
            RiverSize = riverSize;
        }

        public int PointX { get; private set; }

        public int PointY { get; private set; }

        public byte RiverSize { get; set; }
    }
}
