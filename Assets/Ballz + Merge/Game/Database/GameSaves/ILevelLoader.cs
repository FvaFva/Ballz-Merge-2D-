using System.Collections.Generic;

public interface ILevelLoader
{
    public void StartLevel()
    {

    }

    public void Load()
    {
        StartLevel();
    }
}