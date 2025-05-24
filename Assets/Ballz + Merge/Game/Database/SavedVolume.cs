public struct SavedVolume
{
    public int ID;
    public string Name;
    public string Species;
    public int Weight;

    public SavedVolume(int id, string name, string species, int weight)
    {
        ID = id;
        Name = name;
        Species = species;
        Weight = weight;
    }
}
