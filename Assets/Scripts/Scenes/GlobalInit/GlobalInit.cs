using VContainer.Unity;

namespace Scenes.GlobalInit
{
    public class GlobalInit : IStartable
    {
        private readonly SceneLoader _sceneLoader;
        
        public GlobalInit(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public void Start()
        {
            //_sceneLoader.LoadSceneAsyncUniTask(SceneNames.MainMenu);
        }
    }
}