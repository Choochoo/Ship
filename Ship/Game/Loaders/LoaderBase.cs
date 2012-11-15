
#region
using Ship.Game.WorldGeneration;
using Ship.Game.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Ship.Game.Beans.Managers;
#endregion

namespace Ship.Game.Loaders
{
    public class LoaderBase
    {

        #region GetSets

        public int SectorX { get; private set; }

        public int SectorY { get; private set; }

        #endregion

        public SectorData SectorData;

        public LoaderBase()
        {
           SectorData = new SectorData();
        }

        

        public void LoadSector(int sectorX, int sectorY)
        {
            Utility.TimeStart();
            SectorX = sectorX;
            SectorY = sectorY;
            var file = String.Format("gamedata/{0}-{1}", sectorX, sectorY);
               
           if(File.Exists(file))
           {
                  SectorData.Read(file);
           }
           else
           {
               SectorData =
                   CreateWorldTerrain.StartProcess(Managers.SpawnX == sectorX && Managers.SpawnY == sectorY,sectorX,
                                                   sectorY);
           }
            
            Utility.TimeEnd();
        }


        

    }
}