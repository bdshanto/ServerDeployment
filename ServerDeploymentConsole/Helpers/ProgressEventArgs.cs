namespace ServerDeployment.Console.Helpers;

public class ProgressEventArgs : EventArgs
{
    public string Message { get; set; }
    public int? Percent { get; set; }

    public ProgressEventArgs(string message, int? percent = null)
    {
        Message = message;
        Percent = percent;
    }
}