namespace BONDCeilingFanCloudConnected
{
    using System.Runtime.CompilerServices;
    using Crestron.SimplSharp;

    /// <summary>
    ///     Basic logging class that prints messages to the crestron console.
    /// </summary>
    public static class BONDLogging
    {
        public static void TraceMessage(
            bool enabled,
            string message = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!enabled) return;
            CrestronConsole.Print("{0} - {1}:{2}  :", memberName, sourceFilePath, sourceLineNumber);
            foreach (var str in message.Split('\n'))
                CrestronConsole.PrintLine(str.TrimEnd('\r'));
        }
    }
}