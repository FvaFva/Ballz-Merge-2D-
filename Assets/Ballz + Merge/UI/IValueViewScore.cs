using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueViewScore
{
    public event Action<IValueViewScore, int, int> ScoreChanged;
}
