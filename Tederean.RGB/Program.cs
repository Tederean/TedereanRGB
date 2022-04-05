using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tederean.RGB
{

  public static class Program
  {

    public static async Task Main()
    {
#if DEBUG
      if (!Debugger.IsAttached)
      {
        while (!Debugger.IsAttached)
        {
          await Task.Delay(100);
        }

        Debugger.Break();
      }
#endif


      var cancellationTokenSource = new CancellationTokenSource();

      AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
      {
        cancellationTokenSource.Cancel();
      };


      try
      {
        if (OperatingSystem.IsWindows())
        {
          WindowUtil.SetWindowVisibility(isVisible: false);
        }

        await LoopRoutine.RunAsync(cancellationTokenSource.Token);
      }
      catch (Exception) when (cancellationTokenSource.IsCancellationRequested) { }
    }
  }
}