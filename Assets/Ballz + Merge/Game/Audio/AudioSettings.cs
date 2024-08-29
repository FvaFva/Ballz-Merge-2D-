using System;
using UnityEngine;

public class AudioSettings
{
    private const string Name = "AudioSettings";

    public AudioSettings() 
    {
        IsActive = PlayerPrefs.GetInt(Name, 0) is 1;
    }

    public bool IsActive { get; private set; }
    public event Action Changed;

    public void Change()
    {
        IsActive = !IsActive;
        PlayerPrefs.SetInt(Name, IsActive ? 1:0);
        Changed?.Invoke();
    }
}