using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    [CreateAssetMenu(fileName = "New block additional effect settings", menuName = "Bellz+Merge/Block/AdditionalBlockEffectSettings", order = 51)]
    public class BlockAdditionalEffectSettings : ScriptableObject
    {
        private const int TargetChance = 1;
        private const int DecimalPlaces = 2;

        [SerializeField] private BlockAdditionalEffectProperty[] _properties;

        public IEnumerable<BlockAdditionalEffectProperty> Properties => _properties;

        private void OnValidate()
        {
            if (_properties == null || _properties.Length == 0)
                return;

            // Суммируем все элементы массива
            float sum = 0f;
            foreach (var value in _properties)
            {
                sum += value.ChanceToPerform;
            }

            // Проверяем, что сумма не равна нулю, чтобы избежать деления на ноль
            if (sum == 0f)
                return;

            // Масштабируем каждый элемент так, чтобы сумма была равна targetSum
            float scale = TargetChance / sum;

            for (int i = 0; i < _properties.Length; i++)
            {
                _properties[i].ChanceToPerform *= scale;
                _properties[i].ChanceToPerform = (float)System.Math.Round(_properties[i].ChanceToPerform, DecimalPlaces);
            }

            // Проверяем результат
            float newSum = 0f;
            foreach (var value in _properties)
            {
                newSum += value.ChanceToPerform;
            }
        }

        public BlockAdditionalEffectBase GetPrefab()
        {
            int normalizationFactor = 100;
            // Генерируем случайное число от 0 до общей суммы весов
            float randomValue = Random.Range(0, TargetChance * normalizationFactor);
            float sum = 0;

            // Выбираем объект на основе случайного числа
            foreach (var property in _properties)
            {
                sum += property.ChanceToPerform * normalizationFactor;

                if (randomValue < sum)
                    return property.Prefab;
            }

            // На случай ошибки, возвращаем первый объект
            return _properties[0].Prefab;
        }
    }
}
