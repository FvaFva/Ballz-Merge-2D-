using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New rules list", menuName = "Bellz+Merge/Game/Rules", order = 51)]
public class GameRulesList : ScriptableObject
{
    [SerializeField] private List<string> _rules;
    [SerializeField] private List<GameRule> _rulesList;

    public IReadOnlyList<GameRule> Rules => _rulesList;

    public string Get() => _rules[Random.Range(0, _rules.Count)];
}
