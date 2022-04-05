using System;
using System.Runtime.InteropServices;

namespace Tederean.RGB
{

  public static class WindowUtil
  {

    private const int SW_HIDE = 0;

    private const int SW_SHOW = 5;


    public static void SetWindowVisibility(bool isVisible)
    {
      var consoleWindow = GetConsoleWindow();
      var nCmdShow = isVisible ? SW_SHOW : SW_HIDE;

      ShowWindow(consoleWindow, nCmdShow);
    }


    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
  }
}
