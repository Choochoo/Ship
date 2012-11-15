

namespace Ship.Game.Beans.Events
{
    public class MoveArgs
    {
        private readonly byte _direction;
        public const byte Up = 1;
        public const byte Down = 2;
        public const byte Left = 3;
        public const byte Right = 4;

        public MoveArgs(byte direction)
        {
            _direction = direction;
        }

        public byte Direction { get { return _direction; } }
    }
}
