using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    [CreateAssetMenu(fileName = "New block additional effect settings", menuName = "Bellz+Merge/Block/AdditionalBlockEffectSettings", order = 51)]
    public class BlockAdditionalEffectSettings : ScriptableObject
    {
        private const int TargetChance = 1;
        private const int DecimalPlaces = 2;
        private const int NormalizationFactor = 100;

        [SerializeField] private BlockAdditionalEffectProperty[] _properties;
        [SerializeField, Range(0, 1)] private float _spawnChance;

        public BlockAdditionalEffectProperty[] Properties => _properties;

        private void OnValidate()
        {
            _spawnChance = (float)System.Math.Round(_spawnChance, DecimalPlaces);

            if (_properties == null || _properties.Length == 0)
                return;

            // Суммируем все элементы массива
            float sum = 0f;

            foreach (var value in _properties)
                sum += value.ChanceToPerform;

            // Проверяем, что сумма не равна нулю, чтобы избежать деления на ноль
            if (sum == 0f)
                return;

            // Масштабируем каждый элемент так, чтобы сумма была равна targetSum
            float scale = TargetChance / sum;

            for (int i = 0; i < _properties.Length; i++)
                _properties[i].ChanceToPerform = (float)System.Math.Round(_properties[i].ChanceToPerform * scale, DecimalPlaces);

            // Проверяем результат
            float newSum = 0f;

            foreach (var value in _properties)
                newSum += value.ChanceToPerform;
        }

        public BlockAdditionalEffectBase GetPrefab()
        {
            // Генерируем случайное число от 0 до общей суммы весов
            float randomValue = Random.Range(0, TargetChance * NormalizationFactor);
            float sum = 0;

            // Выбираем объект на основе случайного числа
            foreach (var property in _properties)
            {
                sum += property.ChanceToPerform * NormalizationFactor;

                if (randomValue < sum)
                    return property.Prefab;
            }

            // На случай ошибки, возвращаем первый объект
            return _properties[0].Prefab;
        }

        public bool ChanceToGetPrefab()
        {
            float chanceToSpawn = Random.Range(0, TargetChance * NormalizationFactor);

            if (chanceToSpawn < _spawnChance * NormalizationFactor)
                return true;

            return false;
        }
    }
}
