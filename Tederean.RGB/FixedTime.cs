using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tederean.RGB
{

  public class FixedTime : IAsyncDisposable
  {

    private readonly TimeSpan Start;

    private readonly TimeSpan Period;

    private readonly CancellationToken CancellationToken;


    public FixedTime(TimeSpan period, CancellationToken cancellationToken)
    {
      Start = DateTimeOffset.UtcNow.Offset;
      Period = period;
      CancellationToken = cancellationToken;
    }


    public async ValueTask DisposeAsync()
    {
      if (CancellationToken.IsCancellationRequested)
        return;

      var end = DateTimeOffset.UtcNow.Offset;
      var elapsedTime = end - Start;

      try
      {
        await Task.Delay(Period - elapsedTime, CancellationToken);
      }
      catch (TaskCanceledException) { }
    }
  }
}
