using OpenRGB.NET;
using System.Collections.Generic;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public static class RgbDeviceHandler
  {

    public static List<IRgbDeviceHandler> GetDeviceHandlers(OpenRGBClient client)
    {
      var rgbDevices = client.GetAllControllerData().Select((device, deviceId) => new { Object = device, Id = deviceId }).ToList();
      var deviceHandlers = new List<IRgbDeviceHandler>();

      deviceHandlers.AddRange(rgbDevices.Where(device => device.Object.Name == "ENE DRAM").Select(device =>
      {
        return new AuraRamDeviceHandler(client, device.Object, device.Id);
      }));

      deviceHandlers.AddRange(rgbDevices.Where(device => device.Object.Name == "MSI GeForce RTX 3070 8GB Gaming X Trio").Select(device =>
      {
        return new MsiRtx3070DeviceHandler(client, device.Object, device.Id);
      }));

      deviceHandlers.AddRange(rgbDevices.Where(device => device.Object.Name == "ASUS ROG STRIX B550-I GAMING").Select(device =>
      {
        return new AsusB550IDeviceHandler(client, device.Object, device.Id);
      }));

      return deviceHandlers;
    }
  }
}
