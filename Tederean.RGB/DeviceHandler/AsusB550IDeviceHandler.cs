using OpenRGB.NET;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class AsusB550IDeviceHandler : IRgbDeviceHandler
  {

    private const string ModeDirect = "Direct";

    private const string ZoneHeader = "Aura Addressable 1";

    private const int LedsCount = 120;


    private readonly OpenRgbClient _Client;

    private readonly int _DeviceId;

    private readonly int _ZoneId;

    private readonly int _DirectModeId;


    public AsusB550IDeviceHandler(OpenRgbClient client, Device device)
    {
      _Client = client;
      _DeviceId = device.Index;

      _DirectModeId = device.Modes.First(mode => mode.Name == ModeDirect).Index;
      _ZoneId = device.Zones.First(zone => zone.Name == ZoneHeader).Index;
    }


    public void ApplyMode()
    {
      _Client.ResizeZone(_DeviceId, _ZoneId, LedsCount);
      _Client.SaveMode(_DeviceId, _DirectModeId);
    }

    public void SetColor(Color color)
    {
      var nextColors = Enumerable.Repeat(color, LedsCount).ToArray();

      _Client.UpdateZoneLeds(_DeviceId, _ZoneId, nextColors);
    }
  }
}
