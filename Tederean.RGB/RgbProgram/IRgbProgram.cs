using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tederean.RGB.DeviceHandler;

namespace Tederean.RGB.RgbProgram
{

  public interface IRgbProgram
  {

    Task RunAsync(CancellationToken cancellationToken, List<IRgbDeviceHandler> deviceHandlers);
  }
}
