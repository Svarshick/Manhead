using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI;
using Manhead.Core.Logic.Gameplay;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Manhead.Core.Logic.Editor;

public class RunScreen : GameScreen
{
    private SpriteBatch _spriteBatch;
    private ScreenLayout _screenLayout;

    private LevelBlueprint _level;
    private GameLoop _gameLoop;
    private GridLayout _gridLayout;
    private GameInput _input;
    
    private EventBus _eventBus;
    private RunUI _runUI;
    
    public RunScreen(ManheadGame manheadGame, LevelBlueprint level) : base(manheadGame)
    {
        _level = level;
    }

    public override void Initialize()
    {
        _spriteBatch = Services.GetService<SpriteBatch>();
        _screenLayout = Services.GetService<ScreenLayout>();
        _gridLayout = new GridLayout(_screenLayout.ToPixels(1, 1));
        _input = new();
        _eventBus = new();
        _gameLoop = new GameLoop(_level, _input, _gridLayout);
    }

    public override void LoadContent()
    {
        _runUI = new RunUI(_eventBus);
        ManheadGame.GumService.Root.AddChild(_runUI);
    }
    
    public override void UnloadContent()
    {
        ManheadGame.GumService.Root.RemoveChild(_runUI);
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