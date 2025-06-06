public struct SavedBlockEffect
{
    public string Name;
    public int EffectBlock;
    public int ConnectBlock;

    public SavedBlockEffect(string name, int effectBlock, int connectBlock)
    {
        Name = name;
        EffectBlock = effectBlock;
        ConnectBlock = connectBlock;
    }
}
