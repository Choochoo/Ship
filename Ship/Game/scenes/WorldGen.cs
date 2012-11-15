package game.scenes;

import game.ShipGame;
import game.utils.Utility;
import game.worldgen.CreateWorld;
import game.worldgen.worlddrawing.MapData;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

public class WorldGen implements Screen
{
	private SpriteBatch spriteBatch;
	private ShipGame myGame;
	private BitmapFont font;

	public WorldGen(ShipGame g)
	{
		super();
		myGame = g;
	}

	@Override
	public void render(float delta)
	{
		Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
		spriteBatch.begin();
		spriteBatch.enableBlending();
		spriteBatch.draw(tex, 100, 50);
		// font.draw(spriteBatch, "my-string", 20, 20);
		spriteBatch.end();
	}

	private Texture tex;

	@Override
	public void show()
	{
		MapData.setWIDTH(512);
		MapData.setHEIGHT(512);
		Utility.createNewSeeds();
		// createGame(512, 512);
		// CreateArea ca = new CreateArea(0, 448, cw);
		// tex = new Texture(ca);
		CreateWorld cw = new CreateWorld(false, false);
		// CreateArea ca = new CreateArea(0, 0, 3, cw);
		cw.dispose();
		cw = null;
		// tex = new Texture(ca);
		spriteBatch = new SpriteBatch();
		// font = new BitmapFont();
	}

	private void createGame(int w, int h)
	{
		CreateWorld cw = new CreateWorld(false, true);
		// CreateArea ca = new CreateArea(0, 0, 3, cw);
		// for (int i = 0; i < MapData.getWIDTH(); i++)
		// {
		// for (int j = 0; j < MapData.getHEIGHT(); j++)
		// {
		// CreateArea ca = new CreateArea(i, j,3, cw);
		// PixmapIO.writeCIM(
		// Gdx.files.external("\\game\\" + i + "-" + j + ".dat"),
		// ca);
		// ca.dispose();
		// ca = null;
		// }
		// }
		// ca.dispose();
		// ca = null;
		cw.dispose();
		cw = null;
	}

	@Override
	public void resize(int width, int height)
	{
		// TODO Auto-generated method stub
	}

	@Override
	public void hide()
	{
		// TODO Auto-generated method stub
	}

	@Override
	public void pause()
	{
		// TODO Auto-generated method stub
	}

	@Override
	public void resume()
	{
		// TODO Auto-generated method stub
	}

	@Override
	public void dispose()
	{
		// TODO Auto-generated method stub
	}
}