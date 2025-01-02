using UnityEngine;

public class BallVolumeCageElement : MonoBehaviour
{
    [SerializeField] private GameDataVolumeMicView _view;

    public BallVolumesBagCell Current {  get; private set; }
    public bool IsFree { get; private set; }

    public BallVolumeCageElement Apply(BallVolumesBagCell volume)
    {
        if (Current.IsEqual(volume))
            return this;

        Current = volume;
        _view.Show(Current.Volume, Current.Value);
        return this;
    }

    public void Hide(bool isClearing)
    {
        if(isClearing)
        {
            IsFree = true;
            Current = default;
        }

        _view.Hide();
        gameObject.SetActive(false);
    }

    public BallVolumeCageElement Activate()
    {
        if (IsFree == false)
            _view.Show(Current.Volume, Current.Value);
        else
            IsFree = false;
        
        gameObject.SetActive(true);
        return this;
    }
}