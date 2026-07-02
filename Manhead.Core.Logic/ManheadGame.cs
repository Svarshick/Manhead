using Autofac;
using Gum;
using Manhead.Core.Logic.Editor;
using Manhead.Core.Logic.Editor.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Manhead.Core.Logic;

public sealed class ManheadGame : Game
{
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    public static DefaultSystem DefaultSystem = new();

    public static ScreenLayout ScreenLayout { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }

    public new IContainer Services { get; private set; }
    
    private GraphicsDeviceManager _graphicsDeviceManager;
    private GumService _gumService;
    private ScreenManager _screenManager;
    
    private bool _isResizing;

    public ManheadGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
    }

    protected override void Initialize()
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(this).SingleInstance();
        builder.RegisterInstance(_graphicsDeviceManager).SingleInstance();
        InitScreen();
        InitSystems();
        TestStuff();
        Services = builder.Build();
        base.Initialize();
        return;

        void InitScreen()
        {
            ScreenLayout = new ScreenLayout(GraphicsDevice);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            ScreenLayout.FollowPosition(Vector2.Zero);
            _graphicsDeviceManager.PreferredBackBufferWidth = ScreenLayout.WidthResolution;
            _graphicsDeviceManager.PreferredBackBufferHeight = ScreenLayout.HeightResolution;
            _graphicsDeviceManager.IsFullScreen = false; 
            _graphicsDeviceManager.ApplyChanges();
            
            builder.RegisterInstance(GraphicsDevice).SingleInstance();
            builder.RegisterInstance(ScreenLayout).SingleInstance();
        }

        void InitSystems()
        {
            _gumService = GumService.Default;
            _gumService.Initialize(this);
            _gumService.UseSingleThreadedAsync();
            _gumService.EnableExpandToWindow();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _screenManager = new ScreenManager();

            builder.RegisterInstance(Content).SingleInstance();
            builder.RegisterInstance(_gumService).SingleInstance();
            builder.RegisterInstance(SpriteBatch).SingleInstance();
            builder.RegisterInstance(_screenManager).SingleInstance();
        }

        void TestStuff()
        {
        }
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        _screenManager.ShowScreen(new EditScreen(Services));
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        if (_isResizing) return;

        if (Window.ClientBounds is { Width: > 0, Height: > 0 })
        {
            _isResizing = true;
            ScreenLayout.WidthResolution = Window.ClientBounds.Width; 
            ScreenLayout.HeightResolution = Window.ClientBounds.Height;
            
            _graphicsDeviceManager.PreferredBackBufferWidth = ScreenLayout.WidthResolution;
            _graphicsDeviceManager.PreferredBackBufferHeight = ScreenLayout.HeightResolution;
            _graphicsDeviceManager.ApplyChanges();
            _isResizing = false;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);
        _screenManager.Update(gameTime);
        _gumService.Update(gameTime);
        MonoTask.Update();
        GameObjectPool.Update();
        
        base.Update(gameTime);
        LateUpdate();
    }

    private void LateUpdate()
    {
        GameObjectPool.LateUpdate();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        GameObjectPool.Draw();
        _screenManager.Draw(gameTime);
        _gumService.Draw();
        base.Draw(gameTime);
    }
}