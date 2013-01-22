using System;

public class MortalFactory
{
	private const int _tileBuffer = 72;
	private Boolean _decorsLoaded = false;
	private short innerlooper;
	public Decoration[][] decorSprites = new Decoration[LoaderBase.DATA_SQUARE][LoaderBase.DATA_SQUARE];
	private Decoration mostDownSprite;
	private Decoration mostLeftSprite;
	private Decoration mostRightSprite;
	private Decoration mostUpSprite;
	private int spawnX;
	private int spawnY;
	private LoaderBase[][] _decorLoaders;

	public MortalFactory(int _spawnX, int _spawnY, LoaderBase[][] _cl)
	{
		_decorLoaders = _cl;
		spawnX = _spawnX;
		spawnY = _spawnY;
		int ww = MainGame.Width;
		int hh = MainGame.Height;
		int _terrainTileWidth = (ww / 32) + MainGame.ADDONWIDTH;
		int _terrainTileHeight = (hh / 32) + MainGame.ADDONHEIGHT;
		_terrainTileWidth -= _terrainTileWidth % 4;
		_terrainTileHeight -= _terrainTileHeight % 4;
		loadData();
	}

	private void loadData()
	{
		Decoration s;
		int ww = MainGame.Width;
		int hh = MainGame.Height;
		int _terrainTileWidth = (ww / 32) + MainGame.ADDONWIDTH;
		int _terrainTileHeight = (hh / 32) + MainGame.ADDONHEIGHT;
		_terrainTileWidth -= _terrainTileWidth % 4;
		_terrainTileHeight -= _terrainTileHeight % 4;
		int i, j;
		byte spriteVal;
		LoaderBase ld;
		int _terrainTileCollectionWidth = _terrainTileWidth / 2;
		int _terrainTileCollectionHeight = _terrainTileHeight / 2;
		decorSprites = new Decoration[_terrainTileCollectionHeight][_terrainTileCollectionWidth];
		for (i = 0; i < _terrainTileCollectionHeight; i = i + 1)
		{
			for (j = 0; j < _terrainTileCollectionWidth; j = j + 1)
			{
				s = new Decoration();
				decorSprites[i][j] = s;
				if (i == 0 && j == 0)
					mostLeftSprite = mostUpSprite = s;
				else if (i == _terrainTileCollectionHeight - 1
						&& j == _terrainTileCollectionWidth - 1)
					mostRightSprite = mostDownSprite = s;
				s.mySectorX = spawnX;
				s.mySectorY = spawnY;
				s.myVectorSpotX = j;
				s.myVectorSpotY = i;
				s.mySectorSpotX = j;
				s.mySectorSpotY = i;
				s.setX(j * 64);
				s.setY(i * 64);
				ld = getSpriteDecor(s.mySectorX, s.mySectorY);
				spriteVal = ld.getDecorData()[s.mySectorSpotX][s.mySectorSpotY];
				s.setVisible(spriteVal != 0);
				if (s.getVisible())
					s.setFrame(spriteVal);
			}
		}
		_decorsLoaded = true;
	}

	private LoaderBase getSpriteDecor(int sectorX, int sectorY)
	{
		int i, j;
		for (i = 0; i < 3; i = i + 1)
		{
			for (j = 0; j < 3; j = j + 1)
			{
				if (_decorLoaders[i][j].getMySectorX() == sectorX
						&& _decorLoaders[i][j].getMySectorY() == sectorY)
				{
					return _decorLoaders[i][j];
				}
			}
		}
		System.out
				.println("could not find sectors: " + sectorX + "," + sectorY);
		System.out.println("in...");
		return null;
	}

	public void logic(OrthographicCamera camera)
	{
		if (_decorsLoaded)
			checkDecorations(camera);
	}

	private void checkDecorations(OrthographicCamera c)
	{
		float cameraX = (float) (c.position.x - (c.viewportWidth * .5));
		float cameraY = (float) (c.position.y - (c.viewportHeight * .5));
		if (mostLeftSprite.getX() + _tileBuffer > cameraX)
			moveLeft();
		if (mostRightSprite.getX() - _tileBuffer < c.viewportWidth + cameraX)
			moveRight();
		if (mostUpSprite.getY() + _tileBuffer > cameraY)
			moveUp();
		if ((mostDownSprite.getY() + 64) - _tileBuffer < c.viewportHeight
				+ cameraY)
			moveDown();
	}

	private void moveDown()
	{
		// System.out.println( "down" );
		for (innerlooper = 0; innerlooper < decorSprites[0].length; innerlooper++)
			decorSprites[mostUpSprite.myVectorSpotY][innerlooper]
					.setY(mostDownSprite.getY() + 64);
		// swap because everything moved
		Decoration tempSprite = mostDownSprite;
		mostDownSprite = mostUpSprite;
		int vectorY = mostDownSprite.myVectorSpotY;
		// get lowest sectorspotx, then loop it and give it to everybody else
		mostDownSprite.mySectorSpotY = tempSprite.mySectorSpotY;
		mostDownSprite.mySectorY = tempSprite.mySectorY;
		if (mostDownSprite.mySectorSpotY + 1 >= LoaderBase.DATA_SQUARE)
		{
			mostDownSprite.mySectorY = mostDownSprite.mySectorY + 1;
			mostDownSprite.mySectorSpotY = 0;
		} else
			mostDownSprite.mySectorSpotY = tempSprite.mySectorSpotY + 1;
		for (innerlooper = 0; innerlooper < decorSprites[0].length; innerlooper++)
		{
			tempSprite = decorSprites[mostUpSprite.myVectorSpotY][innerlooper];
			tempSprite.mySectorY = mostDownSprite.mySectorY;
			tempSprite.mySectorSpotY = mostDownSprite.mySectorSpotY;
			LoaderBase ld = getSpriteDecor(tempSprite.mySectorX,
					tempSprite.mySectorY);
			byte spriteVal = ld.getDecorData()[tempSprite.mySectorSpotX][tempSprite.mySectorSpotY];
			tempSprite.setVisible(spriteVal != 0);
			if (tempSprite.getVisible())
				tempSprite.setFrame(spriteVal);
		}
		if (vectorY + 1 >= decorSprites.length)
			vectorY = 0;
		else
			vectorY = vectorY + 1;
		mostUpSprite = decorSprites[vectorY][0];
		// //updateVisibles();
	}

	private void moveLeft()
	{
		// System.out.println( "left" );
		for (innerlooper = 0; innerlooper < decorSprites.length; innerlooper++)
			decorSprites[innerlooper][mostRightSprite.myVectorSpotX]
					.setX(mostLeftSprite.getX() - 64);
		// swap because everything moved
		Decoration tempSprite = mostLeftSprite;
		mostLeftSprite = mostRightSprite;
		int vectorX = mostLeftSprite.myVectorSpotX;
		// get lowest sectorspotx, then loop it and give it to everybody else
		mostLeftSprite.mySectorSpotX = tempSprite.mySectorSpotX;
		mostLeftSprite.mySectorX = tempSprite.mySectorX;
		if (mostLeftSprite.mySectorSpotX - 1 < 0)
		{
			mostLeftSprite.mySectorX--;
			mostLeftSprite.mySectorSpotX = LoaderBase.DATA_SQUARE - 1;
		} else
			mostLeftSprite.mySectorSpotX = tempSprite.mySectorSpotX - 1;
		for (innerlooper = 0; innerlooper < decorSprites.length; innerlooper++)
		{
			tempSprite = decorSprites[innerlooper][mostRightSprite.myVectorSpotX];
			tempSprite.mySectorX = mostLeftSprite.mySectorX;
			tempSprite.mySectorSpotX = mostLeftSprite.mySectorSpotX;
			LoaderBase ld = getSpriteDecor(tempSprite.mySectorX,
					tempSprite.mySectorY);
			byte spriteVal = ld.getDecorData()[tempSprite.mySectorSpotX][tempSprite.mySectorSpotY];
			tempSprite.setVisible(spriteVal != 0);
			if (tempSprite.getVisible())
				tempSprite.setFrame(spriteVal);
		}
		if (vectorX - 1 < 0)
			vectorX = decorSprites[0].length - 1;
		else
			--vectorX;
		mostRightSprite = decorSprites[0][vectorX];
		// updateVisibles();
	}

	private void moveUp()
	{
		// System.out.println( "hit up" );
		for (innerlooper = 0; innerlooper < decorSprites[0].length; innerlooper++)
			decorSprites[mostDownSprite.myVectorSpotY][innerlooper]
					.setY(mostUpSprite.getY() - 64);
		// swap because everything moved
		Decoration tempSprite = mostUpSprite;
		mostUpSprite = mostDownSprite;
		int vectorY = mostUpSprite.myVectorSpotY;
		// get lowest sectorspotx, then loop it and give it to everybody else
		mostUpSprite.mySectorSpotY = tempSprite.mySectorSpotY;
		mostUpSprite.mySectorY = tempSprite.mySectorY;
		if (mostUpSprite.mySectorSpotY - 1 < 0)
		{
			mostUpSprite.mySectorY--;
			mostUpSprite.mySectorSpotY = LoaderBase.DATA_SQUARE - 1;
		} else
			mostUpSprite.mySectorSpotY = tempSprite.mySectorSpotY - 1;
		for (innerlooper = 0; innerlooper < decorSprites[0].length; innerlooper++)
		{
			tempSprite = decorSprites[mostDownSprite.myVectorSpotY][innerlooper];
			tempSprite.mySectorY = mostUpSprite.mySectorY;
			tempSprite.mySectorSpotY = mostUpSprite.mySectorSpotY;
			LoaderBase ld = getSpriteDecor(tempSprite.mySectorX,
					tempSprite.mySectorY);
			byte spriteVal = ld.getDecorData()[tempSprite.mySectorSpotX][tempSprite.mySectorSpotY];
			tempSprite.setVisible(spriteVal != 0);
			if (tempSprite.getVisible())
				tempSprite.setFrame(spriteVal);
		}
		if (vectorY - 1 < 0)
			vectorY = decorSprites.length - 1;
		else
			--vectorY;
		mostDownSprite = decorSprites[vectorY][0];
		// updateVisibles();
	}

	private void moveRight()
	{
		// System.out.println( "right hit" );
		// X COORDINATE SHIFT
		for (innerlooper = 0; innerlooper < decorSprites.length; innerlooper++)
			decorSprites[innerlooper][mostLeftSprite.myVectorSpotX]
					.setX(mostRightSprite.getX() + 64);
		Decoration tempSprite = mostRightSprite;
		mostRightSprite = mostLeftSprite;
		int vectorX = mostRightSprite.myVectorSpotX;
		// get lowest sectorspotx, then loop it and give it to everybody else
		mostRightSprite.mySectorSpotX = tempSprite.mySectorSpotX;
		mostRightSprite.mySectorX = tempSprite.mySectorX;
		if (mostRightSprite.mySectorSpotX + 1 >= LoaderBase.DATA_SQUARE)
		{
			mostRightSprite.mySectorX = mostRightSprite.mySectorX + 1;
			mostRightSprite.mySectorSpotX = 0;
		} else
			mostRightSprite.mySectorSpotX = tempSprite.mySectorSpotX + 1;
		for (innerlooper = 0; innerlooper < decorSprites.length; innerlooper++)
		{
			tempSprite = decorSprites[innerlooper][mostRightSprite.myVectorSpotX];
			tempSprite.mySectorX = mostRightSprite.mySectorX;
			tempSprite.mySectorSpotX = mostRightSprite.mySectorSpotX;
			LoaderBase ld = getSpriteDecor(tempSprite.mySectorX,
					tempSprite.mySectorY);
			byte spriteVal = ld.getDecorData()[tempSprite.mySectorSpotX][tempSprite.mySectorSpotY];
			tempSprite.setVisible(spriteVal != 0);
			if (tempSprite.getVisible())
				tempSprite.setFrame(spriteVal);
		}
		if (vectorX + 1 >= decorSprites[0].length)
			vectorX = 0;
		else
			vectorX = vectorX + 1;
		mostLeftSprite = decorSprites[0][vectorX];
		// updateVisibles();
	}

	private void updateVisibles()
	{
		short i, j;
		int index = 0;
		visibleDecors.clear();
		for (j = 0; j < decorSprites[0].length; j++)
		{
			for (i = 0; i < decorSprites.length; i++)
			{
				if (decorSprites[i][j].getVisible())
				{
					if (visibleDecors.size() <= index)
						visibleDecors.add(decorSprites[i][j]);
					else
						visibleDecors.set(index, decorSprites[i][j]);
					index++;
				}
			}
		}
		Collections.sort(visibleDecors, tc);
	}

	private DecorationComparer tc = new DecorationComparer();
	private ArrayList<Decoration> visibleDecors = new ArrayList<Decoration>();

	public void render(SpriteBatch batch)
	{
		Decoration currTexture;
		updateVisibles();
		if (visibleDecors != null)
		{
			// using traditional for loop
			for (int i = 0; i < visibleDecors.size(); i++)
			{
				currTexture = visibleDecors.get(i);
				if (currTexture != null)
				{
					batch.draw(currTexture, currTexture.getModX(),
							currTexture.getModY());
				} else
				{
					break;
				}
			}
		}
	}

	public void dispose()
	{
		// TODO Auto-generated method stub
	}
}
