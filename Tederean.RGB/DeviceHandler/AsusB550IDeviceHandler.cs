using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class AsusB550IDeviceHandler : IRgbDeviceHandler
  {

    private const string ModeDirect = "Direct";

    private const string ModeRainbow = "Rainbow";

    private const string ZoneHeader = "Aura Addressable 1";


    private readonly OpenRGBClient _Client;

    private readonly int _DeviceId;

    private readonly int _ZoneId;

    private readonly int _LedsCount;

    private readonly int _DirectModeId;

    private readonly int _RainbowModeId;


    public AsusB550IDeviceHandler(OpenRGBClient client, Device device, int deviceId)
    {
      _Client = client;
      _DeviceId = deviceId;

      var modes = device.Modes.Select((mode, modeId) => new { Object = mode, Id = modeId }).ToList();

      _DirectModeId = modes.First(mode => mode.Object.Name == ModeDirect).Id;
      _RainbowModeId = modes.First(mode => mode.Object.Name == ModeRainbow).Id;


      var zones = device.Zones.Select((zone, zoneId) => new { Object = zone, Id = zoneId }).ToList();
      var zone = zones.First(zone => zone.Object.Name == ZoneHeader);

      _LedsCount = (int)zone.Object.LedCount;
      _ZoneId = zone.Id;
    }


    public void Initialize()
    {
      _Client.SetMode(_DeviceId, _DirectModeId);
    }

    public void SetColor(Color color)
    {
      var nextColors = Enumerable.Range(0, _LedsCount).Select(e => color).ToArray();

      _Client.UpdateZone(_DeviceId, _ZoneId, nextColors);
    }

    public void Shutdown()
    {
      _Client.SetMode(_DeviceId, _RainbowModeId);
    }
  }
}
