using System.Diagnostics;

namespace SettingsForTV;

internal class CommandLine
{
    public void CallCommandLine(string property, string value)
    {
        var process = new Process();
        ProcessStartInfo startInfo = null;
        switch (property)
        {
            case "Resolution":
                startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = @"C:\Users\mamorrison\Downloads\Compressed\qres-src1097",
                    FileName = "cmd.exe",
                    Arguments = value
                    /*qres x=Width y=Height f = Frame-Rate.
                    For Example: qres x=1920 y=1080 f=60.*/
                };

                break;
        }

        if (startInfo != null) process.StartInfo = startInfo;
        process.Start();
    }
}