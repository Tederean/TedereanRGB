using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using OpenRGB.NET;
using Tederean.RGB.DeviceHandler;
using Tederean.RGB.RgbProgram;

namespace Tederean.RGB
{

  public static class Program
  {

    private const string OpenRgbPath = "C:\\Tederean\\Programme\\OpenRGB";

    private const int OpenRgbPort = 6742;


    public static async Task RunAsync(CancellationTokenSource cancellationTokenSourceApp)
    {
      var powerMode = PowerModes.Resume;

      while (!cancellationTokenSourceApp.IsCancellationRequested)
      {
        using (var cancellationTokenSourceSuspend = new CancellationTokenSource())
        {
          void OnPowerModeChangedInternal(object sender, PowerModeChangedEventArgs args)
          {
            if (args.Mode == PowerModes.StatusChange)
              return;

            powerMode = args.Mode;
            cancellationTokenSourceSuspend.Cancel();
          }

          SystemEvents.PowerModeChanged += OnPowerModeChangedInternal;

          try
          {
            using (var cancellationTokenSourceLinked = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSourceApp.Token, cancellationTokenSourceSuspend.Token))
            {
              if (powerMode == PowerModes.Resume)
              {
                await RunOpenRgbAsync(cancellationTokenSourceLinked);
              }

              else if (powerMode == PowerModes.Suspend)
              {
                try
                {
                  await cancellationTokenSourceLinked.Token;
                }
                catch (OperationCanceledException) { }
              }
            }
          }
          finally
          {
            SystemEvents.PowerModeChanged -= OnPowerModeChangedInternal;
          }
        }
      }
    }

    private static async Task RunOpenRgbAsync(CancellationTokenSource cancellationTokenSource)
    {
      var processStartInfo = new ProcessStartInfo()
      {
        WorkingDirectory = OpenRgbPath,
        FileName = $"{OpenRgbPath}\\OpenRGB.exe",
        Arguments = $"--noautoconnect --server --server-port {OpenRgbPort} --localconfig",
        UseShellExecute = true,
        CreateNoWindow = false,
        WindowStyle = ProcessWindowStyle.Normal,
      };


      using (var openRGBServer = Process.Start(processStartInfo))
      {
        if (openRGBServer != null)
        {
          try
          {
            await OpenRgbServerAvailableAsync(cancellationTokenSource.Token, OpenRgbPort);


            if (!cancellationTokenSource.IsCancellationRequested)
            {
              await Task.Run(async () =>
              {
                using (var client = new OpenRgbClient(port: OpenRgbPort))
                {
                  var deviceHandlers = RgbDeviceHandler.GetDeviceHandlers(client).ToList();

                  deviceHandlers.ForEach(deviceHandler => deviceHandler.ApplyMode());

                  var rgbProgram = new SpectrumShiftProgram();

                  await rgbProgram.RunAsync(cancellationTokenSource.Token, deviceHandlers);
                }
              });
            }
          }
          finally
          {
            openRGBServer.Kill();
            await openRGBServer.WaitForExitAsync();
          }
        }
      }
    }

    private static async Task OpenRgbServerAvailableAsync(CancellationToken token, int port)
    {
      while (!token.IsCancellationRequested)
      {
        var tcpEndPoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
        var serverAvailable = tcpEndPoints.Any(tcpEndPoint => tcpEndPoint.Port == port);

        try
        {
          await Task.Delay(500, token);
        }
        catch (TaskCanceledException) { }


        if (serverAvailable)
        {
          try
          {
            await Task.Delay(5000, token); // OpenRGB needs additional time even then TCP API is already available.
          }
          catch (TaskCanceledException) { }

          return;
        }
      }
    }
  }
}