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
      try
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


        using (var cancellationTokenSource = new CancellationTokenSource())
        {
          void OnProcessExitInternal(object? sender, EventArgs args)
          {
            cancellationTokenSource.Cancel();
          }

          void OnCancelKeyPressInternal(object? sender, ConsoleCancelEventArgs e)
          {
            cancellationTokenSource.Cancel();
          }


          AppDomain.CurrentDomain.ProcessExit += OnProcessExitInternal;
          Console.CancelKeyPress += OnCancelKeyPressInternal;

          try
          {
            if (OperatingSystem.IsWindows())
            {
              WindowUtil.SetWindowVisibility(isVisible: false);
            }

            await LoopRoutine.RunAsync(cancellationTokenSource.Token);
          }
          finally
          {
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExitInternal;
            Console.CancelKeyPress -= OnCancelKeyPressInternal;
          }
        }
      }
      catch (Exception ex)
      {
        if (OperatingSystem.IsWindows())
        {
          WindowUtil.SetWindowVisibility(isVisible: true);
        }

        Console.WriteLine(ex.ToString());
        Console.ReadKey();
      }
    }
  }
}