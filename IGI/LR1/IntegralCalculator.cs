using System.Diagnostics;

namespace ClassLibrary;

public class IntegralCalculator
{
    private const int MaxParallelCountDefault = 3;
    private static SemaphoreSlim _semaphore = new(MaxParallelCountDefault, MaxParallelCountDefault);

    public void SetMaxParallelCount(int maxParallelCount)
    {
        _semaphore = new(maxParallelCount, maxParallelCount);
    }
    
    public event Action<double>? ProgressChanged;
    public event Action<double, TimeSpan>? CalculationCompleted;

    private double _trashCalculate(int iterations = 100_000)
    {
        double res = 12.4;
        for (int i = 0; i < iterations; ++i)
        {
            res *= 1.001;
        }

        return res;
    }
    
    public void CalculateSin(double from = 0.0, double to = 1.0, double step = 0.0001)
    {
        try
        {
            _semaphore.Wait();
            Stopwatch sw = Stopwatch.StartNew();
            double result = 0.0;

            var totalSteps = (int)((to - from) / step);
            var progressOut = (int)((100_000) * step);

            for (int i = 0; i < totalSteps; ++i)
            {
                double x = from + i * step;
                _trashCalculate();

                result += Math.Sin(x) * step;
                if (i % progressOut == 0) ProgressChanged?.Invoke((double)i / totalSteps * 100);
            }

            sw.Stop();
            CalculationCompleted?.Invoke(result, sw.Elapsed);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}