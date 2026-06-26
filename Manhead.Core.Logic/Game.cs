using Manhead.Core.Logic.Editor;
using Manhead.Core.Logic.Editor.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGameGum;

namespace Manhead.Core.Logic;

public sealed class Game : Microsoft.Xna.Framework.Game
{
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    public static DefaultSystem DefaultSystem = new();

    public static ScreenLayout ScreenLayout { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static ContentManager Content { get; private set; }
    public static GumService GumService => GumService.Default;

    private readonly GraphicsDeviceManager _graphics;
    public readonly ScreenManager ScreenManager;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        ScreenManager = new ScreenManager();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitScreen();
        InitSystems();
        TestStuff();
        ScreenManager.ShowScreen(new EditScreen(this));
        return;

        void InitScreen()
        {
            IsMouseVisible = true;
            ScreenLayout = new ScreenLayout(Window, GraphicsDevice);
            ScreenLayout.FollowPosition(Vector2.Zero);
            _graphics.PreferredBackBufferWidth = ScreenLayout.WidthResolution;
            _graphics.PreferredBackBufferHeight = ScreenLayout.HeightResolution;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            
            Services.AddService(ScreenLayout);
        }
        
        void InitSystems()
        {
            GumService.Initialize(this);
            GumService.UseSingleThreadedAsync();
            
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Content = base.Content;

            Services.AddService(Content);
            Services.AddService(SpriteBatch);
        }
        
        void TestStuff()
        {
        }
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);
        ScreenManager.Update(gameTime);
        GumService.Update(gameTime);
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
        ScreenManager.Draw(gameTime);
        GumService.Draw();
        base.Draw(gameTime);
    }
}