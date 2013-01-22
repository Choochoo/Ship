

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public static class MapData
    {
        private static int _width = int.MinValue;

        public static int Width { get { return _width; } set { _width = value; } }
        private static int _height = int.MinValue;

        public static int Height { get { return _height; } set { _height = value; } }
    }
}
