#region

using Microsoft.Xna.Framework;
using Ship.Game.Play.Utils;
using System;
using System.Collections.Generic;

#endregion

namespace Ship.Game.Play.WorldGeneration.WorldDrawing
{
    public class WaterGenerator
    {
        private const byte RiverSizeDividedByPerSquare = 4;
        private const byte MinimumRiverSize = 1;
        private const byte MaximumRiverSize = 18;
        public WaterSquare[][] WaterSquares;
        private Random _rand = new Random(Seeds.WaterSeed);
        private List<RiverCoord>[][] _riverMap;
        private List<List<RiverCoord>> _rivers = new List<List<RiverCoord>>();

        public WaterGenerator()
        {
            _riverMap = new List<RiverCoord>[MainGame.MapWidth][];
            for (var i = 0; i < MainGame.MapWidth; i++)
                _riverMap[i] = new List<RiverCoord>[MainGame.MapHeight];

            int x, y;
            const double chanceOnHill = .6;
            const double chanceOnPlain = .2;
            for (x = 0; x < MainGame.MapWidth; x += 2)
            {
                for (y = 0; y < MainGame.MapHeight; y += 1)
                {
                    var heightPixel = WorldData.MyWorldData.HeightData[x][y];

                    if (heightPixel == HeightGenerator.Mountains)
                        CreateRiver(x, y);
                    else if (heightPixel == HeightGenerator.Hills && _rand.NextDouble() < chanceOnHill)
                        CreateRiver(x, y);
                    else if (heightPixel == HeightGenerator.Plains && _rand.NextDouble() < chanceOnPlain)
                        CreateRiver(x, y);
                }
            }


            WaterSquares = RiversToWaterSquares(MainGame.MapWidth, MainGame.MapHeight, _rivers);
        }

        public void Destroy()
        {
            _rivers.Clear();
            _riverMap = null;
            _rivers = null;
            _rand = null;
            WaterSquares = null;
        }

        public static WaterSquare[][] RiversToWaterSquares(int width, int height, List<List<RiverCoord>> rivers)
        {
            var output = new WaterSquare[width][];
            for (var i = 0; i < width; i++)
                output[i] = new WaterSquare[height];

            for (var i = 0; i < rivers.Count; i++)
            {
                var currRiver = rivers[i];
                for (var j = 0; j < currRiver.Count; j++)
                {
                    var riverCord = currRiver[j];
                    if (output[riverCord.PointX][riverCord.PointY] == null)
                    {
                        output[riverCord.PointX][riverCord.PointY] = new WaterSquare(riverCord.PointX, riverCord.PointY,
                                                                                     null, null, null, null);
                    }

                    var currWaterSquare = output[riverCord.PointX][riverCord.PointY];
                    var addedOne = false;
                    if (j - 1 >= 0)
                    {
                        addedOne = true;
                        var prevRiverCord = currRiver[j - 1];
                        if (prevRiverCord.PointY == riverCord.PointY - 1)
                            currWaterSquare.North = new WaterSource(false, prevRiverCord.RiverSize);
                        if (prevRiverCord.PointY == riverCord.PointY + 1)
                            currWaterSquare.South = new WaterSource(false, prevRiverCord.RiverSize);
                        if (prevRiverCord.PointX == riverCord.PointX - 1)
                            currWaterSquare.West = new WaterSource(false, prevRiverCord.RiverSize);
                        if (prevRiverCord.PointX == riverCord.PointX + 1)
                            currWaterSquare.East = new WaterSource(false, prevRiverCord.RiverSize);
                    }

                    if (j + 1 < currRiver.Count)
                    {
                        addedOne = true;
                        var nextRiverCord = currRiver[j + 1];
                        if (nextRiverCord.PointY == riverCord.PointY - 1)
                            currWaterSquare.North = new WaterSource(true, riverCord.RiverSize);
                        if (nextRiverCord.PointY == riverCord.PointY + 1)
                            currWaterSquare.South = new WaterSource(true, riverCord.RiverSize);
                        if (nextRiverCord.PointX == riverCord.PointX - 1)
                            currWaterSquare.West = new WaterSource(true, riverCord.RiverSize);
                        if (nextRiverCord.PointX == riverCord.PointX + 1)
                            currWaterSquare.East = new WaterSource(true, riverCord.RiverSize);
                    }
                    if (!addedOne)
                        currWaterSquare.HighestSize = currWaterSquare.LowestSize = riverCord.RiverSize;
                }
            }
            return output;
        }

        private void CreateRiver(int startX, int startY)
        {
            if (_riverMap[startX][startY] != null)
                return;

            if (startX + 1 < MainGame.MapWidth && _riverMap[startX + 1][startY] != null)
                return;

            if (startX - 1 >= 0 && _riverMap[startX - 1][startY] != null)
                return;

            if (startY + 1 < MainGame.MapHeight && _riverMap[startX][startY + 1] != null)
                return;

            if (startY - 1 >= 0 && _riverMap[startX][startY - 1] != null)
                return;

            var rise = _rand.Next(-4, 4);
            var run = _rand.Next(-4, 4);
            var river = new List<RiverCoord>();
            var startx = startX;
            var starty = startY;
            var isDone = false;
            const byte startRiverSize = 1;

            //    riverSize = riverSize < 2 ? (byte) 2 : riverSize;
            var count = 0;
            while (true)
            {
                var riverSize = (byte) (startRiverSize + (count/RiverSizeDividedByPerSquare));
                count++;
                riverSize = riverSize > MaximumRiverSize ? MaximumRiverSize : riverSize;
                riverSize = riverSize < MinimumRiverSize ? MinimumRiverSize : riverSize;
                var runIsNeg = run < 0;
                var runCount = 0;


                while (runCount <= Math.Abs(run))
                {
                    startx += runIsNeg ? -1 : 1;

                    if (startx == 214 && starty == 448)
                        System.Diagnostics.Debug.WriteLine("hit");

                    if (startx < 0 || startx >= MainGame.MapWidth)
                    {
                        isDone = true;
                        break;
                    }


                    river.Add(new RiverCoord(startx, starty, riverSize));
                    if (_riverMap[startx][starty] != null)
                    {
                        isDone = true;
                        SearchRiver(riverSize, startx, starty);
                        break;
                    }

                    _riverMap[startx][starty] = river;
                    var heightPixel = WorldData.MyWorldData.HeightData[startx][starty];
                    if (heightPixel == HeightGenerator.ShallowOcean)
                    {
                        isDone = true;
                        break;
                    }
                    runCount++;
                }

                if (isDone)
                    break;


                var riseIsNeg = run < 0;
                var riseCount = 0;
                while (riseCount <= Math.Abs(rise))
                {
                    starty += riseIsNeg ? -1 : 1;
                    if (starty < 0 || starty >= MainGame.MapHeight)
                    {
                        isDone = true;
                        break;
                    }
                    if (startx == 214 && starty == 448)
                        System.Diagnostics.Debug.WriteLine("hit");
                    river.Add(new RiverCoord(startx, starty, riverSize));
                    if (_riverMap[startx][starty] != null)
                    {
                        isDone = true;
                        break;
                    }
                    _riverMap[startx][starty] = river;
                    var heightPixel = WorldData.MyWorldData.HeightData[startx][starty];
                    if (heightPixel == HeightGenerator.ShallowOcean)
                    {
                        isDone = true;
                        break;
                    }
                    riseCount++;
                }
                if (isDone)
                    break;
            }

            if (river.Count > 0)
                _rivers.Add(river);
            // System.out.println("finished");
        }

        private void SearchRiver(byte riverSize, int currX, int currY)
        {
            //-1 taking out currentRiver
            var mergingRiver = _riverMap[currX][currY];
            var mergeStart = -1;
            for (var y = 0; y < mergingRiver.Count; y++)
            {
                var currentRiverCoord = mergingRiver[y];
                if (currentRiverCoord.PointX == currX && currentRiverCoord.PointY == currY)
                {
                    mergeStart = y;
                    break;
                }
            }


            if (mergingRiver == null)
                System.Diagnostics.Debug.WriteLine("bad news");
            else
            {
                byte count = 0;
                byte savedSized = 0;
                for (var y = mergeStart; y < mergingRiver.Count; y++)
                {
                    if (mergingRiver[y].PointY == 448 && mergingRiver[y].PointX == 214)
                        System.Diagnostics.Debug.WriteLine("hit");

                    var newRiverSize = mergingRiver[y].RiverSize > riverSize ? mergingRiver[y].RiverSize : riverSize;
                    newRiverSize = newRiverSize < savedSized ? savedSized : newRiverSize;
                    var testRiverSize = (byte) (newRiverSize + (count/RiverSizeDividedByPerSquare));
                    var riverValue = MaximumRiverSize < testRiverSize ? MaximumRiverSize : testRiverSize;

                    riverValue = riverValue < MinimumRiverSize ? MinimumRiverSize : riverValue;
                    mergingRiver[y].RiverSize = riverValue;
                    savedSized = savedSized < riverValue ? riverValue : savedSized;
                    count++;
                }
            }
        }

        //private int[] GetValues(int[,] output, int i, bool topRight)
        //{
        //    float floatx;

        //    if (topRight)
        //        floatx = ((125 - output[i, 0])/50.0f);
        //    else
        //        floatx = ((output[i, 0] - 125)/50.0f);

        //    var floaty = ((output[i, 1] - 125)/50.0f);
        //    var realValueX = floatx > 1.0f ? 1 : 0;
        //    realValueX = floatx < -1.0f ? -1 : realValueX;
        //    var realValueY = floaty > 1.0f ? 1 : 0;
        //    realValueY = floaty < -1.0f ? -1 : realValueY;
        //    if (Seeds.Randomizer.NextDouble() > .5)
        //        realValueX = realValueY != 0 ? 0 : realValueX;
        //    else
        //        realValueY = realValueX != 0 ? 0 : realValueY;

        //    if (Math.Abs(realValueX) == 1 && Math.Abs(realValueY) == 1)
        //    {
        //        if (Seeds.Randomizer.NextDouble() > .5)
        //            realValueY = 0;
        //        else
        //            realValueX = 0;
        //    }
        //    // System.out.println("x:"
        //    // + (Math.abs(realValueX) + Math.abs(generalDirectionX)) + "y:"
        //    // + (Math.abs(realValueY) + Math.abs(generalDirectionY)));
        //    var ar = new int[2];
        //    ar[0] = realValueX;
        //    ar[1] = realValueY;
        //    return ar;
        //}

        public Color[] MapColors()
        {
            return new[]
                {
                    Utility.IntToColor(TerrainColors.RiverExist)
                };
        }
    }
}