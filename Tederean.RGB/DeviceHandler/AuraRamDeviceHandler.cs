using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class AuraRamDeviceHandler : IRgbDeviceHandler
  {

    private const string ModeStatic = "Static";

    private const string ModeRainbow = "Rainbow";


    private readonly OpenRGBClient _Client;

    private readonly int _DeviceId;

    private readonly int _LedsCount;

    private readonly int _StaticModeId;

    private readonly int _RainbowModeId;


    public AuraRamDeviceHandler(OpenRGBClient client, Device device, int deviceId)
    {
      _Client = client;
      _DeviceId = deviceId;
      _LedsCount = device.Leds.Length;

      var modes = device.Modes.Select((mode, modeId) => new { Object = mode, Id = modeId }).ToList();

      _StaticModeId = modes.First(mode => mode.Object.Name == ModeStatic).Id;
      _RainbowModeId = modes.First(mode => mode.Object.Name == ModeRainbow).Id;
    }


    public void Initialize()
    {
      _Client.SetMode(_DeviceId, _StaticModeId);
    }

    public void SetColor(Color color)
    {
      var nextColors = Enumerable.Range(0, _LedsCount).Select(e => color).ToArray();

      _Client.UpdateLeds(_DeviceId, nextColors);
    }

    public void Shutdown()
    {
      _Client.SetMode(_DeviceId, _RainbowModeId);
    }
  }
}
