package noise;

import game.utils.Globals;
import game.utils.WaterPoint;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.Stroke;
import java.awt.geom.CubicCurve2D;
import java.awt.geom.Point2D;
import java.awt.image.BufferedImage;
import java.util.Random;

public class NoiseGen
{
	private static long mapSeed;

	public static byte[][] getNoise(long _mapSeed, int size, double sizeX,
			double sizeY, Point p, byte val, double percentageOfRarity)
	{
		mapSeed = _mapSeed;
		if (rand == null)
			rand = new Random(Globals.getMapSeed());
		int bubblyValue = (int) (percentageOfRarity * 255);
		SimplexNoise.genGrad(mapSeed);
		// ImprovedNoise.genGrad(mapSeed);
		byte[][] ba = new byte[size][size];
		for (short _x = 0; _x < size; _x++)
		{
			for (short _y = 0; _y < size; _y++)
			{
				double tile = SimplexNoise.noise((_x + p.x) / sizeX, (_y + p.y)
						/ sizeY);
				// double tile = ImprovedNoise.noise((_x + p.x) / sizeX,
				// (_y + p.y) / sizeY, 0);
				int tilenum = (int) ((tile + 1) * 127);
				byte test = tilenum > bubblyValue ? (byte) val : (byte) 0;
				ba[_x][_y] = test;
			}
		}
		return ba;
	}

	public static byte[][] addNoise(byte[][] ba, double sizeX, double sizeY,
			Point p, byte val, double percentageOfRarity)
	{
		int size = ba.length;
		int percOfRarity = (int) (percentageOfRarity * .255);
		for (short _x = 0; _x < size; _x++)
		{
			for (short _y = 0; _y < size; _y++)
			{
				double tile = SimplexNoise.noise((_x + p.x) / sizeX, (_y + p.y)
						/ sizeY);
				if (ba[_x][_y] == 0)
				{
					byte test = (byte) (((short) (tile + 1) * 127) > percOfRarity ? val
							: 0);
					ba[_x][_y] = test;
				}
			}
		}
		return ba;
	}

	public static void applyRiver(byte[][] ba, Point start, Point exit, byte val)
	{
		if (rand == null)
			rand = new Random(Globals.getMapSeed());
		CubicCurve2D.Double cubicCurve;
		BufferedImage img = new BufferedImage(ba.length, ba.length,
				BufferedImage.TYPE_INT_RGB);
		Point2D.Double P1 = new Point2D.Double(start.x, start.y); // Start
		// Point
		Point2D.Double P2 = new Point2D.Double(exit.x, exit.y); // End
		// Point
		Point halfway = getHalfway(start, exit);
		Point firstVert = getHalfway(start, halfway);
		Point secondVert = getHalfway(halfway, exit);
		// Point
		// 1
		Point2D.Double ctrl1;
		Point2D.Double ctrl2;
		float d = rand.nextFloat();
		if (rand.nextBoolean())
		{
			ctrl1 = new Point2D.Double(firstVert.x + (firstVert.x * d),
					firstVert.y + (firstVert.y * d)); // Control
		} else
		{
			ctrl1 = new Point2D.Double(firstVert.x - (firstVert.x * d),
					firstVert.y - (firstVert.y * d)); // Control
		}
		float b = rand.nextFloat();
		if (rand.nextBoolean())
		{
			ctrl2 = new Point2D.Double(secondVert.x + (secondVert.x * b),
					secondVert.y + (secondVert.y * b)); // Control
		} else
		{
			ctrl2 = new Point2D.Double(secondVert.x - (secondVert.x * b),
					secondVert.y - (secondVert.y * b)); // Control
		}
		// Point2D.Double ctrl2 = new Point2D.Double(secondVert.x *
		// Math.random(),
		// secondVert.y * -d); // Control
		// Point2D.Double ctrl1 = new Point2D.Double(firstVert.x, firstVert.y);
		// // Control
		cubicCurve = new CubicCurve2D.Double(P1.x, P1.y, ctrl1.x, ctrl1.y,
				ctrl2.x, ctrl2.y, P2.x, P2.y);
		Stroke stroke = new BasicStroke(8.0f);
		Graphics2D gd2 = img.createGraphics();
		gd2.setStroke(stroke);
		gd2.setColor(Color.BLUE);
		gd2.draw(cubicCurve);
		short _x, _y;
		for (_x = 0; _x < ba.length; _x++)
		{
			for (_y = 0; _y < ba.length; _y++)
			{
				if ((int) img.getRGB(_x, _y) != -16777216)
				{
					ba[_x][_y] = val;
				}
			}
		}
	}

	private static Random rand;
	public static final byte FILL_TOP_LEFT = 1;
	public static final byte FILL_TOP_RIGHT = 2;
	public static final byte FILL_BOTTOM_LEFT = 3;
	public static final byte FILL_BOTTOM_RIGHT = 4;
	public static final byte FILL_TOP_TO_DOWN = 5;
	public static final byte FILL_LEFT_TO_RIGHT = 6;
	public static final byte FILL_DOWN_TO_TOP = 7;
	public static final byte FILL_RIGHT_TO_LEFT = 8;
	public static final byte FILL_EVERYTHING_BUT_TOPLEFT = 9;
	public static final byte FILL_EVERYTHING_BUT_TOPRIGHT = 10;
	public static final byte FILL_EVERYTHING_BUT_BOTTOMLEFT = 11;
	public static final byte FILL_EVERYTHING_BUT_BOTTOMRIGHT = 12;

	public static byte[][] makeTerrainBlend(int size, byte defaultValue,
			byte fillValue, int typeOfCurve)
	{
		if (rand == null)
			rand = new Random(Globals.getMapSeed());
		byte fillType = 0;
		int midValueHigh = (int) (size * .75f);
		int midValueLow = (int) (size * .25f);
		Point start = new Point();
		Point end = new Point();
		boolean bendRight = false;
		boolean bendTop = false;
		boolean itMatters = false;
		float divider = 0.7f;
		boolean allVal = false;
		byte[][] ba = new byte[size][size];
		switch (typeOfCurve)
		{
		case 2:
			start.setLocation(0, midValueLow);
			end.setLocation(midValueLow, 0);
			fillType = NoiseGen.FILL_EVERYTHING_BUT_TOPLEFT;
			bendRight = true;
			bendTop = false;
			itMatters = true;
			divider = .2f;
			break;
		case 3:
			start.setLocation(midValueHigh, 0);
			end.setLocation(size, midValueLow);
			fillType = NoiseGen.FILL_EVERYTHING_BUT_TOPRIGHT;
			bendRight = false;
			bendTop = true;
			itMatters = true;
			divider = .2f;
			break;
		case 6:
			start.setLocation(0, midValueHigh);
			end.setLocation(midValueLow, size);
			fillType = NoiseGen.FILL_EVERYTHING_BUT_BOTTOMLEFT;
			bendRight = false;
			bendTop = true;
			itMatters = true;
			divider = .2f;
			break;
		case 7:
			start.setLocation(midValueHigh, size);
			end.setLocation(size, midValueHigh);
			fillType = NoiseGen.FILL_EVERYTHING_BUT_BOTTOMRIGHT;
			bendRight = false;
			bendTop = true;
			itMatters = true;
			divider = .2f;
			break;
		case 8:
			start.setLocation(midValueLow, size);
			end.setLocation(size, midValueLow);
			fillType = NoiseGen.FILL_BOTTOM_RIGHT;
			break;
		case 9:
		case 10:
			start.setLocation(0, midValueLow);
			end.setLocation(size, midValueLow);
			fillType = NoiseGen.FILL_DOWN_TO_TOP;
			break;
		case 11:
			end.setLocation(midValueHigh, size);
			start.setLocation(0, midValueLow);
			fillType = NoiseGen.FILL_BOTTOM_LEFT;
			break;
		case 12:
		case 16:
			start.setLocation(midValueLow, 0);
			end.setLocation(midValueLow, size);
			fillType = NoiseGen.FILL_RIGHT_TO_LEFT;
			break;
		case 13:
		case 14:
		case 17:
		case 18:
			allVal = true;
			break;
		case 15:
		case 19:
			start.setLocation(midValueHigh, 0);
			end.setLocation(midValueHigh, size);
			fillType = NoiseGen.FILL_LEFT_TO_RIGHT;
			break;
		case 20:
			start.setLocation(midValueLow, 0);
			end.setLocation(size, midValueHigh);
			fillType = NoiseGen.FILL_TOP_RIGHT;
			break;
		case 21:
		case 22:
			start.setLocation(0, midValueHigh);
			end.setLocation(size, midValueHigh);
			fillType = NoiseGen.FILL_TOP_TO_DOWN;
			break;
		case 23:
			start.setLocation(0, midValueHigh);
			end.setLocation(midValueHigh, 0);
			fillType = NoiseGen.FILL_TOP_LEFT;
			break;
		}
		int _x, _y;
		if (allVal)
		{
			for (_x = 0; _x < size; _x++)
			{
				for (_y = 0; _y < size; _y++)
				{
					ba[_y][_x] = fillValue;
				}
			}
		} else
		{
			CubicCurve2D.Double cubicCurve;
			BufferedImage img = new BufferedImage(size, size,
					BufferedImage.TYPE_INT_RGB);
			Point2D.Double P1 = new Point2D.Double(start.x, start.y); // Start
			// Point
			Point2D.Double P2 = new Point2D.Double(end.x, end.y); // End
			// Point
			// 1
			Point2D.Double ctrl1;
			Point2D.Double ctrl2;
			Point halfway = getHalfway(start, end);
			Point firstVert = getHalfway(start, halfway);
			Point secondVert = getHalfway(halfway, end);
			// Point
			// 1
			float num = 2.0f;
			float multiplyAmt = rand.nextFloat() * divider;
			float multiplyAmt2 = rand.nextFloat() * divider;
			boolean enterFirst = rand.nextBoolean();
			boolean enterSecond = rand.nextBoolean();
			//
			if (itMatters && bendRight)
				enterFirst = true;
			else if (itMatters)
				enterFirst = false;
			//
			if (itMatters && bendTop)
				enterSecond = false;
			else if (itMatters)
				enterSecond = true;
			//
			if (enterFirst)
			{
				ctrl1 = new Point2D.Double(firstVert.x
						+ ((firstVert.x * multiplyAmt) / num), firstVert.y
						+ ((firstVert.y * multiplyAmt) / num)); // Control
			} else
			{
				ctrl1 = new Point2D.Double(firstVert.x
						- ((firstVert.x * multiplyAmt) / num), firstVert.y
						- ((firstVert.y * multiplyAmt) / num)); // Control
			}
			if (enterSecond)
			{
				ctrl2 = new Point2D.Double(secondVert.x
						+ ((secondVert.x * multiplyAmt2) / num), secondVert.y
						+ ((secondVert.y * multiplyAmt2) / num)); // Control
			} else
			{
				ctrl2 = new Point2D.Double(secondVert.x
						- ((secondVert.x * multiplyAmt2) / num), secondVert.y
						- ((secondVert.y * multiplyAmt2) / num)); // Control
			}
			cubicCurve = new CubicCurve2D.Double(P1.x, P1.y, ctrl1.x, ctrl1.y,
					ctrl2.x, ctrl2.y, P2.x, P2.y);
			Stroke stroke = new BasicStroke(2.0f);
			Graphics2D gd2 = img.createGraphics();
			gd2.setStroke(stroke);
			gd2.setColor(Color.BLUE);
			gd2.draw(cubicCurve);
			boolean fillColor = false;
			for (_x = 0; _x < size; _x++)
			{
				for (_y = 0; _y < size; _y++)
				{
					if ((int) img.getRGB(_x, _y) != -16777216)
					{
						ba[_y][_x] = fillValue;
					} else
						ba[_y][_x] = defaultValue;
				}
			}
			switch (fillType)
			{
			case NoiseGen.FILL_EVERYTHING_BUT_BOTTOMRIGHT:
				for (_y = 0; _y < size; _y++)
				{
					for (_x = 0; _x < size; _x++)
					{
						if (ba[_y][_x] == fillValue)
							break;
						else
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_EVERYTHING_BUT_BOTTOMLEFT:
				for (_y = 0; _y < size; _y++)
				{
					for (_x = size - 1; _x >= 0; _x--)
					{
						if (ba[_y][_x] == fillValue)
							break;
						else
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_EVERYTHING_BUT_TOPRIGHT:
				for (_y = size - 1; _y > -1; _y--)
				{
					for (_x = 0; _x < size; _x++)
					{
						if (ba[_y][_x] == fillValue)
							break;
						else
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_EVERYTHING_BUT_TOPLEFT:
				for (_y = size - 1; _y >= 0; _y--)
				{
					for (_x = size - 1; _x >= 0; _x--)
					{
						if (ba[_y][_x] == fillValue)
							break;
						else
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_LEFT_TO_RIGHT:
			case NoiseGen.FILL_TOP_LEFT:
				for (_y = size - 1; _y >= 0; _y--)
				{
					fillColor = false;
					for (_x = size - 1; _x >= 0; _x--)
					{
						if (ba[_y][_x] == fillValue)
							fillColor = true;
						if (fillColor)
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_TOP_RIGHT:
				for (_x = 0; _x < size; _x++)
				{
					fillColor = false;
					for (_y = size - 1; _y >= 0; _y--)
					{
						if (ba[_y][_x] == fillValue)
							fillColor = true;
						if (fillColor)
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_RIGHT_TO_LEFT:
				for (_y = 0; _y < size; _y++)
				{
					fillColor = false;
					for (_x = 0; _x < size; _x++)
					{
						if (ba[_y][_x] == fillValue)
							fillColor = true;
						if (fillColor)
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_DOWN_TO_TOP:
			case NoiseGen.FILL_BOTTOM_RIGHT:
				for (_x = 0; _x < size; _x++)
				{
					fillColor = false;
					for (_y = 0; _y < size; _y++)
					{
						if (ba[_y][_x] == fillValue)
							fillColor = true;
						if (fillColor)
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_BOTTOM_LEFT:
				for (_y = 0; _y < size; _y++)
				{
					fillColor = false;
					for (_x = size - 1; _x >= 0; _x--)
					{
						if (ba[_y][_x] == fillValue)
							fillColor = true;
						if (fillColor)
							ba[_y][_x] = fillValue;
					}
				}
				break;
			case NoiseGen.FILL_TOP_TO_DOWN:
				for (_x = 0; _x < size; _x++)
				{
					for (_y = 0; _y < size; _y++)
					{
						if (ba[_y][_x] == fillValue)
							break;
						else
							ba[_y][_x] = fillValue;
					}
				}
				break;
			}
		}
		return ba;
	}

	public static Point getHalfway(Point first, Point second)
	{
		double halfwayX = (first.x + second.x) / 2;
		double halfwayY = (first.y + second.y) / 2;
		Point p = new Point((int) halfwayX, (int) halfwayY);
		return p;
	}

	private static WaterPoint getHalfway(WaterPoint first, WaterPoint second)
	{
		double halfwayX = (first.getX() + second.getX()) / 2;
		double halfwayY = (first.getY() + second.getY()) / 2;
		WaterPoint p = new WaterPoint((int) halfwayX, (int) halfwayY, (byte) 0,
				0, 0);
		return p;
	}

	public static int[][] getWorldSimplex(long seed, int sizeX, int sizeY,
			Point p, int octaves, float verticalMod, float sizeMod)
	{
		SimplexNoise.genGrad(seed);
		int[][] val = new int[sizeX][sizeY];
		int lowest = 0;
		int highest = 0;
		for (int i = 0; i < val.length; i++)
		{
			for (int j = 0; j < val[0].length; j++)
			{
				float sigma = 0.0f;
				float divisor = .007f * sizeMod;
				float yMod = .004f * verticalMod;
				for (int n = 0; n < octaves; n++)
				{
					sigma += (float) SimplexNoise.noise((i + p.x)
							* (divisor + yMod), (j + p.y) * divisor)
							/ divisor;
					divisor *= 2;
				}
				int c = (int) (sigma * 127 + 128);
				if (c > highest)
					highest = c;
				else if (c < lowest)
					lowest = c;
				val[i][j] = c;
			}
		}
		int minus = -lowest;
		int total = highest + minus;
		for (int i = 0; i < val.length; i++)
		{
			for (int j = 0; j < val[0].length; j++)
			{
				int c = val[i][j];
				c += minus;
				c = (int) (((float) c / (float) total) * 255);
				// c = Math.round();
				val[i][j] = c;
			}
		}
		return val;
	}
}
