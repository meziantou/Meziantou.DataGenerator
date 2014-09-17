using System;
using System.Diagnostics.Eventing;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Meziantou.DataGenerator.Diagnostics
{
    public static class Logger
    {
        public static Guid ProviderGuid = new Guid("ECB7F943-447A-48B7-8F2C-7D8E94AF6DC3");
        private static readonly EventProvider _provider = new EventProvider(ProviderGuid);

        public static void Log(LogType type, int indent = 0, [CallerMemberName]string methodName = null, object value = null)
        {
            string sindent = indent > 0 ? new string(' ', indent * 4) : null;
            _provider.WriteMessageEvent(sindent + "[" + Thread.CurrentThread.ManagedThreadId + "]" + type + "." + methodName + ": " + value, (byte)type, 0);
        }
    }
}
