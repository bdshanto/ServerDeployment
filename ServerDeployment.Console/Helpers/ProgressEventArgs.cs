using ServerDeployment.Console.Helpers;

public class ProgressEventArgs : EventArgs
{
    public string Message { get; set; }
    public int? Percent { get; set; }
    public ProgressType ProgressFor { get; set; }

    public ProgressEventArgs(string message, int? percent = null, ProgressType progressFor = ProgressType.Backup)
    {
        Message = message;
        Percent = percent;
        ProgressFor = progressFor;
    }
}