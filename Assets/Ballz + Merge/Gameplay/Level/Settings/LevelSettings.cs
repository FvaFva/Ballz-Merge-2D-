using UnityEngine;

public class LevelSettings : ScriptableObject , ISerializationCallbackReceiver
{
    [SerializeField] private string _title;
    [SerializeField] private BlocksSettings _blocksSettings;
    [SerializeField] private DropSettings _dropSettings;

    [SerializeField] private int _iD;

    public BlocksSettings BlocksSettings => _blocksSettings;
    public DropSettings DropSettings => _dropSettings;
    public string Title => _title;
    public int ID => _iD;

    public LevelSettings()
    {
        _blocksSettings = new BlocksSettings();
        _dropSettings = new DropSettings();
    }

    private void OnValidate()
    {
        _blocksSettings.RebuildColorMap();
    }

    public void OnBeforeSerialize()
    {
        _dropSettings?.OnBeforeSerialize();
    }

    public void OnAfterDeserialize()
    {
        _dropSettings.OnAfterDeserialize();
    }
}
