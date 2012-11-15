
using Ship.Game.Loaders;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ship.Game.WorldGeneration;

namespace Ship.Game.Beans.Managers
{
    internal class LoaderBaseManager
    {
        public LoaderBase[][] Loaders;

        public LoaderBaseManager()
        {
            Loaders = new LoaderBase[3][];
                for (var i = 0; i < 3; i++)
                    Loaders[i] = new LoaderBase[3];

            for (var i = 0; i < 3; i = i + 1)
            {
                for (var j = 0; j < 3; j = j + 1)
                {
                    var lb = new LoaderBase();
                    Loaders[i][j] = lb;
                    Loaders[i][j].LoadSector(Managers.SpawnX + (i - 1), Managers.SpawnY + (j - 1));
                }
            }

            CreateWorldTerrain.UpdateSpace(Managers.SpawnX,Managers.SpawnY);
        }

        public void RotateTilesUp()
        {
            // System.out.println("before up");
            // showTiles();
            foreach (LoaderBase[] t in Loaders)
            {
                var firstLoader = t[Loaders.Length - 1];
                for (var j = Loaders[0].Length - 1; j >= 0; j--)
                {
                    if (j == 0)
                    {
                        t[j] = firstLoader;
                        firstLoader.LoadSector(t[j + 1].SectorX, t[j + 1].SectorY - 1);
                    }
                    else
                        t[j] = t[j - 1];
                }
            }
            
        }

        public void RotateTilesDown()
        {
            // System.out.println("before down");
            // showTiles();
            foreach (LoaderBase[] t in Loaders)
            {
                var firstLoader = t[0];
                for (int j = 1, len = Loaders[0].Length; j < len; j++)
                {
                    t[j - 1] = t[j];
                    if (j != len - 1) continue;
                    t[j] = firstLoader;
                    firstLoader.LoadSector(t[j - 1].SectorX, t[j - 1].SectorY + 1);
                }
            }
        }

        public void RotateTilesLeft()
        {
            var firstLoader = Loaders[Loaders.Length - 1];
            for (var j = Loaders[0].Length - 1; j > 0; j--)
                Loaders[j] = Loaders[(j - 1)];

            Loaders[0] = firstLoader;

            for (var k = 0; k < Loaders[0].Length; k++)
                Loaders[0][k].LoadSector(Loaders[1][k].SectorX - 1, Loaders[1][k].SectorY);

        }

        public void RotateTilesRight()
        {
            var firstLoader = Loaders[0];

            for (var j = 1; j < Loaders.Length; j++)
                Loaders[(j - 1)] = Loaders[j];

            Loaders[Loaders.Length - 1] = firstLoader;

            for (int k = 0, len = Loaders[Loaders.Length - 1].Length; k < len; k++)
                Loaders[len - 1][k].LoadSector(Loaders[len - 2][k].SectorX + 1,
                                                    Loaders[len - 2][k].SectorY);

        }
    }
}
