using OpenRGB.NET;
using OpenRGB.NET.Utils;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class SteelSeriesRival3 : IRgbDeviceHandler
  {

    private const string ModeDirect = "Direct";

    private const double DimmingFactor = 0.1;


    private readonly OpenRgbClient _Client;

    private readonly int _DeviceId;

    private readonly int _LedsCount;

    private readonly int _StaticModeId;


    public SteelSeriesRival3(OpenRgbClient client, Device device)
    {
      _Client = client;
      _DeviceId = device.Index;
      _LedsCount = device.Leds.Length;

      _StaticModeId = device.Modes.First(mode => mode.Name == ModeDirect).Index;
    }


    public void ApplyMode()
    {
      _Client.SaveMode(_DeviceId, _StaticModeId);
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