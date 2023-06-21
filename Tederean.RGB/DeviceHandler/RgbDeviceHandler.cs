using OpenRGB.NET;
using System.Collections.Generic;

namespace Tederean.RGB.DeviceHandler
{

  public static class RgbDeviceHandler
  {

    public static IEnumerable<IRgbDeviceHandler> GetDeviceHandlers(OpenRgbClient client)
    {
      foreach (var device in client.GetAllControllerData())
      {
        if (device.Name == "ENE DRAM")
        {
          yield return new AuraRamDeviceHandler(client, device);
        }

        if (device.Name == "MSI GeForce RTX 3070 8GB Gaming X Trio")
        {
          yield return new MsiRtx3070DeviceHandler(client, device);
        }

        if (device.Name == "ASUS ROG STRIX B550-I GAMING")
        {
          yield return new AsusB550IDeviceHandler(client, device);
        }

        if (device.Name.StartsWith("SteelSeries Rival 3"))
        {
          yield return new SteelSeriesRival3(client, device);
        }
      }
    }
  }
}
