using Data;
using UnityEngine;
using VContainer.Unity;

namespace Scenes.GameStarter
{
    public class GameStarter : IStartable
    {
        private readonly GameSave _save;
        public GameStarter(GameSave save)
        {
            _save = save;
        }

        public void Start()
        {
            Debug.Log($"Game save {_save.Directory.Directory.Name} loaded!");
        }
    }
}