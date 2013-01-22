#region

using Ship.Game.Play.WorldGeneration.WorldDrawing;

#endregion

namespace Ship.Game.Play.WorldGeneration
{
    public abstract class MapBase
    {
        protected MapBase()
        {
            Data = new byte[MainGame.MapWidth][];
            for (var i = 0; i < MainGame.MapWidth; i++)
                Data[i] = new byte[MainGame.MapHeight];
        }

        public byte[][] Data { get; set; }
    }
}