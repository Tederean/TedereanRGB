﻿using CoordinateSharp;
using OpenRGB.NET;
using OpenRGB.NET.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;

namespace Tederean.RGB.RgbProgram
{

  public class SpectrumShiftProgram : IRgbProgram
  {

    private const int PeriodDuration_ms = 60000;

    private const int FramesPerSecond = 24;

    private const double Latitude = 51.79;

    private const double Logitude = 6.17;

    private readonly TimeSpan DelayTime;

    private readonly int ColorCount;

    private readonly Color[] RainbowColors;


    private Color? m_LastLedColor;

    private int m_RainbowColorIndex;


    public SpectrumShiftProgram()
    {
      DelayTime = TimeSpan.FromMilliseconds(1000.0 / FramesPerSecond);
      ColorCount = (int)Math.Round(PeriodDuration_ms / (1000.0 / FramesPerSecond));

      RainbowColors = ColorUtils.GetHueRainbow(ColorCount).ToArray();
    }


    public async Task RunAsync(CancellationToken cancellationToken, List<IRgbDeviceHandler> deviceHandlers)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        await using (new FixedTime(DelayTime, cancellationToken))
        {
          var nextRainbowColor = NextRainbowColor();
          var nextLedColor = ApplyBrightnessCorrection(nextRainbowColor);


          if (nextLedColor != m_LastLedColor)
          {
            m_LastLedColor = nextLedColor;

            deviceHandlers.ForEach(deviceHandler =>
            {
              deviceHandler.SetColor(nextLedColor);
            });
          }
        }
      }
    }



    private Color ApplyBrightnessCorrection(Color color)
    {
      var sunAltitude = Celestial.CalculateCelestialTimes(Latitude, Logitude, DateTime.UtcNow).SunAltitude;
      var brightnessRatio = SunAltitudeToBrightnessRatio(sunAltitude);

      (var hue, var saturation, var value) = color.ToHsv();

      value *= brightnessRatio;

      return ColorUtils.FromHsv(hue, saturation, value);
    }

    private Color NextRainbowColor()
    {
      if (m_RainbowColorIndex >= RainbowColors.Length)
      {
        m_RainbowColorIndex = 0;
      }

      var returnColor = RainbowColors[m_RainbowColorIndex];

      m_RainbowColorIndex++;

      return returnColor;
    }

    private double SunAltitudeToBrightnessRatio(double sunAltitude)
    {
      if (sunAltitude >= 0.0)
        return 1.0;

      if (sunAltitude <= -6.0)
        return 0.2;

      return (2.0 / 15.0) * sunAltitude + 1.0;
    }
  }
}