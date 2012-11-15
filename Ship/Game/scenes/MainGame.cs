package game.scenes;

import game.ShipGame;
import game.Loaders.LoaderBase;
import game.accessors.CameraAccessor;
import game.beans.factories.MortalFactory;
import game.beans.factories.TileFactory;
import game.beans.terrain.RegionCache;
import game.utils.WaterPoint;
import game.worldgen.FileAtts;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import threading.MiniMap;
import aurelienribon.tweenengine.Tween;
import aurelienribon.tweenengine.TweenManager;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureAtlas;
import com.badlogic.gdx.graphics.g2d.TextureAtlas.AtlasRegion;
import com.badlogic.gdx.utils.Json;

public class MainGame implements Screen
{
	// must be divisible by 64!!!!!
	private static final int WIDTH = 832;
	private static final int HEIGHT = 832;
	private static int spawnX = 32;

	public static int getSpawnX()
	{
		return spawnX;
	}

	public static int getSpawnY()
	{
		return spawnY;
	}

	private static int spawnY = 330;
	private SpriteBatch batch;
	// BitmapFont font;
	
	private MortalFactory mortalFactory;
	private OrthographicCamera cam;
	private LoaderBase[][] _contentLoaders = new LoaderBase[3][3];
	public static MainGame GAME;
	public static final int MAX_UPDATE_ITERATIONS = 5;
	public static final float fixedTimeStep = 1 / 60f;
	public static final int tweenTimeStep = (int) (fixedTimeStep * 1000);
	private float accum = 0;
	private int iterations = 0;
	private TweenManager tweenManager;
	public static final byte ADDONWIDTH = 15;
	public static final byte ADDONHEIGHT = 10;
	private ShipGame sg;
	private List<AtlasRegion> regions;
	private MiniMap minimap;
	// private Texture mapTexture = new Texture(
	// Gdx.files.internal("assets/mapholder.png"));
	// private Texture mapCase = new Texture(
	// Gdx.files.internal("assets/mapholder.png"));
	private FileAtts fa;
	private ExecutorService tpes = Executors.newFixedThreadPool(3);

	public MainGame(ShipGame shipGame)
	{
		super();
		sg = shipGame;
	}

	public void show()
	{
		if (GAME != null)
		{
			try
			{
				throw new IOException(
						"ALREADY CREATED, do not create again! bad bad");
			} catch (Exception e)
			{
			}
		}
		{
			GAME = this;
			setupLoaders(getFileAtts());
			setupCamera();
			TextureAtlas atlas = new TextureAtlas(
					Gdx.files.internal("assets/decorations.txt"));
			regions = atlas.getRegions();
			RegionCache.makeCache(regions);
			// reg.flip(false, true);
			// sprite = new Decoration();
			// sprite.setFrame(TerrainConstants.TREE_HOT);
			// tileRegions = textureReg.split(32, 32);
			fa = getFileAtts();
			minimap = new MiniMap();
			getMinimap().setSector(spawnX, spawnY, fa);
			// minimap.execute();
			tileFactory = new TileFactory(spawnX, spawnY, _contentLoaders);
			// mortalFactory = new MortalFactory(spawnX, spawnY,
			// _contentLoaders);
			batch = new SpriteBatch();
			Gdx.gl.glClearColor(0, 1, 0, 1);
			tweenManager = new TweenManager();
			Tween.registerAccessor(OrthographicCamera.class,
					new CameraAccessor());
			Gdx.graphics.setVSync(false);
		}
	}

	private FileAtts getFileAtts()
	{
		InputStream input;
		FileAtts r = null;
		try
		{
			input = new FileInputStream("C:\\Users\\Jared\\game\\init.dat");
			r = new Json().fromJson(FileAtts.class, input);
			input.close();
		} catch (IOException e)
		{
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return r;
	}

	public byte[][] getIndividualData(String name, int w, int h)
	{
		byte[][] ba = new byte[w][h];
		try
		{
			FileInputStream fin = new FileInputStream(name);
			for (int _width = 0; _width < w; _width++)
			{
				for (int _height = 0; _height < h; _height++)
				{
					ba[_width][_height] = (byte) fin.read();
				}
			}
			fin.close();
		} catch (IOException e)
		{
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return ba;
	}

	public List<AtlasRegion> getAtlas()
	{
		return regions;
	}

	private void setupCamera()
	{
		cam = new OrthographicCamera(MainGame.getWidth(), MainGame.getHeight());
		cam.setToOrtho(true);
	}

	public void updateMiniMap(LoaderBase center)
	{
		getMinimap()
				.setSector(center.getMySectorX(), center.getMySectorY(), fa);
	}

	private void setupLoaders(FileAtts fa)
	{
		int i, j;
		for (i = 0; i < 3; i = i + 1)
		{
			for (j = 0; j < 3; j = j + 1)
			{
				// needsLoaded = needsLoaded + 1;
				_contentLoaders[i][j] = new LoaderBase(spawnX + (i - 1), spawnY
						+ (j - 1), fa);
				// if (i == 1)
				// _contentLoaders[i][j].setWater(true);
			}
		}
		for (i = 0; i < 3; i = i + 1)
		{
			for (j = 0; j < 3; j = j + 1)
			{
				tpes.execute(_contentLoaders[i][j]);
			}
		}
	}

	private void gameLogic()
	{
		keyboardLogic();
		tileFactory.logic(cam);
		// mortalFactory.logic(cam);
	}

	private void batchRender()
	{
		batch.setProjectionMatrix(cam.combined);
		batch.begin();
		batch.disableBlending();
		tileFactory.render(batch);
		if (getMinimap().getTexture() != null)
		{
			getMinimap().setCoordinates(cam);
			batch.draw(getMinimap().getTexture(), getMinimap().getX(),
					getMinimap().getY());
		}
		batch.enableBlending();
		// batch.draw(this.mapCase, cam.position.x - (cam.viewportWidth / 2),
		// cam.position.y - (cam.viewportHeight / 2));
		// batch.draw(sprite, sprite.getModX(), sprite.getModY());
		// mortalFactory.render(batch);
		batch.end();
	}

	private void cameraRender()
	{
		GL20 gl = Gdx.graphics.getGL20();
		gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
		cam.update();
		// cam.apply(gl);
	}

	private final int velocity = 9;

	private void keyboardLogic()
	{
		// if (Gdx.input.isKeyPressed(Input.Keys.A))
		// {
		// cam.zoom += 0.02;
		// }
		// if (Gdx.input.isKeyPressed(Input.Keys.Q))
		// {
		// cam.zoom -= 0.02;
		// }
		if (Gdx.input.isKeyPressed(Input.Keys.LEFT))
		{
			Tween.to(cam, CameraAccessor.POSITION_X, .01f)
					.target(cam.position.x - velocity).start(tweenManager);
		}
		if (Gdx.input.isKeyPressed(Input.Keys.RIGHT))
		{
			Tween.to(cam, CameraAccessor.POSITION_X, .01f)
					.target(cam.position.x + velocity).start(tweenManager);
		}
		if (Gdx.input.isKeyPressed(Input.Keys.DOWN))
		{
			Tween.to(cam, CameraAccessor.POSITION_Y, .01f)
					.target(cam.position.y + velocity).start(tweenManager);
		}
		if (Gdx.input.isKeyPressed(Input.Keys.UP))
		{
			Tween.to(cam, CameraAccessor.POSITION_Y, .01f)
					.target(cam.position.y - velocity).start(tweenManager);
		}
		// if (Gdx.input.isKeyPressed(Input.Keys.W))
		// {
		// cam.rotate(-0.5f, 0, 0, 1);
		// }
		// if (Gdx.input.isKeyPressed(Input.Keys.E))
		// {
		// cam.rotate(0.5f, 0, 0, 1);
		// }
	}

	public void resize(int width, int height)
	{
	}

	public void pause()
	{
	}

	public void resume()
	{
	}

	public void dispose()
	{
		tileFactory.dispose();
		// mortalFactory.dispose();
	}

	public static int getWidth()
	{
		return WIDTH;
	}

	public static int getHeight()
	{
		return HEIGHT;
	}

	public ArrayList<WaterPoint[]> getArrayOfTilesAroundMe(LoaderBase loaderBase)
	{
		short outterI, outterJ;
		ArrayList<WaterPoint[]> points = new ArrayList<WaterPoint[]>();
		for (outterI = 0; outterI < 3; outterI++)
		{
			for (outterJ = 0; outterJ < 3; outterJ++)
			{
				loopContents(points, outterI, outterJ);
			}
		}
		return points;
	}

	private void loopContents(ArrayList<WaterPoint[]> points, short outteri,
			short outterj)
	{
		LoaderBase leftLoader = getLoaderBase(
				_contentLoaders[outteri][outterj].getMySectorX() - 1,
				_contentLoaders[outteri][outterj].getMySectorY());
		addPoints(leftLoader, points, outteri, outterj);
		LoaderBase rightLoader = getLoaderBase(
				_contentLoaders[outteri][outterj].getMySectorX() + 1,
				_contentLoaders[outteri][outterj].getMySectorY());
		addPoints(rightLoader, points, outteri, outterj);
		LoaderBase downLoader = getLoaderBase(
				_contentLoaders[outteri][outterj].getMySectorX(),
				_contentLoaders[outteri][outterj].getMySectorY() + 1);
		addPoints(downLoader, points, outteri, outterj);
		LoaderBase upLoader = getLoaderBase(
				_contentLoaders[outteri][outterj].getMySectorX(),
				_contentLoaders[outteri][outterj].getMySectorY() - 1);
		addPoints(upLoader, points, outteri, outterj);
	}

	private void addPoints(LoaderBase lb, ArrayList<WaterPoint[]> points,
			short i, short j)
	{
		if (lb != null && lb.hasWater())
		{
			if (_contentLoaders[i][j].waterLocs()[0] != null)
				points.add(_contentLoaders[i][j].waterLocs());
		}
	}

	public LoaderBase getLoaderBase(int sectorX, int sectorY)
	{
		int i, j;
		for (i = 0; i < 3; i = i + 1)
		{
			for (j = 0; j < 3; j = j + 1)
			{
				if (_contentLoaders[i][j].getMySectorX() == sectorX
						&& _contentLoaders[i][j].getMySectorY() == sectorY)
				{
					return _contentLoaders[i][j];
				}
			}
		}
		// System.out
		// .println("could not find sectors: " + sectorX + "," + sectorY);
		// System.out.println("in...");
		return null;
	}

	@Override
	public void render(float delta)
	{
		/********** Logic *************/
		accum += delta;
		iterations = 0;
		while (accum > fixedTimeStep && iterations < MAX_UPDATE_ITERATIONS)
		{
			tweenManager.update(tweenTimeStep);
			gameLogic();
			accum -= fixedTimeStep;
			iterations++;
		}
		/********** Render ***********/
		cameraRender();
		batchRender();
	}

	@Override
	public void hide()
	{
		// TODO Auto-generated method stub
	}

	public MiniMap getMinimap()
	{
		return minimap;
	}
}
