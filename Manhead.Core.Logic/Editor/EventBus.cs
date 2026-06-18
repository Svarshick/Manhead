using Manhead.Core.Logic.Editor.Data;

namespace Manhead.Core.Logic.Editor;

public class EventBus
{
    public event Action<Template?>? TemplateSelected;
    public event Action<LevelBlueprint>? RunLevel;
    
    public void SelectTemplate(Template? template) => TemplateSelected?.Invoke(template);
    public void LevelToRun(LevelBlueprint level) => RunLevel?.Invoke(level);
}