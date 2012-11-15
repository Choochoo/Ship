#region

using System;
using Microsoft.Xna.Framework;
using Ship.Game.Utils;
using Ship.Game.WorldGeneration.WorldDrawing;
using Ship.Game.Loaders;
using System.Threading.Tasks;
using System.Diagnostics;

#endregion

namespace Ship.Game.WorldGeneration.Noise
{
    public class NoiseGen
    {
        #region Variables

        private const short PointsOnRiver = 400;

        private const byte FillTopLeft = 1;
        private const byte FillTopRight = 2;
        private const byte FillBottomLeft = 3;
        private const byte FillBottomRight = 4;
        private const byte FillTopToBottom = 5;
        private const byte FillBottomToTop = 6;
        private const byte FillLeftToRight = 7;
        private const byte FillRightToLeft = 8;
        private const byte FillTopCenter = 9;
        private const byte FillBottomCenter = 10;
        private const byte FillLeftCenter = 11;
        private const byte FillRightCenter = 12;

        private static readonly BezierCurve Bc = new BezierCurve();


        #endregion

        #region GetRiverSquare

        private static byte[,] _newRiver = new byte[MainGame.SectorTileSize,MainGame.SectorTileSize];
        private static byte[,] _newerRiver = new byte[MainGame.SectorTileSize,MainGame.SectorTileSize];
        public static void GetRiverSquare(ref byte[,] output, WaterSquare area, byte val,Random rand)
        {
            var size = output.GetLength(0);
            
            var mainSource = (area.North != null && area.North.IsFlowDirection) ? area.North : null;
            mainSource = (mainSource == null && area.South != null && area.South.IsFlowDirection)
                             ? area.South
                             : mainSource;
            mainSource = (mainSource == null && area.West != null && area.West.IsFlowDirection) ? area.West : mainSource;
            mainSource = (mainSource == null && area.East != null && area.East.IsFlowDirection) ? area.East : mainSource;


            var tempVal = (byte)5;
            var count = 0;
            if (area.North != null)
                count++;
            if (area.South != null)
                count++;
            if (area.West != null)
                count++;
            if (area.East != null)
                count++;

            if (mainSource == null && count > 0)
            {
                var goNorth = area.North != null;
                var goSouth = area.South != null;
                var goWest = area.West != null;
                var goEast = area.East != null;

                var hasFlow = goNorth && area.North.IsFlowDirection;
                hasFlow = (goSouth && area.South.IsFlowDirection) || hasFlow;
                hasFlow = (goWest && area.West.IsFlowDirection) || hasFlow;
                hasFlow = (goEast && area.East.IsFlowDirection) || hasFlow;

                var outThickness = goNorth ? area.North.Thickness : byte.MaxValue;
                outThickness = goSouth && area.South.Thickness < outThickness ? area.South.Thickness : outThickness;
                outThickness = goWest && area.West.Thickness < outThickness ? area.West.Thickness : outThickness;
                outThickness = goEast && area.East.Thickness < outThickness ? area.East.Thickness : outThickness;
                if (!hasFlow)
                {
                    if (_newRiver == null)
                    {
                        _newRiver = new byte[size,size];
                    }
                    else
                        Array.Clear(_newRiver,0,_newRiver.Length);

                    if (_newerRiver == null)
                        _newerRiver = new byte[size,size];
                    else
                        Array.Clear(_newRiver, 0, _newRiver.Length);

                    const bool createLake = true;
                    if (goNorth)
                    {
                        CreateRiverBezier(ref _newerRiver, area.North.Thickness, tempVal, true, false, false,
                                          false, createLake, rand);
                        tempVal++;
                        MergeData(ref _newRiver, _newerRiver);
                    }
                    if (goSouth)
                    {
                        CreateRiverBezier(ref _newerRiver, area.South.Thickness, tempVal, false, true, false,
                                          false, createLake, rand);
                        tempVal++;
                        MergeData(ref _newRiver,_newerRiver);
                    }
                    if (goWest)
                    {
                        CreateRiverBezier(ref _newerRiver, area.West.Thickness, tempVal, false, false, true,
                                          false, createLake, rand);
                        tempVal++;
                        MergeData(ref _newRiver, _newerRiver);
                    }
                    if (goEast)
                    {
                        CreateRiverBezier(ref _newerRiver, area.East.Thickness, tempVal, false, false, false,
                                          true, createLake, rand);
                        tempVal++;
                        MergeData(ref _newRiver,_newerRiver);
                    }
                }
                else
                {

                    CreateRiverBezier(ref _newRiver, outThickness, tempVal, goNorth, goSouth, goWest, goEast, true, rand);
                    tempVal++;
                }
                
                ChangeVals(ref _newRiver, tempVal);
                tempVal++;
                MergeData(ref output, _newRiver);
            }


            if (mainSource == null)
            {
                GetLake(ref output, area.LowestSize, rand);
                return;
            }

            if (count == 1)
            {
                Array.Clear(_newRiver,0,_newRiver.Length);

                CreateRiverBezier(ref _newRiver, mainSource.Thickness, tempVal, area.North == mainSource,
                                                 area.South == mainSource, area.West == mainSource,
                                                 area.East == mainSource,
                                                 true, rand);
                MergeData(ref output, _newRiver);
            }
            else
            {
                Array.Clear(_newRiver,0,_newRiver.Length);

                for (var i = 0; i < 4; i++)
                {
                    WaterSource currentTester;
                    switch (i)
                    {
                        case 0:
                            currentTester = area.North;
                            break;
                        case 1:
                            currentTester = area.South;
                            break;
                        case 2:
                            currentTester = area.West;
                            break;
                        default:
                            currentTester = area.East;
                            break;
                    }

                    if (currentTester == null)
                        continue;

                    if (currentTester == mainSource)
                        continue;

                    var goNorth = currentTester == area.North || mainSource == area.North;
                    var goSouth = currentTester == area.South || mainSource == area.South;
                    var goWest = currentTester == area.West || mainSource == area.West;
                    var goEast = currentTester == area.East || mainSource == area.East;

                    var outThickness = goNorth ? area.North.Thickness : Byte.MaxValue;
                    outThickness = goSouth && area.South.Thickness < outThickness
                                       ? area.South.Thickness
                                       : outThickness;
                    outThickness = goWest && area.West.Thickness < outThickness
                                       ? area.West.Thickness
                                       : outThickness;
                    outThickness = goEast && area.East.Thickness < outThickness
                                       ? area.East.Thickness
                                       : outThickness;


                    CreateRiverBezier(ref _newRiver, outThickness, tempVal, goNorth, goSouth, goWest, goEast, false, rand);
                    tempVal++;
                    MergeData(ref output, _newRiver);
                }
            }
            //for (var y = 0; y < size; y++)
            //{
            //    for (var x = 0; x < size; x++)
            //    {
            //            System.Diagnostics.Debug.Write(output[x,y] + ",");
            //    }
            //    System.Diagnostics.Debug.WriteLine("");
            //}
                ChangeVals(ref output, val);

        }

        #endregion

        #region ChangeVals

        private static void ChangeVals(ref byte[,] output, byte val)
        {
            const byte size = MainGame.SectorTileSize;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    if (output[x, y] == 0) continue;

                    output[x, y] = val;
                }
            }
        }

        #endregion

        #region CreateRiverBezier
        private static readonly double[] Points = new double[PointsOnRiver];
        private static void CreateRiverBezier(ref byte[,] output, byte thickness, byte val, bool topEqual, bool bottomEqual,
                                                 bool leftEqual, bool rightEqual, bool createLake, Random rand)
        {
            
            var startx = int.MaxValue;
            var starty = int.MaxValue;
            var endx = int.MaxValue;
            var endy = int.MaxValue;
            Array.Clear(Points,0,Points.Length);
            //System.Diagnostics.Debug.WriteLine("creating river");
            const int half = MainGame.SectorTileSize / 2;

            if (topEqual)
            {
                startx = half;
                starty = 0;
            }
            if (bottomEqual)
            {
                if (startx == int.MaxValue)
                {
                    startx = half;
                    starty = MainGame.SectorTileSize;
                }
                else
                {
                    endx = half;
                    endy = MainGame.SectorTileSize;
                }
            }
            if (leftEqual)
            {
                if (startx == int.MaxValue)
                {
                    startx = 0;
                    starty = half;
                }
                else
                {
                    endx = 0;
                    endy = half;
                }
            }
            if (rightEqual)
            {
                endx = MainGame.SectorTileSize;
                endy = half;
            }


            if (startx == int.MaxValue && starty == int.MaxValue && endx == int.MaxValue && endy == int.MaxValue)
                return;

            if (startx == int.MaxValue && starty == int.MaxValue)
            {
                startx = half;
                starty = half;
                if (createLake)
                    GetLake(ref output, thickness, rand);
            }
            else if (endx == int.MaxValue && endy == int.MaxValue)
            {
                endx = half;
                endy = half;
                if (createLake)
                    GetLake(ref output, thickness, rand);
            }

            var ptList = new double[6];
            ptList[0] = (startx);
            ptList[1] = (starty);

            double xRand;
            double yRand;
            if ((topEqual && bottomEqual) || (leftEqual && rightEqual))
            {
                const double min = .2;
                const double max = .5;
                xRand = rand.NextDouble() * (max - min) + min;
                yRand = rand.NextDouble() * (max - min) + min;
            }
            else
            {
                xRand = rand.NextDouble() * rand.NextDouble();
                yRand = rand.NextDouble() * rand.NextDouble();
            }

            double xOut;
            double yOut;

            if ((topEqual && rightEqual))
            {
                xOut = half + (half*xRand);
                yOut = half - (half*yRand); // Control
            }
            else if ((topEqual && leftEqual))
            {
                xOut = half - (half*xRand);
                yOut = half - (half*yRand); // Control
            }
            else if ((bottomEqual && rightEqual))
            {
                xOut = half + (half*xRand);
                yOut = half + (half*yRand); // Control
            }
            else if ((bottomEqual && leftEqual))
            {
                xOut = half - (half*xRand);
                yOut = half + (half*yRand); // Control
            }
            else if ((rightEqual && leftEqual))
            {
                xOut = half;
                if (rand.NextDouble() > .5)
                    yOut = half + (half*xRand); // Control
                else
                    yOut = half - (half*yRand); // Control
            }
            else //top down
            {
                yOut = half;
                if (rand.NextDouble() > .5)
                    xOut = half + (half*xRand); // Control
                else
                    xOut = half - (half*yRand); // Control
            }
            ptList[2] = (xOut);
            ptList[3] = (yOut);


            ptList[4] = (endx);
            ptList[5] = (endy);
            var moddedThick = thickness > 1 ? thickness - 1 : thickness;
            var leftthick = thickness > 1 ? moddedThick/2 : 0;
            var rightthick = thickness > 1 ? moddedThick - leftthick : 0;
            
            //System.Diagnostics.Debug.WriteLine("size:" + thickness);
            Bc.Bezier2D(ptList, PointsOnRiver/2, Points);
            var lastPointX = (int)Points[2];
            var lastPointY = (int)Points[1];
            for (var i = 1; i < PointsOnRiver - 1; i += 2)
            {
                var outx = (int)Points[i + 1];
                var outy = (int)Points[i];

                outx = outx > MainGame.SectorTileSize - 1 ? MainGame.SectorTileSize - 1 : outx;
                outy = outy > MainGame.SectorTileSize - 1 ? MainGame.SectorTileSize - 1 : outy;

                var diffx = outx - lastPointX;
                diffx = diffx < 0 ? -diffx : diffx;
                var diffy = outy - lastPointY;
                diffy = diffy < 0 ? -diffy : diffy;

                if (diffx + diffy > 1)
                {//lastpointy = 33, outy = 32

                    output[outx, lastPointY] = val;

                    output[lastPointX, outy] = val;
                }

                output[outx, outy] = val;
                lastPointX = outx;
                lastPointY = outy;
            }
            output[startx == MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : startx, starty == MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : starty] = val;
            output[endx == MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : endx, endy == MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : endy] = val;

            for (var y = 0; y < MainGame.SectorTileSize; y++)
            {
                for (var x = 0; x < MainGame.SectorTileSize; x++)
                {
                    if (output[x, y] != val) continue;
                    var outx2 = x - leftthick < 0 ? 0 : x - leftthick;
                    var outy2 = y - leftthick < 0 ? 0 : y - leftthick;
                    var outx3 = x + rightthick >= MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : x + rightthick;
                    var outy3 = y + rightthick >= MainGame.SectorTileSize ? MainGame.SectorTileSize - 1 : y + rightthick;

                    for (var x2 = outx2; x2 <= outx3; x2++)
                    {
                        if (output[x2, y] == val) continue;
                        output[x2, y] = 1;
                    }

                    for (var y2 = outy2; y2 <= outy3; y2++)
                    {
                        if (output[x, y2] == val) continue;
                        output[x, y2] = 1;
                    }
                }
            }
        }



        #endregion

        #region CreateOceanBezier

        private static void CreateOceanBezier(ref byte[,] output, int[] spots, byte landval, byte fillDirection, Random rand,
                                              bool isLine = false, bool isSmallCorner = false)
        {

            //System.Diagnostics.Debug.WriteLine("creating ocean");
            var ptList = new double[spots.Length];
            ptList[0] = spots[0];
            ptList[1] = spots[1];


            for (var i = 2; i < spots.Length - 2; i += 2)
            {
                var first = isLine ? -15 : -50;
                var second = isLine ? 15 : 50;

                first = isSmallCorner ? -10 : first;
                second = isSmallCorner ? 10 : second;

                var xRand = rand.Next(first, second) / 100.0;
                var yRand = rand.Next(first, second) / 100.0;

                const double lowestNum = .08;
                xRand = xRand < lowestNum && xRand > 0 ? lowestNum : xRand;
                xRand = xRand > -lowestNum && xRand < 0 ? -lowestNum : xRand;

                yRand = yRand < lowestNum && yRand > 0 ? lowestNum : yRand;
                yRand = yRand > -lowestNum && yRand < 0 ? -lowestNum : yRand;

                ptList[i] = spots[i] + (xRand*spots[i]);
                ptList[i + 1] = spots[i + 1] + (yRand*spots[i + 1]);
            }

            ptList[spots.Length - 2] = (spots[spots.Length - 2]);
            ptList[spots.Length - 1] = (spots[spots.Length - 1]);


            var p = new double[PointsOnRiver];

            Bc.Bezier2D(ptList, (PointsOnRiver)/2, p);
            var maxX = output.Length;
            var maxY = output.Length;

            for (var i = 1; i < PointsOnRiver - 1; i += 2)
            {
                var outx = (int) p[i + 1];
                var outy = (int) p[i];
                outx = outx >= maxX ? maxX - 1 : outx;
                outy = outy >= maxY ? maxY - 1 : outy;
                outx = outx < 0 ? 0 : outx;
                outy = outy < 0 ? 0 : outy;
                output[outx, outy] = landval;
            }
            var len = output.GetLength(0);

            switch (fillDirection)
            {
                case FillLeftCenter:
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x,y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }


                    break;
                case FillRightCenter:
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x,y] = 1;
                        }
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }
                    for (var y = 0; y < len; y++)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }
                    break;
                case FillBottomCenter:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[0, y] == landval)
                            break;
                    }
                    for (var y = 0; y < len; y++)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[len - 1, y] == landval)
                            break;
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    #endregion

                    break;
                case FillTopCenter:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[0, y] == landval)
                            break;
                    }
                    for (var y = 0; y < len; y++)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[len - 1, y] == landval)
                            break;
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    #endregion

                    break;
                case FillTopLeft:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[0, y] == landval)
                            break;
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[x, 0] == landval)
                            break;
                    }

                    #endregion

                    break;
                case FillTopRight:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x,y] == landval) break;
                            output[x,y] = 1;
                        }
                        if (output[len - 1, y] == landval)
                            break;
                    }
                    for (var x = len - 1; x >= 0; x--)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x,y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[x, 0] == landval)
                            break;
                    }

                    #endregion

                    break;
                case FillBottomRight:

                    #region MyRegion

                    for (var y = len - 1; y >= 0; y--)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[len - 1, y] == landval)
                            break;
                    }
                    for (var x = len - 1; x >= 0; x--)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[x, len - 1] == landval)
                            break;
                    }

                    #endregion

                    break;
                case FillBottomLeft:

                    #region MyRegion

                    for (var y = len - 1; y >= 0; y--)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x,y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[0, y] == landval)
                            break;
                    }
                    for (var x = 0; x < len; x++)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                        if (output[x, len - 1] == landval)
                            break;
                    }

                    #endregion

                    break;
                case FillLeftToRight:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = 0; x < len; x++)
                        {
                            if (output[x, y] == landval) break;
                            output[x,y] = 1;
                        }
                    }

                    #endregion

                    break;
                case FillRightToLeft:

                    #region MyRegion

                    for (var y = 0; y < len; y++)
                    {
                        for (var x = len - 1; x >= 0; x--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    #endregion

                    break;
                case FillBottomToTop:

                    #region MyRegion

                    for (var x = 0; x < len; x++)
                    {
                        for (var y = len - 1; y >= 0; y--)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    #endregion

                    break;
                case FillTopToBottom:

                    #region MyRegion

                    for (var x = 0; x < len; x++)
                    {
                        for (var y = 0; y < len; y++)
                        {
                            if (output[x, y] == landval) break;
                            output[x, y] = 1;
                        }
                    }

                    #endregion

                    break;
            }

            for (var y = 0; y < len; y++)
            {
                for (var x = len - 1; x >= 0; x--)
                {
                    if (output[x, y] == 1)
                        output[x, y] = landval;
                }
            }
        }

        #endregion

        #region GetLake

        private static void GetLake(ref byte[,] output, byte thickness, Random rand)
        {
            var newThickness = thickness;
            //System.Diagnostics.Debug.WriteLine("creating lake");

            //newThickness = newThickness < 2 ? (byte) 2 : newThickness;
           // newThickness = newThickness > 20 ? (byte) 20 : newThickness;

            const int half = MainGame.SectorTileSize / 2;
            var spots = newThickness*2;
            spots = spots < 2 ? 2 : spots;
            spots = spots > 30 ? 30 : spots;

            var modT = newThickness; // (int)(newThickness * .7);
            for (var k = 1; k <= spots; k++)
            {
                var halfx = half;
                var halfy = half;
                if (spots > 2)
                {
                    halfx = 32 + rand.Next(-modT, modT);
                    halfy = 32 + rand.Next(-modT, modT);
                }
                const int timesToRun = 200;
                var iterations = spots > 6 ? modT/2 : modT;
                for (var j = 0; j < iterations; j++)
                {
                    for (var i = 1; i < timesToRun; i++)
                    {
                        const int modX = timesToRun/2;
                        const int modY = timesToRun/2;
                        var modThickness = j + rand.Next(-3, 3); //2.0 + (newThickness*percX);
                        var modThicknessy = j + rand.Next(-3, 3); //2.0 + (newThickness * percY);
                        var cos = Math.PI/modX;
                        var sin = Math.PI/modY;
                        cos = Math.Cos(cos*i);
                        sin = Math.Sin(sin*i);
                        var xPos = (int) Math.Round(halfx - modThickness*cos);
                        var yPos = (int) Math.Round(halfy + modThicknessy*sin);

                        xPos = xPos < 0 ? 0 : xPos;
                        yPos = yPos < 0 ? 0 : yPos;

                        xPos = xPos > MainGame.SectorTileSize - 1 ? MainGame.SectorTileSize - 1 : xPos;
                        yPos = yPos > MainGame.SectorTileSize - 1 ? MainGame.SectorTileSize - 1 : yPos;
                        output[xPos, yPos] = 1;
                    }
                }
            }
        }

        #endregion

        #region GetTileSimplex

        public static void GetTileSimplex(ref byte[,] output, Vector2 p)
        {
            //System.Diagnostics.Debug.WriteLine("creating tilesimplex");
            const byte size = MainGame.SectorTileSize;
            var locationX = (p.X * size) + 1;
            var locationY = (p.Y * size) + 1;


            const int halfSize = byte.MaxValue/2;

            
            const float persistence = .2f;
            const byte octaves = 1;
            //var sw3 = Stopwatch.StartNew();
           

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var t = 0.0;
                    var amplitude = 1.0f;
                    var freq = .02f;
                    for (var k = 0; k < octaves; k++)
                    {

                        
                        var xOut = (x + locationX) * freq + Seeds.ForestSeed;
                        var yOut = (y + locationY) * freq + Seeds.ForestSeed;
                        var addon = SimplexNoise.Noise(xOut, yOut) * amplitude;
                        t += addon;
                        amplitude *= persistence;
                        freq *= 2;
                    }
                    t = t > 1.0 ? 1.0 : t;
                    t = t < -1.0 ? -1.0 : t;
                    var val = (byte)((t * halfSize) + halfSize);
                    output[x, y] = val;
                }
            }

            //sw3.Stop();
        }

        #endregion

        #region GetDecorSimplex

        public static void GetDecorSimplex(ref byte[,] output, Vector2 p)
        {
            //System.Diagnostics.Debug.WriteLine("creating decor simplex");
            const byte size = MainGame.SectorTileSize;
            var locationX = (p.X*size)+1;
            var locationY = (p.Y*size)+1;


            const int halfSize = byte.MaxValue/2;
            const float persistence = .8f;
            const byte octaves = 1;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {

                    var t = 0.0;
                    var amplitude = 1.0f;
                    var freq = .02f;

                    for (var k = 0; k < octaves; k++)
                    {
                        var xOut = (x + locationX) * freq + Seeds.ForestSeed;
                        var yOut = (y + locationY)*freq + Seeds.ForestSeed;
                        var addon = SimplexNoise.Noise(xOut,yOut)*amplitude ;
                        t += addon;
                        amplitude *= persistence;
                        freq *= 2.0f;
                    }
                    t = t > 1.0 ? 1.0 : t;
                    t = t < -1.0 ? -1.0 : t;
                     var val = (byte) ((t*halfSize)+halfSize);
                    output[x, y] = val;
                }
            }
        }

        #endregion

        #region GetWorldSimplex

        public static int[,] GetWorldSimplex(int seed, int sizeX, int sizeY,
                                             Vector2 p, int octaves, float verticalMod, float sizeMod)
        {
            var val = new int[sizeX,sizeY];
            var lowest = 0;
            var highest = 0;
            for (var i = 0; i < val.GetLength(0); i++)
            {
                for (var j = 0; j < val.GetLength(1); j++)
                {
                    var sigma = 0.0f;
                    var divisor = .007f*sizeMod;
                    var yMod = .004f*verticalMod;
                    for (var n = 0; n < octaves; n++)
                    {
                        sigma += (float) SimplexNoise.Noise((i + p.X)
                                                            *(divisor + yMod), (j + p.Y)*divisor)
                                 /divisor;
                        divisor *= 2;
                    }
                    var c = (int) (sigma*127 + 128);
                    if (c > highest)
                        highest = c;
                    else if (c < lowest)
                        lowest = c;
                    val[i, j] = c;
                }
            }
            var minus = -lowest;
            var total = highest + minus;
            for (var i = 0; i < val.GetLength(0); i++)
            {
                for (var j = 0; j < val.GetLength(1); j++)
                {
                    var c = val[i, j];
                    c += minus;
                    c = (int) ((c/(float) total)*255);
                    // c = Math.round();
                    val[i, j] = c;
                }
            }
            return val;
        }

        #endregion

        #region OceanWithLand
        public static void OceanWithLand(ref byte[,] output, byte[][] waterData, int x, int y, byte val)
        {
            var size = output.Length;
            var rand = new Random((x + y)*val);
            var topLeftFine = ValueDecider(waterData, x, y, -1, -1, HeightGenerator.ShallowOcean);
            var topMidFine = ValueDecider(waterData, x, y, 0, -1, HeightGenerator.ShallowOcean);
            var topRightFine = ValueDecider(waterData, x, y, 1, -1, HeightGenerator.ShallowOcean);
            //
            var midLeftFine = ValueDecider(waterData, x, y, -1, 0, HeightGenerator.ShallowOcean);
            var midRightFine = ValueDecider(waterData, x, y, 1, 0, HeightGenerator.ShallowOcean);
            //
            var bottomLeftFine = ValueDecider(waterData, x, y, -1, 1, HeightGenerator.ShallowOcean);
            var bottomMidFine = ValueDecider(waterData, x, y, 0, 1, HeightGenerator.ShallowOcean);
            var bottomRightFine = ValueDecider(waterData, x, y, 1, 1, HeightGenerator.ShallowOcean);

            var oneThird = (int) Math.Round(size*.33);
            var twoThird = (int) Math.Round(size*.66);
            var half = size/2;

            if (topLeftFine && topMidFine && topRightFine && midLeftFine && midRightFine && bottomLeftFine &&
                bottomMidFine && bottomRightFine)
            {
                for (var i = 0; i < size; i++)
                {
                    for (var j = 0; j < size; j++)
                        output[i, j] = val;
                }
                return;
            }
            var doNotInvert = false;
            if (!midLeftFine && midRightFine && topMidFine && bottomMidFine)
            {
                CreateOceanBezier(ref output, new[] { oneThird, 0, oneThird, half, oneThird, size }, val, FillLeftToRight, rand);
                //topLeftNum = 12;
                //topRightNum = 13;
                //bottomLeftNum = 16;
                //bottomRightNum = 14;

                if (!topRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, 0, twoThird, oneThird, size, oneThird}, val,
                                      FillTopRight, rand, false, true);
                }
                if (!bottomRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, size, twoThird, twoThird, size, twoThird}, val,
                                      FillBottomRight, rand, false, true);
                }
            }
            else if (midLeftFine && !midRightFine && topMidFine && bottomMidFine)
            {
                //topLeftNum = 13;
                //topRightNum = 15;
                //bottomLeftNum = 17;
                //bottomRightNum = 19;
                CreateOceanBezier(ref output, new[] { twoThird, 0, twoThird, half, twoThird, size }, val, FillRightToLeft, rand);

                if (!bottomLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {0, twoThird, oneThird, twoThird, oneThird, size}, val,
                                      FillBottomLeft, rand, false, true);
                }
                if (!topLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {oneThird, 0, oneThird, oneThird, 0, oneThird}, val, FillTopLeft,rand,
                                      false, true);
                }
            }
            else if (midLeftFine && midRightFine && !topMidFine && bottomMidFine)
            {
                //topLeftNum = 9;
                //topRightNum = 10;
                //bottomLeftNum = 13;
                //bottomRightNum = 14;
                CreateOceanBezier(ref output, new[] {0, oneThird, half, oneThird, size, oneThird}, val, FillTopToBottom,rand,
                                  true);
                if (!bottomLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {0, twoThird, oneThird, twoThird, oneThird, size}, val,
                                      FillBottomLeft,rand, false, true);
                }
                if (!bottomRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, size, twoThird, twoThird, size, twoThird}, val,
                                      FillBottomRight,rand, false, true);
                }
            }
            else if (midLeftFine && midRightFine && topMidFine && !bottomMidFine)
            {
                CreateOceanBezier(ref output, new[] { 0, twoThird, half, twoThird, size, twoThird }, val, FillBottomToTop, rand);
                //topLeftNum = 13;
                //topRightNum = 14;
                //bottomLeftNum = 21;
                //bottomRightNum = 22;
                if (!topRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, 0, twoThird, oneThird, size, oneThird}, val,
                                      FillTopRight, rand, false, true);
                }
                if (!topLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {oneThird, 0, oneThird, oneThird, 0, oneThird}, val, FillTopLeft,rand,
                                      false, true);
                }
            }
            else if (!midLeftFine && midRightFine && !topMidFine && bottomMidFine)
            {
                //topLeftNum = 8;
                //topRightNum = 9;
                //bottomLeftNum = 12;
                //bottomRightNum = 18;
                CreateOceanBezier(ref output, new[] {oneThird, size, oneThird, oneThird, size, oneThird}, val,
                                  FillTopLeft, rand);
                if (!bottomRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, size, twoThird, twoThird, size, twoThird}, val,
                                      FillBottomRight, rand, false, true);
                }
            }
            else if (!midLeftFine && midRightFine && topMidFine)
            {
                CreateOceanBezier(ref output, new[] {oneThird, 0, oneThird, twoThird, size, twoThird}, val,
                                  FillBottomLeft, rand);
                //topLeftNum = 16;
                //topRightNum = 13;
                //bottomLeftNum = 20;
                //bottomRightNum = 21;
                if (!topRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, 0, twoThird, oneThird, size, oneThird}, val,
                                      FillTopRight, rand, false, true);
                }
            }
            else if (midLeftFine && !midRightFine && !topMidFine && bottomMidFine)
            {
                CreateOceanBezier(ref output, new[] {0, oneThird, twoThird, oneThird, twoThird, size}, val,
                                  FillTopRight, rand);
                //topLeftNum = 9;
                //topRightNum = 11;
                //bottomLeftNum = 17;
                //bottomRightNum = 15;
                if (!bottomLeftFine)
                    CreateOceanBezier(ref output, new[] {0, twoThird, oneThird, twoThird, oneThird, size}, val,
                                      FillBottomLeft, rand, false, true);
            }
            else if (midLeftFine && !midRightFine && topMidFine)
            {
                CreateOceanBezier(ref output, new[] { twoThird, 0, twoThird, twoThird, 0, twoThird }, val, FillBottomRight, rand);
                //topLeftNum = 13;
                //topRightNum = 15;
                //bottomLeftNum = 21;
                //bottomRightNum = 23;
                if (!topLeftFine)
                {
                    CreateOceanBezier(ref output, new[] { oneThird, 0, oneThird, oneThird, 0, oneThird }, val, FillTopLeft, rand,
                                      false, true);
                }
            }
            else if (!midLeftFine && !midRightFine && !topMidFine && bottomMidFine)
            {
                //topLeftNum = 8;
                //topRightNum = 11;
                //bottomLeftNum = 12;
                //bottomRightNum = 15;
                CreateOceanBezier(ref output,
                                  new[] {oneThird, size, -oneThird, 0, size + oneThird, 0, twoThird, size}, val,
                                  FillBottomCenter, rand, false, true);
            }
            else if (midLeftFine && !midRightFine)
            {
                //topLeftNum = 9;
                //topRightNum = 11;
                //bottomLeftNum = 21;
                //bottomRightNum = 5;
                CreateOceanBezier(ref output,
                                  new[] {0, oneThird, size, -oneThird, size, size + oneThird, 0, twoThird}, val,
                                  FillLeftCenter,rand, false, true);
            }
            else if (!midLeftFine && midRightFine)
            {
                //topLeftNum = 8;
                //topRightNum = 9;
                //bottomLeftNum = 4;
                //bottomRightNum = 21;
                CreateOceanBezier(ref output,
                                  new[]
                                      {size, oneThird, -oneThird, -oneThird, -oneThird, size + oneThird, size, twoThird},
                                  val, FillRightCenter,rand, false, true);
            }
            else if (!midLeftFine && topMidFine && !bottomMidFine)
            {
                //topLeftNum = 12;
                //topRightNum = 15;
                //bottomLeftNum = 4;
                //bottomRightNum = 5;
                CreateOceanBezier(ref output, new[] {oneThird, 0, -oneThird, size, size + oneThird, size, twoThird, 0},
                                  val, FillTopCenter,rand, false, true);
            }
            else if (midLeftFine && !topMidFine)
            {
                //topLeftNum = 9;
                //topRightNum = 10;
                //bottomLeftNum = 21;
                //bottomRightNum = 22;
                CreateOceanBezier(ref output, new[] {0, oneThird, half, oneThird, size, oneThird}, val, FillTopToBottom,rand,
                                  true);
                CreateOceanBezier(ref output, new[] {0, twoThird, half, twoThird, size, twoThird}, val, FillBottomToTop,rand,
                                  true);
            }
            else if (!midLeftFine && topMidFine)
            {
                CreateOceanBezier(ref output, new[] {oneThird, 0, oneThird, half, oneThird, size}, val, FillLeftToRight,rand,
                                  true);
                CreateOceanBezier(ref output, new[] {twoThird, 0, twoThird, half, twoThird, size}, val, FillRightToLeft,rand,
                                  true);

                //topLeftNum = 12;
                //topRightNum = 15;
                //bottomLeftNum = 16;
                //bottomRightNum = 19;
            }
            else if (midLeftFine)
            {
                if (!topLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {oneThird, 0, oneThird, oneThird, 0, oneThird}, val, FillTopLeft,rand,
                                      false, true);
                }
                if (!topRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, 0, twoThird, oneThird, size, oneThird}, val,
                                      FillTopRight, rand, false, true);
                }
                if (!bottomLeftFine)
                {
                    CreateOceanBezier(ref output, new[] {0, twoThird, oneThird, twoThird, oneThird, size}, val,
                                      FillBottomLeft,rand, false, true);
                }
                if (!bottomRightFine)
                {
                    CreateOceanBezier(ref output, new[] {twoThird, size, twoThird, twoThird, size, twoThird}, val,
                                      FillBottomRight, rand, false, true);
                }
                //topLeftNum = topLefts;
                //topRightNum = topRights;
                //bottomLeftNum = bottomLefts;
                //bottomRightNum = bottomRights;
            }
            else
            {
                //topLeftNum = 8;
                //topRightNum = 11;
                //bottomLeftNum = 20;
                //bottomRightNum = 23;
                doNotInvert = true;
                GetLake(ref output, 32,rand);
            }
            if (!doNotInvert)
                InvertOuput(ref output, val);

        }

        #endregion

        #region AddSand
        public static void AddSand(ref byte[,] output, byte waterVal, byte grassVal, byte sandVal,int sizeMin, int sizeMax,Random rand)
        {
            var size = output.GetLength(0);
            for (var xx = 0; xx < size; xx++)
            {
                for (var yy = 0; yy < size; yy++)
                {
                    if (output[xx, yy] != waterVal) continue;
                    var numOfSandSquares = rand.Next(sizeMin, sizeMax);
                    for (var y = -numOfSandSquares; y <= numOfSandSquares; y++)
                    {
                        if (xx >= size || xx < 0 || yy + y >= size || yy + y < 0) continue;
                        if (output[xx , yy + y] != grassVal) continue;
                        output[xx, yy + y] = sandVal;
                    }
                    numOfSandSquares = rand.Next(sizeMin, sizeMax);
                    for (var x = -numOfSandSquares; x <= numOfSandSquares; x++)
                    {
                        if (xx + x >= size || xx + x < 0 || yy >= size || yy < 0) continue;
                        if (output[xx + x, yy] != grassVal) continue;
                        output[xx + x, yy] = sandVal;
                    }
                }
            }
        }
        #endregion

        #region InvertOutput
        private static void InvertOuput(ref byte[,] output, byte val)
        {
            var size = output.GetLength(0);
            for (var xx = 0; xx < size; xx++)
            {
                for (var yy = 0; yy < size; yy++)
                {
                    if (output[xx, yy] == val)
                        output[xx, yy] = 0;
                    else output[xx, yy] = val;
                }
            }
        }
        #endregion

        #region MergeData

        public static void MergeData(ref byte[,] mainRiver, byte[,] addonRiver, int sandVal = 0)
        {
            if (addonRiver == null)
                return;

            var len = mainRiver.GetLength(0);
            for (var i = 0; i < len; i++)
            {
                for (var j = 0; j < len; j++)
                {
                    if (mainRiver[i, j] != 0 && mainRiver[i, j] != sandVal) continue;
                    if (addonRiver[i, j] == 0) continue;
                    mainRiver[i, j] = addonRiver[i, j];
                }
            }
        }

        #endregion

        #region ValueDecider

        private static bool ValueDecider(byte[][] data, int originX, int originY, int offsetX, int offsetY, byte value)
        {
            var size = data.Length;
            if (originX + offsetX >= size)
                return true;
            if (originX + offsetX < 0)
                return true;
            if (originY + offsetY > size)
                return true;
            if (originY + offsetY < 0)
                return true;
            return data[originX + offsetX][originY + offsetY] <= value;
        }

        #endregion
    }
}