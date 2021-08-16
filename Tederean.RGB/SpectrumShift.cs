using OpenRGB.NET;
using OpenRGB.NET.Models;
using System;
using System.Linq;
using System.Threading;

namespace Tederean.RGB
{

  public class SpectrumShift
  {

    private const int PeriodDuration_ms = 60000;

    private const int FramesPerSecond = 24;

    private const string ModeDirect = "Direct";

    private readonly int DelayTime_ms;

    private readonly int ColorCount;

    private readonly Color[] RainbowColors;

    private readonly string IPAddress;

    private int m_ColorIndex;


    public SpectrumShift(string ip)
    {
      DelayTime_ms = (int)Math.Round(1000.0 / FramesPerSecond);
      ColorCount = (int)Math.Round(PeriodDuration_ms / (1000.0 / FramesPerSecond));

      IPAddress = ip;
      RainbowColors = Color.GetHueRainbow(ColorCount).ToArray();
    }


    public void Run()
    {
      using (var client = new OpenRGBClient(ip: IPAddress))
      {
        var devices = client.GetAllControllerData().Where(device => device.Modes.Any(mode => mode.Name == ModeDirect)).Select((device, deviceIndex) =>
        {

          return new RGBDevice()
          {
            DeviceId = deviceIndex,
            LEDsCount = device.Leds.Length,
            ModeId = device.Modes.Where(mode => mode.Name == ModeDirect).Select((mode, modeIndex) => modeIndex).First(),
          };

        }).ToList();


        devices.ForEach(device => client.SetMode(device.DeviceId, device.ModeId));


        while (client.Connected)
        {
          var nextColor = NextColor();

          devices.ForEach(device =>
          {
            var nextColors = Enumerable.Range(0, device.LEDsCount).Select(e => nextColor).ToArray();

            client.UpdateLeds(device.DeviceId, nextColors);
          });

          Thread.Sleep(DelayTime_ms);
        }
      }
    }

    private Color NextColor()
    {
      if (m_ColorIndex >= RainbowColors.Length)
      {
        m_ColorIndex = 0;
      }

      var returnColor = RainbowColors[m_ColorIndex];

      m_ColorIndex++;

      return returnColor;
    }
  }
}