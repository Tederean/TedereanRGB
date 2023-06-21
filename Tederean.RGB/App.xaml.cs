using System;
using System.Threading;
using System.Windows;

namespace Tederean.RGB
{

  public partial class App : Application
  {

    protected override async void OnStartup(StartupEventArgs args)
    {
      try
      {
        base.OnStartup(args);


        using (var cancellationTokenSource = new CancellationTokenSource())
        {
          
          void OnAppSessionEndingInternal(object sender, SessionEndingCancelEventArgs args)
          {
            cancellationTokenSource.Cancel();
          }


          SessionEnding += OnAppSessionEndingInternal;

          try
          {
            await Program.RunAsync(cancellationTokenSource);
          }
          finally
          {
            SessionEnding -= OnAppSessionEndingInternal;
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
      finally
      {
        Shutdown();
      }
    }
  }
}