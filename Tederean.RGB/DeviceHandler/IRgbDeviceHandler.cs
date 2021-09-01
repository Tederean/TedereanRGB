using OpenRGB.NET.Models;

namespace Tederean.RGB.DeviceHandler
{

  public interface IRgbDeviceHandler
  {

    void Initialize();

    void SetColor(Color color);

    void Shutdown();
  }
}
