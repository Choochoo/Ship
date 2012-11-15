using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ship.Game.Loaders
{
    public class SectorData
    {
        public byte[,] TileData { get; set; }
        public byte[,] DecorData { get; set; }

        public SectorData()
        {
            TileData = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];
            DecorData = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];
        }

        public SectorData(byte[,] td, byte[,] dd) 
        { 
            TileData = td;
            DecorData = dd;
        }

        public void Read(string filename)
        {
            using (var fsSource = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fsSource.Length];

                fsSource.Read(buffer, 0, buffer.Length);

                var count = 0;
                for (var x = 0; x < MainGame.SectorTileSize; x++)
                {
                    for (var y = 0; y < MainGame.SectorTileSize; y++)
                    {
                        DecorData[x, y] = buffer[count];
                        count++;
                    }
                }

                for (var x = 0; x < MainGame.SectorTileSize; x++)
                {
                    for (var y = 0; y < MainGame.SectorTileSize; y++)
                    {
                        TileData[x, y] = buffer[count];
                        count++;
                    }
                }
            }
        }

        public void Write(string filename)
        {

            try
            {
                var fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                var result = new byte[DecorData.Length * sizeof(byte)];
                Buffer.BlockCopy(DecorData, 0, result, 0, result.Length);

                var resultTile = new byte[TileData.Length * sizeof(byte)];
                Buffer.BlockCopy(TileData, 0, resultTile, 0, resultTile.Length);

                fileStream.Write(result, 0, result.Length);
                fileStream.Write(resultTile, 0, resultTile.Length);
                fileStream.Close();
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());

            }
        }
    }
}
