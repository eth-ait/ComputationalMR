namespace Assets.Scripts.Model
{
    public enum MethodType
    {
        Manual,
        Automatic,
    }

    public enum PrimaryTaskType
    {
        High,
        Mid,
        Low
    }

    public enum SecondaryTaskType
    {
        Email,
        Ideation,
        Break
    }

    public enum ConditionPosition
    {
        Left,
        Right,
        Center
    }

    public class ConditionModelTemplate
    {
        public MethodType Method;
        public PrimaryTaskType PrimaryTask;
        public SecondaryTaskType SecondaryTask;
        public ConditionPosition Position;
    }
}