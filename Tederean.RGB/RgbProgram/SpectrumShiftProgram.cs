using OpenRGB.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;

namespace Tederean.RGB.RgbProgram
{

  public class SpectrumShiftProgram : IRgbProgram
  {

    private const int PeriodDuration_ms = 60000;

    private const int FramesPerSecond = 24;

    private readonly int DelayTime_ms;

    private readonly int ColorCount;

    private readonly Color[] RainbowColors;

    private int m_ColorIndex;


    public SpectrumShiftProgram()
    {
      DelayTime_ms = (int)Math.Round(1000.0 / FramesPerSecond);
      ColorCount = (int)Math.Round(PeriodDuration_ms / (1000.0 / FramesPerSecond));

      RainbowColors = Color.GetHueRainbow(ColorCount).ToArray();
    }


    public async Task Run(Func<bool> isShutdown, List<IRgbDeviceHandler> deviceHandlers)
    {
      while (!isShutdown())
      {
        await using (new FixedTime(TimeSpan.FromMilliseconds(DelayTime_ms)))
        {
          var lastColor = LastColor();
          var nextColor = NextColor();

          if (lastColor != nextColor)
          {
            deviceHandlers.ForEach(deviceHandler =>
            {
              deviceHandler.SetColor(nextColor);
            });
          }
        }
      }
    }


    private Color LastColor()
    {
      var index = Math.Max(0, m_ColorIndex - 1);

      return RainbowColors[index];
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