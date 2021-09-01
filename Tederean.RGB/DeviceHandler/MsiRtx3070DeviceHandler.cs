using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class MsiRtx3070DeviceHandler : IRgbDeviceHandler
  {

    private const string ModeStatic = "Static";

    private const string ModeRainbow = "Rainbow";

    private const double DimmingFactor = 0.5;


    private readonly OpenRGBClient m_Client;

    private readonly int m_DeviceId;

    private readonly int m_LedsCount;

    private readonly int m_StaticModeId;

    private readonly int m_RainbowModeId;


    public MsiRtx3070DeviceHandler(OpenRGBClient client, Device device, int deviceId)
    {
      m_Client = client;
      m_DeviceId = deviceId;
      m_LedsCount = device.Leds.Length;

      var modes = device.Modes.Select((mode, modeId) => new { Object = mode, Id = modeId }).ToList();

      m_StaticModeId = modes.First(mode => mode.Object.Name == ModeStatic).Id;
      m_RainbowModeId = modes.First(mode => mode.Object.Name == ModeRainbow).Id;
    }


    public void Initialize()
    {
      m_Client.SetMode(m_DeviceId, m_StaticModeId);
    }

    public void SetColor(Color color)
    {
      (double hue, double saturation, double value) = color.ToHsv();
      color = Color.FromHsv(hue, saturation, value * DimmingFactor);

      var nextColors = Enumerable.Range(0, m_LedsCount).Select(e => color).ToArray();

      m_Client.UpdateLeds(m_DeviceId, nextColors);
    }

    public void Shutdown()
    {
      m_Client.SetMode(m_DeviceId, m_RainbowModeId);
    }
  }
}
