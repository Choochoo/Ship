namespace Ship.Game.WorldGeneration.Noise
{
    public class WaterData
    {
        public const byte Top = 1;
        public const byte Bottom = 2;
        public const byte Left = 3;
        public const byte Right = 4;

        public readonly byte Thickness;
        public readonly byte Direction;

        public WaterData( byte thickness, byte direction)
        {
            Direction = direction;
            Thickness = thickness;
        }
    }
}
