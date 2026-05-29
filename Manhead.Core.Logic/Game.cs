using Manhead.Core.Logic.Editor;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;

namespace Manhead.Core.Logic;

public sealed class Game : Microsoft.Xna.Framework.Game
{
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
    
    public static ScreenLayout ScreenLayout { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static ContentManager Content { get; private set; }
    public static GumService GumService => GumService.Default;

    private GraphicsDeviceManager _graphicsDeviceManager;

    private Input _input;
    private GridLayout _gridLayout;
    private WorldEditor _editor;


    public Game()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitScreen();
        InitSystems();
        DoStuff();
        return;

        void InitScreen()
        {
            IsMouseVisible = true;
            ScreenLayout = new ScreenLayout(Window, GraphicsDevice);
            ScreenLayout.FollowPosition(Vector2.Zero);
            _graphicsDeviceManager.PreferredBackBufferWidth = ScreenLayout.WidthResolution;
            _graphicsDeviceManager.PreferredBackBufferHeight = ScreenLayout.HeightResolution;
            _graphicsDeviceManager.IsFullScreen = true;
            _graphicsDeviceManager.ApplyChanges();
        }
        
        void InitSystems()
        {
            GumService.Initialize(this);
            GumService.UseSingleThreadedAsync();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Content = base.Content;
            _input = new Input();
            _gridLayout = new GridLayout(ScreenLayout.ToPixels(1, 1));
            _editor = new WorldEditor(_input.Editor, GraphicsDevice, _gridLayout);
        }
        
        void DoStuff()
        {
            //some tests here
        }
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);
        _input.Update();
        GumService.Update(gameTime);
        MonoTask.Update();
        GameObjectPool.Update();
        
        base.Update(gameTime);
        LateUpdate();
    }

    private void LateUpdate()
    {
        _input.LateUpdate();
        GameObjectPool.LateUpdate();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        
        SpriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            rasterizerState: RasterizerState.CullNone,
            transformMatrix: ScreenLayout.Camera.GetViewMatrix()
        );
        
        _editor.Draw();
        GameObjectPool.Draw();
        
        SpriteBatch.End();
        
        GumService.Draw();
        base.Draw(gameTime);
    }
}