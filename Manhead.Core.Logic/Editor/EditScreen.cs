using Autofac;
using Gum;
using Gum.Wireframe;
using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Manhead.Core.Logic.Editor;

public class EditScreen : Screen
{
    private readonly ILifetimeScope _scope;
    
    private SpriteBatch _spriteBatch;
    private ScreenLayout _screenLayout;
    private WorldEditor _worldEditor;
    private Input _input;
   
    public EditScreen(IContainer services)
    {
        _scope = services.BeginLifetimeScope(builder =>
        {
            builder.RegisterType<EditorInput>().AsSelf().As<IUpdatable>().SingleInstance();
            builder.RegisterType<Input>().SingleInstance();
            builder.RegisterType<EventBus>().SingleInstance();
            builder.Register<GridLayout>(context =>
            {
                var layout = context.Resolve<ScreenLayout>();
                return new (layout.ToPixels(1, 1));
            }).SingleInstance();;
            builder.Register<TemplateHolder>(context =>
            {
                var gridLayout = context.Resolve<GridLayout>();
                return new(gridLayout, 100, 100);
            }).SingleInstance();;
            builder.RegisterType<EditorUI>().AsSelf().As<InteractiveGue>().SingleInstance();;
            builder.RegisterType<WorldEditor>().SingleInstance();
        });
        
        _spriteBatch = _scope.Resolve<SpriteBatch>();
        _screenLayout = _scope.Resolve<ScreenLayout>();
        _worldEditor = _scope.Resolve<WorldEditor>();
        _input = _scope.Resolve<Input>();
        var editorUI = _scope.Resolve<EditorUI>();
        var gum = _scope.Resolve<GumService>();
        gum.Root.AddChild(editorUI);

        var eventBus = _scope.Resolve<EventBus>();
        eventBus.RunLevel += level =>
        {
            var screenManager = _scope.Resolve<ScreenManager>();
            screenManager.ReplaceScreen(new RunScreen(services, level));
        };
    }

    public override void Dispose()
    {
        var gum = _scope.Resolve<GumService>();
        var editorUI = _scope.Resolve<EditorUI>();
        gum.Root.RemoveChild(editorUI);
        _scope.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
        _input.Update();
        LateUpdate();
    }

    private void LateUpdate()
    {
        _input.LateUpdate();
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            rasterizerState: RasterizerState.CullNone,
            transformMatrix: _screenLayout.Camera.GetViewMatrix()
        );
        _worldEditor.Draw();
        _spriteBatch.End();
    }
}