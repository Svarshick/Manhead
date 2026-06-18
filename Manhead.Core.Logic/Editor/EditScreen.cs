using Manhead.Core.Logic.Editor.Data;
using Manhead.Core.Logic.Editor.UI;
using Manhead.Core.Logic.WorldSpace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Manhead.Core.Logic.Editor;

public class EditScreen : GameScreen
{
    private SpriteBatch _spriteBatch;
    private ScreenLayout _screenLayout;
    
    private Input _input;
    private EventBus _eventBus;
    private GridLayout _gridLayout;
    private TemplateHolder _templateHolder;
     
    private WorldEditor _worldEditor;
    private EditorUI _editorUI;
   
    public EditScreen(Game game) : base(game)
    {
    }

    public override void Initialize()
    {
        _spriteBatch = Services.GetService<SpriteBatch>();
        _screenLayout = Services.GetService<ScreenLayout>();
        
        _input = new Input();
        _eventBus = new();
        _gridLayout = new GridLayout(_screenLayout.ToPixels(1, 1));
       _templateHolder = new (_gridLayout, 100, 100);
       
        _eventBus.RunLevel += level => ((Game)Game).ScreenManager.ReplaceScreen(new RunScreen((Game)Game, level));
    }

    public override void LoadContent()
    {
        _worldEditor = new WorldEditor(_input.Editor, _eventBus, _templateHolder, _gridLayout, GraphicsDevice);
        _editorUI = new EditorUI(_templateHolder, _eventBus);
    }

    public override void UnloadContent()
    {
        _worldEditor.Dispose();
        _editorUI.Dispose();
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