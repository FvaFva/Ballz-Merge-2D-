namespace BallzMerge.Root
{
    public interface IGameTimeOwner
    {
        public void SetRegular();
        public void Stop();
        public void SpeedUp();
        public void PlaySlowMo(float time);
    }
}
