using Microsoft.Xna.Framework;


public class OceanValue
{
	private Point start;
	private Point end;
	private byte fillType;

	public OceanValue(Point _start, Point _end, byte _fillType)
	{
		start = _start;
		end = _end;
		fillType = _fillType;
	}

	public Point getStart()
	{
		return start;
	}

	public Point getEnd()
	{
		return end;
	}

	public byte getFillType()
	{
		return fillType;
	}
}
