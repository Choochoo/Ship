using System;
using Ship.Game.WorldGeneration.WorldDrawing;
namespace Ship.Game.WorldGeneration
{
    [Serializable]
    public class WorldData
    {
        public byte[][] HeightData { get; set; }
        public byte[][] DesertData { get; set; }
        public byte[][] ForestData { get; set; }
        public byte[][] GrassData { get; set; }
        public byte[][] TempData { get; set; }
        public byte[][] HeightShowData { get; set; }
        public byte[][] ForestShowData { get; set; }
        public byte[][] GrassShowData { get; set; }
        public WaterSquare[][] WaterSquareData { get; set; }


        public WorldData()
	    {// DONT' DELETE NEED FOR SERIALIZATION
	    }

    }
}