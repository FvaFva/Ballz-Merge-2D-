using UnityEngine;

public class LevelSettings : ScriptableObject , ISerializationCallbackReceiver
{
    [SerializeField] private int _iD;
    [SerializeField] private string _title;
    [SerializeField] private BlocksSettings _blocksSettings;
    [SerializeField] private DropSettings _dropSettings;

    public BlocksSettings BlocksSettings => _blocksSettings;
    public DropSettings DropSettings => _dropSettings;
    public string Title => _title;
    public int ID => _iD;

    public LevelSettings()
    {
        _blocksSettings = new BlocksSettings();
        _dropSettings = new DropSettings();
    }

    public void Copy(BlocksSettings blocks, DropSettings drop)
    {
        _blocksSettings = blocks;
        _dropSettings = drop;
    }

    public void OnBeforeSerialize()
    {
        _dropSettings?.OnBeforeSerialize();
    }

    public void OnAfterDeserialize()
    {
        _dropSettings.OnAfterDeserialize();
        _blocksSettings.RebuildColorMap();
    }
}
