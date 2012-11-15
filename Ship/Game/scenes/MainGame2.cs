package game.scenes;

import game.accessors.CameraAccessor;
import aurelienribon.tweenengine.Tween;
import aurelienribon.tweenengine.TweenManager;

import com.badlogic.gdx.ApplicationListener;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.GL10;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureAtlas;
import com.badlogic.gdx.math.Rectangle;

public class MainGame2 implements ApplicationListener
{
	public static final int WIDTH = 640;
	public static final int HEIGHT = 640;
	SpriteBatch batch;
	TextureAtlas atlas;
	BitmapFont font;
	Sprite sprite;
	private OrthographicCamera cam;
	private Rectangle glViewport;

	public void create()
	{
		setupCamera();
		atlas = new TextureAtlas(Gdx.files.internal("assets/decorations.txt"));
		sprite = atlas.createSprite("Tile_Local");
		// sprite.getTexture().setFilter(TextureFilter.MipMap,
		// TextureFilter.MipMap);
		batch = new SpriteBatch();
		tweenManager = new TweenManager();
		font = new BitmapFont(true);
		Tween.registerAccessor(OrthographicCamera.class, new CameraAccessor());
		Gdx.graphics.setVSync(false);
	}

	public TextureAtlas getAtlas()
	{
		return atlas;
	}

	private void setupCamera()
	{
		cam = new OrthographicCamera(Gdx.graphics.getWidth(),
				Gdx.graphics.getHeight());
		cam.setToOrtho(false, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
		glViewport = new Rectangle(0, 0, Gdx.graphics.getWidth(),
				Gdx.graphics.getHeight());
	}

	public static final int MAX_UPDATE_ITERATIONS = 5;
	public static final float fixedTimeStep = 1 / 60f;
	public static final int tweenTimeStep = (int) (fixedTimeStep * 1000);
	private float accum = 0;
	private int iterations = 0;
	private float delta = 0;
	private TweenManager tweenManager;

	@Override
	public void render()
	{
		/********** Logic *************/
		delta = Gdx.graphics.getDeltaTime();
		accum += delta;
		iterations = 0;
		while (accum > fixedTimeStep && iterations < MAX_UPDATE_ITERATIONS)
		{
			tweenManager.update(tweenTimeStep);
			// mainStage.act(fixedTimeStep);
			// world.step(fixedTimeStep, 3, 2);
			gameLogic();
			accum -= fixedTimeStep;
			iterations++;
		}
		/********** Render ***********/
		cameraRender();
		batchRender();
	}

	private void gameLogic()
	{
		keyboardLogic();
		// tileFactory.logic(cam);
	}

	private void batchRender()
	{
		batch.setProjectionMatrix(cam.combined);
		batch.begin();
		sprite.draw(batch);
		// System.out.println("fps:" + Gdx.graphics.getFramesPerSecond());
		// font.draw(batch, "fps" + Gdx.graphics.getFramesPerSecond(),
		// cam.position.x, cam.position.y);
		batch.end();
	}

	private void cameraRender()
	{
		GL10 gl = Gdx.graphics.getGL10();
		gl.glClear(GL10.GL_COLOR_BUFFER_BIT);
		// gl.glViewport((int) glViewport.x, (int) glViewport.y,
		// (int) glViewport.width, (int) glViewport.height);
		cam.update();
		cam.apply(gl);
	}

	private int velocity = 150;

	private void keyboardLogic()
	{
		// System.out.println(" Gdx.graphics.getDeltaTime():"
		// + Gdx.graphics.getDeltaTime());
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
			// cam.position.x -= (int) (Gdx.graphics.getDeltaTime() * velocity);
			Tween.to(cam, CameraAccessor.POSITION_X, 20.0f)
					.target(cam.position.x - 3).start(tweenManager);
		}
		if (Gdx.input.isKeyPressed(Input.Keys.RIGHT))
		{
			Tween.to(cam, CameraAccessor.POSITION_X, 20.0f)
					.target(cam.position.x + 3).start(tweenManager);
		}
		// if (Gdx.input.isKeyPressed(Input.Keys.DOWN))
		// {
		// cam.translate(0, 2.9f, 0);
		// }
		// if (Gdx.input.isKeyPressed(Input.Keys.UP))
		// {
		// cam.translate(0, -2.9f, 0);
		// }
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
		// tileFactory.dispose();
	}
}
