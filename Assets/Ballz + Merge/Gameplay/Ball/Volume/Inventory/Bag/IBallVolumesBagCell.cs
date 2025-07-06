public interface IBallVolumesBagCell<out T> : IBallVolumeViewData where T : BallVolume
{
    new T Volume { get; }
}
