#region

using System;

#endregion

namespace Ship.Game.Play.WorldGeneration.WorldDrawing
{
    [Serializable] public class WaterSquare
    {
        [NonSerialized] public const byte AdjescentNone = 5;
        [NonSerialized] public const byte AdjescentTop = 6;
        [NonSerialized] public const byte AdjescentBottom = 7;
        [NonSerialized] public const byte AdjescentLeft = 8;
        [NonSerialized] public const byte AdjescentRight = 9;

        [NonSerialized] private byte _highestSize = byte.MinValue;
        [NonSerialized] private byte _lowestSize = byte.MaxValue;

        public WaterSquare() { }

        public WaterSquare(int lx, int ly, WaterSource north, WaterSource south, WaterSource west, WaterSource east)
        {
            LocationX = lx;
            LocationY = ly;
            North = north;
            South = south;
            West = west;
            East = east;
        }

        public int LocationX { get; set; }
        public int LocationY { get; set; }

        public WaterSource North { get; set; }
        public WaterSource South { get; set; }
        public WaterSource West { get; set; }
        public WaterSource East { get; set; }

        public byte HighestSize
        {
            get
            {
                if (_highestSize == byte.MinValue)
                {
                    _highestSize = North != null && North.Thickness > _highestSize ? North.Thickness : _highestSize;
                    _highestSize = South != null && South.Thickness > _highestSize ? South.Thickness : _highestSize;
                    _highestSize = West != null && West.Thickness > _highestSize ? West.Thickness : _highestSize;
                    _highestSize = East != null && East.Thickness > _highestSize ? East.Thickness : _highestSize;
                }
                return _highestSize;
            }
            set { _highestSize = value; }
        }

        public byte LowestSize
        {
            get
            {
                if (_lowestSize == byte.MaxValue)
                {
                    _lowestSize = North != null && North.Thickness < _lowestSize ? North.Thickness : _lowestSize;
                    _lowestSize = South != null && South.Thickness < _lowestSize ? South.Thickness : _lowestSize;
                    _lowestSize = West != null && West.Thickness < _lowestSize ? West.Thickness : _lowestSize;
                    _lowestSize = East != null && East.Thickness < _lowestSize ? East.Thickness : _lowestSize;
                }
                return _lowestSize;
            }
            set { _lowestSize = value; }
        }
    }
}