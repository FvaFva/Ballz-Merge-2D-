namespace BallzMerge.Achievement
{
    public struct AchievementPointsStep
    {
        public int Points;
        public int Step;

        public AchievementPointsStep(object points, object step)
        {
            Points = 0;
            Step = 0;
            Points = ParseObjectToInt(points);
            Step = ParseObjectToInt(step);
        }

        public bool IsNewerThan(AchievementPointsStep another)
        {
            return Step > another.Step || (Step == another.Step && Points > another.Points);
        }

        private int ParseObjectToInt(object input)
        {
            if (int.TryParse(input.ToString(), out int result) == false)
                return 0;
            return result;
        }
    }
}