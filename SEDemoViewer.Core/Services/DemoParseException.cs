namespace SEDemoViewer.Core.Services;

[Serializable]
public class DemoParseException : Exception
{
    public  DemoParseException(string message) : base(message) { }
    public DemoParseException(string message, Exception inner) : base(message, inner) { }
}