using OpenRGB.NET;

namespace Tederean.RGB.DeviceHandler
{

  public interface IRgbDeviceHandler
  {

    void ApplyMode();

    void SetColor(Color color);
  }
}
