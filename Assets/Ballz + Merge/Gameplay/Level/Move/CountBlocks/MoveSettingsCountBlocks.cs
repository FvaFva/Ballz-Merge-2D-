using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New move numberSettings", menuName = "Bellz+Merge/Move/CountBlocks", order = 51)]
public class MoveSettingsCountBlocks : ScriptableObject
{
    private const int TargetChance = 1;
    private const int DecimalPlaces = 2;

    [SerializeField] private MoveSettingsCountBlocksProperties[] _properties;

    public IEnumerable<MoveSettingsCountBlocksProperties> Properties => _properties;

    private void OnValidate()
    {
        if (_properties == null || _properties.Length == 0)
            return;

        // Суммируем все элементы массива
        float sum = 0f;

        foreach (MoveSettingsCountBlocksProperties property in _properties)
        {
            sum = property.BlocksProperties.Sum(property => property.Chance);

            // Проверяем, что сумма не равна нулю, чтобы избежать деления на ноль
            if (sum == 0f)
                return;

            // Масштабируем каждый элемент так, чтобы сумма была равна targetSum
            float scale = TargetChance / sum;

            for (int i = 0; i < property.BlocksProperties.Length; i++)
                property.BlocksProperties[i].Chance = (float)System.Math.Round(property.BlocksProperties[i].Chance * scale, DecimalPlaces);
        }
    }
}