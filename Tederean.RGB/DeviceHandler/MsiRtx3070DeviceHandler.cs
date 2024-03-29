﻿using OpenRGB.NET;
using OpenRGB.NET.Utils;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class MsiRtx3070DeviceHandler : IRgbDeviceHandler
  {

    private const string ModeDirect = "Direct";

    private const double DimmingFactor = 0.5;


    private readonly OpenRgbClient _Client;

    private readonly int _DeviceId;

    private readonly int _LedsCount;

    private readonly int _DirectModeId;


    public MsiRtx3070DeviceHandler(OpenRgbClient client, Device device)
    {
      _Client = client;
      _DeviceId = device.Index;

      _LedsCount = device.Leds.Length;
      _DirectModeId = device.Modes.First(mode => mode.Name == ModeDirect).Index;
    }


    public void ApplyMode()
    {
      _Client.SaveMode(_DeviceId, _DirectModeId);
    }

    public void SetColor(Color color)
    {
      (var hue, var saturation, var value) = color.ToHsv();
      color = ColorUtils.FromHsv(hue, saturation, value * DimmingFactor);

      var nextColors = Enumerable.Repeat(color, _LedsCount).ToArray();

      _Client.UpdateLeds(_DeviceId, nextColors);
    }
  }
}
