using OpenRGB.NET;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;
using Tederean.RGB.RgbProgram;

namespace Tederean.RGB
{

  public static class LoopRoutine
  {

    public static async Task RunAsync(CancellationToken cancellationToken)
    {
      // Wait for OpenRGB server to boot - Windows needs more time...

      await Task.Delay(OperatingSystem.IsWindows() ? 8000 : 4000, cancellationToken);

      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          using (var client = new OpenRGBClient())
          {
            var deviceHandlers = RgbDeviceHandler.GetDeviceHandlers(client);

            deviceHandlers.ForEach(deviceHandler => deviceHandler.Initialize());

            try
            {
              var rgbProgram = new SpectrumShiftProgram();

              await rgbProgram.RunAsync(cancellationToken, deviceHandlers);
            }
            finally
            {
              if (cancellationToken.IsCancellationRequested)
              {
                deviceHandlers.ForEach(deviceHandler => deviceHandler.Shutdown());
              }
            }
          }
        }
        catch (Exception exception)
        {
          Console.Error.WriteLine(exception.ToString());

          if (!cancellationToken.IsCancellationRequested)
          {
            await Task.Delay(1000, cancellationToken);
          }
        }
      }
    }
  }
}
