using Autofac;
using Gum;
using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI;
using Manhead.Core.Logic.Gameplay;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Manhead.Core.Logic.Editor;

public class RunScreen : Screen
{
    private readonly ILifetimeScope _scope;
    
    private readonly SpriteBatch _spriteBatch;
    private readonly ScreenLayout _screenLayout;
    private readonly GameLoop _gameLoop;
    private readonly GameInput _input;
    
    public RunScreen(IContainer services, LevelBlueprint level)
    {
        _scope = services.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(level).SingleInstance();;
            builder.RegisterType<GameInput>().SingleInstance();;
            builder.RegisterType<EventBus>().SingleInstance();;
            builder.Register<GridLayout>(context =>
            {
                var layout = context.Resolve<ScreenLayout>();
                return new(layout.ToPixels(1, 1));
            }).SingleInstance();;
            builder.RegisterType<GameLoop>().SingleInstance();;
            builder.RegisterType<RunUI>().SingleInstance();;
        });
        
        _spriteBatch = _scope.Resolve<SpriteBatch>();
        _screenLayout = _scope.Resolve<ScreenLayout>();
        _input =  _scope.Resolve<GameInput>();
        _gameLoop = _scope.Resolve<GameLoop>();
        var runUI = _scope.Resolve<RunUI>();
        var gum = _scope.Resolve<GumService>();
        gum.Root.AddChild(runUI);
    }

    public override void Dispose()
    {
        var runUI = _scope.Resolve<RunUI>();
        var gum = _scope.Resolve<GumService>();
        gum.Root.RemoveChild(runUI);
        _scope.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
        _input.Update();
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            rasterizerState: RasterizerState.CullNone,
            transformMatrix: _screenLayout.Camera.GetViewMatrix()
        );
        
        _gameLoop.Draw();
        
        _spriteBatch.End();
    }
}