using BallzMerge.Data;
using BallzMerge.Gameplay.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSaves : CyclicBehavior, IInitializable
{
    [SerializeField] private List<GameSaverProperty> _gameSavers;
    [SerializeField] private GameCycler _gameCycler;

    [Inject] private GridSettings _gridSettings;
    [Inject] private DataBaseSource _db;

    public void Init()
    {
        foreach (GameSaverProperty gameSaver in _gameSavers)
            gameSaver.LevelSaver.Saved += Save;

        foreach (GameSaverProperty gameSaver in _gameSavers)
            gameSaver.LevelSaver.Requested += LoadSave;

        _gameCycler.SaveGame += SaveGame;
        _gameCycler.LoadSave += Load;
        _gameCycler.DropSave += DropSave;
    }

    public void SaveGame()
    {
        _db.Saves.Save();
    }

    private void Save(string key, float value)
    {
        _db.Saves.TemporarySave(key, value);
    }

    private void Load()
    {
        foreach (GameSaverProperty saverProperty in _gameSavers)
            saverProperty.LevelSaver.Request();
    }

    private void LoadSave(ILevelSaver loader, string key)
    {
        loader.Load(key, _db.Saves.Get(key));
    }

    private void DropSave()
    {
        foreach (GameSaverProperty saverProperty in _gameSavers)
            saverProperty.LevelSaver.Restore();

        SaveGame();
    }
}