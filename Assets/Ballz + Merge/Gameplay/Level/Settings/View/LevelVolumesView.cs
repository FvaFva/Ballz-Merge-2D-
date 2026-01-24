using System.Collections.Generic;
using System.Linq;
using BallzMerge.Gameplay.Level;
using UnityEngine;
using UnityEngine.UI;

public class LevelVolumesView : MonoBehaviour, IAvailable
{
    [SerializeField] private Image _flag;
    [SerializeField] private RectTransform _volumesContent;
    [SerializeField] private GameDataVolumeMicView _viewPrefab;
    [SerializeField] private List<BackgroundUI> _backgrounds;

    private List<GameDataVolumeMicView> _views = new List<GameDataVolumeMicView>();

    public bool IsAvailable { get; private set; }

    public LevelVolumesView ApplyColors(GameColors gameColors)
    {
        foreach (var background in _backgrounds)
            background.ApplyColors(gameColors);

        return this;
    }

    public void Show(DropRarity rarity, IEnumerable<string> names)
    {
        IsAvailable = false;

        _flag.color = rarity.Color;

        foreach (var name in names)
            GetView().Show(name, rarity.Weight);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        IsAvailable = true;

        foreach (var view in _views.Where(v => v.IsAvailable == false))
            view.Hide();

        gameObject.SetActive(false);

    }

    private GameDataVolumeMicView GetView()
    {
        var view = _views.Where(v => v.IsAvailable).FirstOrDefault();

        if (view == default)
        {
            view = Instantiate(_viewPrefab, _volumesContent).Init();
            _views.Add(view);
        }

        return view;
    }
}
