namespace ClassLibrary;

public class ProgressPrinter
{
    private Dictionary<int, int> _threadLines = new();
    private int _freeLine = 0;
    private static readonly object ProgressLock = new();
    
    private string _genProgressLine(double progress)
    {
        var intProgress = (int)progress;
        string past = intProgress > 0 ? new string('=', intProgress -1) : "";
        string last = intProgress > 0 ? new string(' ', 100 - intProgress) : new string(' ', 99);
        return $"[{past}>{last}] {progress:F2}%";
    }

    public void SetProgress(double progress, string message = "")
    {
        var threadNum = Thread.CurrentThread.ManagedThreadId;

        lock (ProgressLock)
        {
            int line;

            if (!_threadLines.TryGetValue(threadNum, out line))
            {
                line = _freeLine;
                _threadLines[threadNum] = line;
                ++_freeLine;
                Console.WriteLine();
            }
            
            Console.SetCursorPosition(0, line);
            Console.Write($"Поток {threadNum}: {_genProgressLine(progress)} {message}");
        }
    }
}