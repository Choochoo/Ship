#region



#endregion

namespace Ship.Game.Beans.Tiles
{
    public class WaterPoint
    {
        public const byte Up = 1;
        public const byte Down = 2;
        public const byte Left = 3;
        public const byte Right = 4;
        private readonly int _mySectorX;


        private readonly int _mySectorY;
        private readonly int _value;
        private readonly int _x;

        private readonly int _y;

        public WaterPoint(int xVal, int yVal, byte directionValue, int sectorX,
                          int sectorY)
        {
            _x = xVal;
            _y = yVal;
            _mySectorX = sectorX;
            _mySectorY = sectorY;
            _value = directionValue;
        }

        public int X { get { return _x; } }

        public int Y { get { return _y; } }

        public int MySectorX { get { return _mySectorX; } }

        public int MySectorY { get { return _mySectorY; } }

        public int Value { get { return _value; } }

        public WaterPoint Clone(byte direction, int sectorX, int sectorY) { return new WaterPoint(X, Y, direction, sectorX, sectorY); }
    }
}