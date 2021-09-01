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


    private readonly OpenRGBClient m_Client;

    private readonly int m_DeviceId;

    private readonly int m_ZoneId;

    private readonly int m_LedsCount;

    private readonly int m_DirectModeId;

    private readonly int m_RainbowModeId;


    public AsusB550IDeviceHandler(OpenRGBClient client, Device device, int deviceId)
    {
      m_Client = client;
      m_DeviceId = deviceId;

      var modes = device.Modes.Select((mode, modeId) => new { Object = mode, Id = modeId }).ToList();

      m_DirectModeId = modes.First(mode => mode.Object.Name == ModeDirect).Id;
      m_RainbowModeId = modes.First(mode => mode.Object.Name == ModeRainbow).Id;


      var zones = device.Zones.Select((zone, zoneId) => new { Object = zone, Id = zoneId }).ToList();
      var zone = zones.First(zone => zone.Object.Name == ZoneHeader);

      m_LedsCount = (int)zone.Object.LedCount;
      m_ZoneId = zone.Id;
    }


    public void Initialize()
    {
      m_Client.SetMode(m_DeviceId, m_DirectModeId);
    }

    public void SetColor(Color color)
    {
      var nextColors = Enumerable.Range(0, m_LedsCount).Select(e => color).ToArray();

      m_Client.UpdateZone(m_DeviceId, m_ZoneId, nextColors);
    }

    public void Shutdown()
    {
      m_Client.SetMode(m_DeviceId, m_RainbowModeId);
    }
  }
}
