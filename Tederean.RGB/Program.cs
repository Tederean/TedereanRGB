using System;
using System.Diagnostics;
using System.Threading;

namespace Tederean.RGB
{

  public static class Program
  {
    
    public static void Main(string[] args)
    {
      var mode = new SpectrumShift("127.0.0.1");

      while (true)
      {
        try
        {
          mode.Run();
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine(ex.ToString());

          Thread.Sleep(1000);
        }
      }
    }
  }
}
