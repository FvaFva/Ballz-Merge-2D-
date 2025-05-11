public struct SavedEffect
{
    public int ID;
    public int CurrentBlock;
    public int ConnectBlock;

    public SavedEffect(int id,int currentBlock, int connectBlock)
    {
        ID = id;
        CurrentBlock = currentBlock;
        ConnectBlock = connectBlock;
    }
}
