using OpenRGB.NET;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;
using Tederean.RGB.RgbProgram;

namespace Tederean.RGB
{

  public static class Program
  {

    private static bool m_IsShutdown;


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
      
      AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
      {
        m_IsShutdown = true;
      };
		
      if (OperatingSystem.IsWindows())
      {
        await Task.Delay(8000);
      }
      else
      {
        await Task.Delay(4000);
      }

      await RgbClientLoop();
    }
		
    private static async Task RgbClientLoop()
    {
      while (!m_IsShutdown)
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

              await rgbProgram.Run(() => m_IsShutdown, deviceHandlers);
            }
            finally
            {
              if (m_IsShutdown)
              {
                deviceHandlers.ForEach(deviceHandler => deviceHandler.Shutdown());
              }
            }
          }
        }
        catch (Exception exception)
        {
          Console.Error.WriteLine(exception.ToString());

          if (!m_IsShutdown)
          {
            await Task.Delay(1000);
          }
        }
      }
    }
  }
}