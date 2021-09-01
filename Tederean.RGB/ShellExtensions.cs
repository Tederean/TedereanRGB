using System.Diagnostics;

namespace Tederean.RGB
{

  public static class ShellExtensions
  {

    public static Process RunInShell(this string command)
    {
      var process = new Process()
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          Arguments = $"-c \"{command}\"",
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };

      process.Start();

      return process;
    }
  }
}
