using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class MsiRtx3070DeviceHandler : IRgbDeviceHandler
  {

    private const string ModeDirect = "Direct";

    private const string ModeRainbow = "Rainbow";

    private const double DimmingFactor = 0.5;


    private readonly OpenRGBClient _Client;

    private readonly int _DeviceId;

    private readonly int _LedsCount;

    private readonly int _DirectModeId;

    private readonly int _RainbowModeId;


    public MsiRtx3070DeviceHandler(OpenRGBClient client, Device device, int deviceId)
    {
      _Client = client;
      _DeviceId = deviceId;
      _LedsCount = device.Leds.Length;

      var modes = device.Modes.Select((mode, modeId) => new { Object = mode, Id = modeId }).ToList();
            
      _DirectModeId = modes.First(mode => mode.Object.Name == ModeDirect).Id;
      _RainbowModeId = modes.First(mode => mode.Object.Name == ModeRainbow).Id;
    }


    public void Initialize()
    {
      _Client.SetMode(_DeviceId, _DirectModeId);
    }

    public void SetColor(Color color)
    {
      (var hue, var saturation, var value) = color.ToHsv();
      color = Color.FromHsv(hue, saturation, value * DimmingFactor);

      var nextColors = Enumerable.Range(0, _LedsCount).Select(e => color).ToArray();

      _Client.UpdateLeds(_DeviceId, nextColors);
    }

    public void Shutdown()
    {
      _Client.SetMode(_DeviceId, _RainbowModeId);
    }
  }
}
