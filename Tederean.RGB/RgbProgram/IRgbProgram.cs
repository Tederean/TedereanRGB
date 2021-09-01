using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;

namespace Tederean.RGB.RgbProgram
{

  public interface IRgbProgram
  {

    Task Run(Func<bool> isShutdown, List<IRgbDeviceHandler> deviceHandlers);
  }
}
