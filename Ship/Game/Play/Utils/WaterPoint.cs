

public class WaterPoint
{
	private int x;

	public int getX()
	{
		return x;
	}

	public int getY()
	{
		return y;
	}

	private int y;
	private int value = 0;
	private int mySectorX;

	public int getMySectorX()
	{
		return mySectorX;
	}

	public int getMySectorY()
	{
		return mySectorY;
	}

	private int mySectorY;

	public int getValue()
	{
		return value;
	}

	public const byte UP = 1;
	public const byte DOWN = 2;
	public const byte LEFT = 3;
	public const byte RIGHT = 4;

	public WaterPoint(int xVal, int yVal, byte directionValue, int sectorX,
			int sectorY)
	{
		x = xVal;
		y = yVal;
		mySectorX = sectorX;
		mySectorY = sectorY;
		value = directionValue;
	}

	public WaterPoint Clone(byte direction, int sectorX, int sectorY)
	{
		return new WaterPoint(x, y, direction, sectorX, sectorY);
	}
}
