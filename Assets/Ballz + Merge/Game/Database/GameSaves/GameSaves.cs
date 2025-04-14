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

    private GameSaverProperty _currentSaver;

    public void Init()
    {
        foreach (GameSaverProperty gameSaver in _gameSavers)
            gameSaver.LevelSaver.Saved += SaveLevel;

        _gameCycler.SaveGame += SaveGame;
        _gameCycler.LoadSave += LoadSaves;
        _gameCycler.DropSave += DropSave;
    }

    public void SaveGame()
    {
        _db.Saves.Save();
    }

    private void SaveLevel(string key, float value)
    {
        _db.Saves.TemporarySave(key, value);
    }

    private void LoadSaves()
    {
        foreach (GameSaverProperty saverProperty in _gameSavers)
        {
            _currentSaver = saverProperty;
            _currentSaver.LevelSaver.Requested += GetKey;
            _currentSaver.LevelSaver.RequestLoad();
            _currentSaver.LevelSaver.Requested -= GetKey;
        }
    }

    private void LoadSave(GameSaverProperty saverProperty, string key, float value)
    {
        saverProperty.LevelSaver.Load(key, value);
    }

    private void GetKey(string key)
    {
        LoadSave(_currentSaver, key, GetSave(key));
    }

    private float GetSave(string key)
    {
        return _db.Saves.Get(key);
    }

    private void DropSave()
    {
        foreach (GameSaverProperty saverProperty in _gameSavers)
            saverProperty.LevelSaver.Restore();

        SaveGame();
    }
}