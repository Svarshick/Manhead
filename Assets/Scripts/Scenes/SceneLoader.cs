using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Scenes
{
    public enum SceneLoaderType
    {
        Internal,
        External
    }
    
    public class SceneLoader
    {
        private readonly LifetimeScope _parent;

        public SceneLoader(LifetimeScope scope)
        {
            _parent = scope;
        }

        public async UniTask LoadSceneAsyncUniTask(string sceneName)
        {
            using (LifetimeScope.EnqueueParent(_parent))
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        public async UniTask LoadSceneAsyncUniTask(string sceneName, Action<IContainerBuilder> installing)
        {
            using (LifetimeScope.EnqueueParent(_parent))
            using (LifetimeScope.Enqueue(installing))
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }
        
        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadScene(sceneName);   
        }
    }
}