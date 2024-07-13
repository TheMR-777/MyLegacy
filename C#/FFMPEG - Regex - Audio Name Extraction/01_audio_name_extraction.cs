using System;
using System.Text.RegularExpressions;

public partial class Program
{
    public static void Main()
    {
        const string input = "[dshow @ 000001fc7a77a440] \"HP Wide Vision HD Camera\" (video)\n" +
                       "[dshow @ 000001fc7a77a440]   Alternative name \"@device_pnp_\\\\?\\usb#vid_0408&pid_5481&mi_00#6&15cea75&0&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\\global\"\n" +
                       "[dshow @ 000001fc7a77a440] \"Microphone Array (Intel Smart Sound Technology for Digital Microphones)\" (audio)\n" +
                       "[dshow @ 000001fc7a77a440]   Alternative name \"@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\\wave_{9D85A943-F2FA-4B3D-8D1C-8746BFE11CA3}\"";

        var match = MyRegex().Match(input);
        if (match.Success)
        {
            Console.WriteLine(match.Groups[1].Value);
        }
    }

    [GeneratedRegex("""\[dshow.*?\] "(.*?)" \(audio\)""")]
    private static partial Regex MyRegex();
}