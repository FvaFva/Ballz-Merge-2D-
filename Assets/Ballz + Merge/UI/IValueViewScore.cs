using System;

public interface IValueViewScore
{
    public event Action<IValueViewScore, int, int> ScoreChanged;
}
