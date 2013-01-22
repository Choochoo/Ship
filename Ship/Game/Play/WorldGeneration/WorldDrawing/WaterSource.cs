#region

using System;

#endregion

namespace Ship.Game.Play.WorldGeneration.WorldDrawing
{
    [Serializable] public class WaterSource
    {
        public WaterSource() { }

        public WaterSource(bool isFlowDirection, byte thickness)
        {
            //IsMain = isMain;
            IsFlowDirection = isFlowDirection;
            Thickness = thickness;
        }

        public byte Thickness { get; set; }
        // public bool IsMain { get; private set; }
        public bool IsFlowDirection { get; set; }
    }
}