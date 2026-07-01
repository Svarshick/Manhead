using Gum.Forms;
using Gum.Wireframe;
using MonoGame.Extended.Input;

namespace Manhead.Core.Logic;

public enum InputOwner
{
    None,
    UI,
    Game
}

public class Input : IUpdatable
{
    public InputOwner InputOwner { get; private set; } = InputOwner.None;
    
    private IUpdatable _gameInput;
    public readonly Editor.EditorInput Editor = new();
    
    private readonly ICursor _defaultCursor;
    private readonly ICursor _disabledCursor;
    
    public Input()
    {
        _gameInput = Editor;
        _defaultCursor = ManheadGame.GumService.Cursor;
        _disabledCursor = new DisabledCursor();
    }

    public void Update()
    {
        var hasInput = MouseExtended.GetState().DeltaScrollWheelValue != 0 ||
                       MouseExtended.GetState().IsAnyButtonDown() ||
                       KeyboardExtended.GetState().IsAnyButtonDown();

        if (InputOwner == InputOwner.None && hasInput)
        {
            var uiInput = ManheadGame.GumService.Cursor.VisualOver is { } visual
                          && visual != ManheadGame.Background;
            InputOwner = uiInput ? InputOwner.UI : InputOwner.Game;
        }

        if (InputOwner != InputOwner.None && !hasInput)
        {
            InputOwner = InputOwner.None;
        }

        switch (InputOwner)
        {
            case InputOwner.UI:
                FormsUtilities.SetCursor(_defaultCursor);
                MouseExtended.Update();
                KeyboardExtended.Update();
                break;
            case InputOwner.Game:
                FormsUtilities.SetCursor(_disabledCursor);
                _gameInput.Update();
                break;
            case InputOwner.None:
                FormsUtilities.SetCursor(_defaultCursor);
                _gameInput.Update();
                break;
        }
    }

    public void LateUpdate()
    {
        if (InputOwner is InputOwner.Game or InputOwner.None)
        {
            _gameInput.LateUpdate();
        }
    }
}

public class DisabledCursor : ICursor
{
    public Cursors? CustomCursor { get; set; }
    public InputDevice LastInputDevice => InputDevice.Mouse;
    public int X => -1000;
    public int Y => -1000;

    public double LastPrimaryPushTime => -1000;
    public double LastPrimaryClickTime => -1000;

    public int XChange => 0;
    public int YChange => 0;

    public int ScrollWheelChange => 0;
    public float ZVelocity => 0;

    public bool PrimaryPush => false;
    public bool PrimaryDown => false;
    public bool PrimaryClick => false;
    public bool PrimaryClickNoSlide => false;
    public bool PrimaryDoubleClick => false;
    public bool PrimaryDoublePush => false;

    public bool SecondaryPush => false;
    public bool SecondaryDown => false;
    public bool SecondaryClick => false;
    public bool SecondaryDoubleClick => false;

    public bool MiddlePush => false;
    public bool MiddleDown => false;
    public bool MiddleClick => false;
    public bool MiddleDoubleClick => false;

    public InteractiveGue WindowPushed { get; set; }
    public InteractiveGue VisualRightPushed { get; set; }
    public InteractiveGue WindowOver { get; set; }
    public InteractiveGue? VisualOver { get; set; }

    public void Activity(double currentGameTimeTotalSeconds){}

    public float XRespectingGumZoomAndBounds() => -1000;

    public float YRespectingGumZoomAndBounds() => -1000;
}