using Manhead.Core.Logic.Editor.Data;

namespace Manhead.Core.Logic.Editor;

public class EventBus
{
    public event Action<Template?>? SelectTemplate;
    public event Action<LevelBlueprint>? RunLevel;
    public event Action? ExitLevel;
    
    public void RaiseSelectTemplate(Template? template) => SelectTemplate?.Invoke(template);
    public void RaiseRunLevel(LevelBlueprint level) => RunLevel?.Invoke(level);
    public void RaiseExitLevel() => ExitLevel?.Invoke();
}