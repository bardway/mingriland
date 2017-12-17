// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
using TLAuto.Base.Network.Ports;
using TLAuto.Machine.Plugins.Cooking.Models.Enums;
#endregion

namespace TLAuto.Machine.Plugins.Cooking
{
    public static class ArduinoHelper
    {
        private static readonly SerialPortWrapper SerialPortWrapper;
        private static readonly ConcurrentQueue<string> _queues = new ConcurrentQueue<string>();

        static ArduinoHelper()
        {
            var com = ConfigurationManager.AppSettings["CookingSerialPort"];
            SerialPortWrapper = new SerialPortWrapper(com);
        }

        public static bool Connect()
        {
            var result = SerialPortWrapper.Connect();
            if (result)
            {
                Task.Factory.StartNew(async () =>
                                      {
                                          while (true)
                                          {
                                              while (!_queues.IsEmpty)
                                              {
                                                  string data;
                                                  if (_queues.TryDequeue(out data))
                                                  {
                                                      SerialPortWrapper.Send(data.HexStrToBytes(" "));
                                                      await Task.Delay(50);
                                                  }
                                              }
                                              await Task.Delay(30);
                                          }
                                      });
            }
            return result;
        }

        public static void Close()
        {
            SerialPortWrapper.Close();
        }

        public static void ShowLed2(int number, int ledIndex)
        {
            var ledNumber = number.ToPadLeft();
            for (var i = 0; i < ledNumber.Length; i++)
            {
                var data = $"55 7A {01.ToHex()} {((ledIndex * 2) + i).ToHex()} {ledNumber.Substring(i, 1).ToInt32().ToHex()} 11";
                _queues.Enqueue(data);
            }
        }

        public static void ShowLed3(int number, Led3Type led3Type)
        {
            var ledNumber = number.ToPadLeft(3);
            if (led3Type == Led3Type.Temperature)
            {
                var index = 0;
                for (var i = ledNumber.Length - 1; i >= 0; i--)
                {
                    var data = $"55 7A 02 {((led3Type.ToInt32() * 3) + i).ToHex()} {ledNumber.Substring(index++, 1).ToInt32().ToHex()} 11";
                    _queues.Enqueue(data);
                }
            }
            else
            {
                var index = 0;
                for (var i = ledNumber.Length - 1; i >= 0; i--)
                {
                    var data = $"55 7A 02 {((led3Type.ToInt32() * 3) + i).ToHex()} {ledNumber.Substring(index++, 1).ToInt32().ToHex()} 11";
                    _queues.Enqueue(data);
                }
            }
        }
    }
}