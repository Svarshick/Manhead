using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Scenes;
using VContainer;

namespace PlayerSpace.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        private readonly SceneLoader _externalSceneLoader;
        private readonly int _notDisposableCell = -1;
        private readonly SaveStorage _storage;

        public MainMenuViewModel(
            SaveStorage storage,
            [Key(SceneLoaderType.External)] SceneLoader externalSceneLoader)
        {
            _storage = storage;
            _externalSceneLoader = externalSceneLoader;
            SavesInitialization();
        }

        public List<GameSave> Cells { get; } = new();

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
            for (var i = 0; i < Cells.Count; ++i)
                if (i != _notDisposableCell)
                    Cells[i]?.Dispose();
        }
    }
}