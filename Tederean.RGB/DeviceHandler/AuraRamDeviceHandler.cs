using OpenRGB.NET;
using System;
using System.Linq;

namespace Tederean.RGB.DeviceHandler
{

  public class AuraRamDeviceHandler : IRgbDeviceHandler
  {

    //private const string ModeStatic = "Static";

    //private const string ModeRainbow = "Rainbow";

    private const string ModeDirect = "Direct";


    private readonly OpenRgbClient _Client;

    private readonly int _DeviceId;

    private readonly int _LedsCount;

    //private readonly int _StaticModeId;

    //private readonly int _RainbowModeId;

    private readonly int _DirectModeId;


    private DateTime _LastModeCheck_Utc;


    public AuraRamDeviceHandler(OpenRgbClient client, Device device)
    {
      _Client = client;
      _DeviceId = device.Index;
      _LedsCount = device.Leds.Length;

      //_StaticModeId = device.Modes.First(mode => mode.Name == ModeStatic).Index;
      //_RainbowModeId = device.Modes.First(mode => mode.Name == ModeRainbow).Index;
      _DirectModeId = device.Modes.First(mode => mode.Name == ModeDirect).Index;
    }


    public void ApplyMode()
    {
      _Client.SaveMode(_DeviceId, _DirectModeId);
    }

    public void SetColor(Color color)
    {
      CheckAndCorrectMode();

      var nextColors = Enumerable.Repeat(color, _LedsCount).ToArray();

      _Client.UpdateLeds(_DeviceId, nextColors);
    }

    private void CheckAndCorrectMode()
    {
      var now_utc = DateTime.UtcNow;

      if ((now_utc - _LastModeCheck_Utc) > TimeSpan.FromSeconds(10))
      {
        _LastModeCheck_Utc = now_utc;

        var device = _Client.GetControllerData(_DeviceId);

        if (device.ActiveModeIndex != _DirectModeId)
        {
          ApplyMode();
        }
      }
    }
  }
}
