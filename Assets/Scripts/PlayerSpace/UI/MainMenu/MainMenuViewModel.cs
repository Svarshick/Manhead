using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Scenes;
using VContainer;

namespace PlayerSpace.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        private readonly SaveStorage _storage;
        private readonly SceneLoader _externalSceneLoader;

        public List<GameSave> Cells { get; private set; } = new();
        private int _notDisposableCell = -1;
        
        public MainMenuViewModel(
            SaveStorage storage,
            [Key(SceneLoaderType.External)] SceneLoader externalSceneLoader)
        {
            _storage = storage;
            _externalSceneLoader = externalSceneLoader;
            SavesInitialization();
        }

        private void SavesInitialization()
        {
            Cells.Add(_storage.OpenOrCreateGameSave(SaveFiles.Cell1Dir, false));
            Cells.Add(_storage.OpenOrCreateGameSave(SaveFiles.Cell2Dir, false));
            Cells.Add(_storage.OpenOrCreateGameSave(SaveFiles.Cell3Dir, false));
        }

        public void LoadGame(GameSave save)
        {
            _externalSceneLoader.LoadSceneAsyncUniTask(
                SceneNames.GameStarter, 
                builder => builder.RegisterInstance(save))
                .Forget();
            _externalSceneLoader.UnloadScene(SceneNames.MainMenu);
        }

        public override void Dispose()
        {
            base.Dispose();
            for (int i = 0; i < Cells.Count; ++i)
                if (i != _notDisposableCell)
                    Cells[i]?.Dispose();
        }
    }
}