using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, IInitializable, ILevelSaver, ILevelLoader, ILevelFinisher
    {
        private const string CountOfSavedVolumes = "CountOfVolumes";
        private const float AnimationTime = 0.8f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

        private List<BallVolumesBagCell> _dropsMap = new List<BallVolumesBagCell>();
        private Action _callback;

        public int CountOfVolumes { get; private set; }
        public IReadOnlyList<BallVolumesBagCell> DropsMap => _dropsMap;

        public event Action<BallVolumesBagCell> DropSelected;
        public event Action Opened;

        private void OnEnable()
        {
            _firstSlot.Selected += OnSelect;
            _secondSlot.Selected += OnSelect;
        }

        private void OnDisable()
        {
            _firstSlot.Selected -= OnSelect;
            _secondSlot.Selected -= OnSelect;
        }

        public void Init()
        {
            _firstSlot.CashMaterials();
            _secondSlot.CashMaterials();
        }

        public void StartLevel()
        {
            _dropsMap.Clear();
            CountOfVolumes = 0;
        }

        public IDictionary<string, object> GetSavingData()
        {
            return new Dictionary<string, object>
            {
                { CountOfSavedVolumes, _dropsMap.Count }
            };
        }

        public void Load(IDictionary<string, object> data)
        {
            CountOfVolumes = JsonConvert.DeserializeObject<int>(data[CountOfSavedVolumes].ToString());
        }

        public void Show(Drop first, Drop second, Action callback)
        {
            _firstSlot.Show(first);
            _secondSlot.Show(second);
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, AnimationTime);
            _callback = callback;
            Opened?.Invoke();
        }

        public void FinishLevel()
        {
            _canvasGroup.alpha = 0;
            _dropsMap.Clear();
            CountOfVolumes = 0;
            gameObject.SetActive(false);
        }

        public void SelectDrop(Drop drop, int? id = null)
        {
            BallVolumesBagCell newCell = new BallVolumesBagCell(drop.Volume, drop.Rarity, id);
            DropSelected?.Invoke(newCell);
            _dropsMap.Add(newCell);
        }

        private void OnSelect(Drop drop)
        {
            Hide();
            SelectDrop(drop);
        }

        private void Hide() => _canvasGroup.DOFade(0, AnimationTime).OnComplete(OnHideAnimationFinished);

        private void OnHideAnimationFinished()
        {
            _firstSlot.Show(default);
            _secondSlot.Show(default);
            _callback();
            gameObject.SetActive(false);
        }
    }
}