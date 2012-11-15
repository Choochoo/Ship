#region

using System;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ship.Game.WorldGeneration;
using System.Runtime.Serialization.Formatters.Binary;
using Ship.Game.WorldGeneration.Noise;

#endregion

namespace Ship.Game.Utils
{
    public static class Utility
    {
        public const string SaveMapName = "map.dat";
        public const string SaveMapFolder = "Mapdata";

        private static int _timestart = 0;

        public static void TimeStart() 
        { 
            _timestart = System.Environment.TickCount;
        }

        public static void TimeEnd()
        {
            System.Diagnostics.Debug.WriteLine("Timer Ended: {0}",(System.Environment.TickCount-_timestart));
        }

        public static Color IntToColor(int newColor)
        {
            var red = (newColor >> 16) & 0xFF;
            var green = (newColor >> 8) & 0xFF;
            var blue = newColor & 0xFF;
            return new Color(red,green,blue);
        }

        //public static void ClearArray(ref byte[][] output)
        //{
        //    for (var x = 0; x < output.Length; x++)
        //        for (var y = 0; y < output.Length; y++)
        //            output[x][y] = 0;
        //}

        //public static void ClearArray(ref ushort[][] output)
        //{
        //    for (var x = 0; x < output.Length; x++)
        //        for (var y = 0; y < output.Length; y++)
        //            output[x][y] = 0;
        //}

        //public static void ClearArray(ref byte[,] output)
        //{
        //    var size = output.GetLength(0);
        //    for (var x = 0; x < size; x++)
        //        for (var y = 0; y < size; y++)
        //            output[x,y] = 0;
        //}

        //public static bool SaveSettings(WorldData wd)
        //{
        //    // Open a storage container
        //    var device = MainGame.Game.StorageD;
        //    var result = device.BeginOpenContainer(SaveMapFolder, null, null);
        //    // Wait for the WaitHandle to become signaled.
        //    result.AsyncWaitHandle.WaitOne();

        //    var container = device.EndOpenContainer(result);


        //    // Check to see whether the save exists.
        //    if (container.FileExists(SaveMapName))
        //        // Delete it so that we can create one fresh.
        //        container.DeleteFile(SaveMapName);

        //    Stream stream = container.CreateFile(SaveMapName);

        //    // Convert the object to XML data and put it in the stream
        //    var serializer = new XmlSerializer(typeof (WorldData));
        //    serializer.Serialize(stream, wd);

        //    // Close the file
        //    stream.Close();

        //    // Dispose the container, to commit changes
        //    container.Dispose();
        //    result.AsyncWaitHandle.Close();
        //    return true;
        //}

       

        public static void SaveGameData(WorldData wd)
        {
            Stream stream = File.Open(SaveMapName, FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, wd);
            stream.Close();
        }

        public static WorldData LoadGameData()
        {
            if (!File.Exists(SaveMapName)) return null;
            Stream stream = File.Open(SaveMapName, FileMode.Open);
            var binaryFormatter = new BinaryFormatter();
            var objectToBeDeSerialized = (WorldData)binaryFormatter.Deserialize(stream);
            stream.Close();
            return objectToBeDeSerialized;
        }

        //public static WorldData LoadLand()
        //{
        //    var device = MainGame.Game.StorageD;
        //    var result = device.BeginOpenContainer(SaveMapFolder, null, null);
        //    // Wait for the WaitHandle to become signaled.
        //    result.AsyncWaitHandle.WaitOne();

        //    var container = device.EndOpenContainer(result);

        //    // Close the wait handle.
        //    result.AsyncWaitHandle.Close();


        //    // Check to see whether the save exists.
        //    if (!container.FileExists(SaveMapName))
        //    {
        //        // If not, dispose of the container and return.
        //        container.Dispose();
        //        System.Diagnostics.Debug.WriteLine("Error: Map could not load");
        //        return null;
        //    }

        //    var stream = container.OpenFile(SaveMapName, FileMode.Open);
        //    var serializer = new XmlSerializer(typeof (WorldData));
        //    var data = (WorldData) serializer.Deserialize(stream);
        //    stream.Close();
        //    container.Dispose();
        //    System.Diagnostics.Debug.WriteLine("Map Successfully loaded");
        //    return data;
        //}

        public static void PrintArray(byte[,] byte1, byte[][]byte2)
        {
            if (byte1 != null)
            {
                for(int y = 0, len = byte1.GetLength(0); y < len; y++)
                {
                    for (var x = 0; x < len; x++)
                    {
                        System.Diagnostics.Debug.Write(byte1[x,y]+",");
                    }
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
            if (byte2 == null) return;
            for (int y = 0, len = byte2.GetLength(0); y < len; y++)
            {
                for (var x = 0; x < len; x++)
                {
                    System.Diagnostics.Debug.Write(byte2[x][ y] + ",");
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }

        internal  static void RemoveSave()
        {
            if (!File.Exists(SaveMapName))
                File.Delete(SaveMapName);
        }

        internal static void RemoveSettingsSave()
        {
            // Open a storage container
            var device = MainGame.StorageD;
            var result = device.BeginOpenContainer(SaveMapFolder, null, null);
            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            var container = device.EndOpenContainer(result);


            // Check to see whether the save exists.
            if (container.FileExists(SaveMapName))
                // Delete it so that we can create one fresh.
                container.DeleteFile(SaveMapName);
        }

        
    }
}