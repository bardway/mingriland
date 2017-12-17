// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Concurrent;
using System.Collections.Generic;
#endregion

namespace TLAuto.Machine.Plugins.HitCheckTask
{
    public static class HitTaskCache
    {
        private static readonly ConcurrentQueue<SwitchItemWithDeviceNumber> Messages = new ConcurrentQueue<SwitchItemWithDeviceNumber>();

        //private static readonly Queue<SwitchItemWithDeviceNumber> Messages = new Queue<SwitchItemWithDeviceNumber>();

        public static void PushMessage(SwitchItemWithDeviceNumber message)
        {
            Messages.Enqueue(message);
        }

        public static List<SwitchItemWithDeviceNumber> GetMessages()
        {
            var messages = new List<SwitchItemWithDeviceNumber>();
            var queuecount = Messages.Count;
            for (var i = 0; i < queuecount; i++)
            {
                SwitchItemWithDeviceNumber message;
                Messages.TryDequeue(out message);
                messages.Add(message);
            }
            return messages;
        }

        public static SwitchItemWithDeviceNumber PopMessage()
        {
            SwitchItemWithDeviceNumber message;
            Messages.TryDequeue(out message);
            return message;
        }

        public static int CountQueue()
        {
            return Messages.Count;
        }
    }
}