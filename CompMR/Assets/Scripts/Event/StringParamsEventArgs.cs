namespace Assets.Scripts.Event
{
    public class StringParamsEventArgs
    {
        public string[] Parameters;

        public StringParamsEventArgs(params string[] parameters)
        {
            Parameters = parameters;
        }
    }
}