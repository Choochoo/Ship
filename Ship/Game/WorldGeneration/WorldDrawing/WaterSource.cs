using System;
namespace Ship.Game.WorldGeneration.WorldDrawing
{
    [Serializable]
    public class WaterSource
    {

        public byte Thickness { get; set; }
       // public bool IsMain { get; private set; }
        public bool IsFlowDirection { get; set; }

        public WaterSource() { }

        public WaterSource(bool isFlowDirection, byte thickness)
        {
            //IsMain = isMain;
            IsFlowDirection = isFlowDirection;
            Thickness = thickness;
        }
    }
}
