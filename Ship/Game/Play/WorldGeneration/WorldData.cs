#region

using System;
using Ship.Game.Play.WorldGeneration.WorldDrawing;

#endregion

namespace Ship.Game.Play.WorldGeneration
{
    [Serializable] public class WorldData
    {
        [NonSerialized] private static WorldData _myWorldData;
        public byte[][] HeightData { get; set; }
        public byte[][] DesertData { get; set; }
        public byte[][] ForestData { get; set; }
        public byte[][] GrassData { get; set; }
        //public byte[][] TempData { get; set; }
        public byte[][] HeightShowData { get; set; }
        public byte[][] ForestShowData { get; set; }
        public byte[][] GrassShowData { get; set; }
        public byte[][] FogData { get; set; }
        public WaterSquare[][] WaterSquareData { get; set; }


        public static WorldData MyWorldData { get { return _myWorldData; } set { _myWorldData = value; } }
    }
}