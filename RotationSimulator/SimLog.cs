using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public class SimLog
    {
        /// <summary>
        /// If false, simply ignores all messages. 
        /// Ideally loggers shouldn't even bother composing the message if disabled.
        /// </summary>
        public static bool Enabled = true;

        private static List<SimLogEvent> events = new List<SimLogEvent>();

        public static void Clear() {
            events.Clear();
        }

        public static void Error(string text, int timeStamp, ActionDef action = null) {
            if (!Enabled) { return; }

            events.Add(new SimLogEvent()
            {
                LogLevel = ESimLogLevel.Error,
                Message = text,
                TimeStamp = timeStamp,
                RelevantAction = action
            }) ;
        }

        public static void Warning(string text, int timeStamp, ActionDef action = null) {
            if (!Enabled) { return; }

            events.Add(new SimLogEvent()
            {
                LogLevel = ESimLogLevel.Warning,
                Message = text,
                TimeStamp = timeStamp,
                RelevantAction = action
            });
        }

        public static void Info(string text, int timeStamp, ActionDef action = null) {
            if (!Enabled) { return; }

            events.Add(new SimLogEvent()
            {
                LogLevel = ESimLogLevel.Info,
                Message = text,
                TimeStamp = timeStamp,
                RelevantAction = action
            });
        }
        public static void Detail(string text, int timeStamp, ActionDef action = null) {
            if (!Enabled) { return; }

            events.Add(new SimLogEvent()
            {
                LogLevel = ESimLogLevel.Detail,
                Message = text,
                TimeStamp = timeStamp,
                RelevantAction = action
            });
        }

        public static IEnumerable<SimLogEvent> GetErrors() {
            foreach (SimLogEvent e in events) {
                if (e.LogLevel == ESimLogLevel.Error) {
                    yield return e;
                }
            }
        }
        public static IEnumerable<SimLogEvent> GetErrorsWarnings() {
            foreach (SimLogEvent e in events) {
                if (e.LogLevel == ESimLogLevel.Error || e.LogLevel == ESimLogLevel.Warning) {
                    yield return e;
                }
            }
        }
        public static IEnumerable<SimLogEvent> GetInfoOrWorse() {
            foreach (SimLogEvent e in events) {
                if (e.LogLevel == ESimLogLevel.Error || e.LogLevel == ESimLogLevel.Warning || e.LogLevel == ESimLogLevel.Info) {
                    yield return e;
                }
            }
        }

        public static IEnumerable<SimLogEvent> GetAll() {
            foreach (SimLogEvent e in events) {
                yield return e;
            }
        }
    }
}
