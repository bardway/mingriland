// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.DMX
{
    public class TLAutoDMXDevice
    {
        private readonly byte[] _dmxData = new byte[0x200];
        private int _hLib;

        public TLAutoDMXDevice()
        {
            _dmxData[0] = 1;
            _hLib = 0;
        }

        public bool IsLoaded { private set; get; }

        public async Task LoadDMXDevice()
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            if (_hLib != 0)
                                            {
                                                FreeLibrary(_hLib);
                                            }
                                            _hLib = 0; //Free OLD
                                            _hLib = LoadLibrary("USB-DMX.dll");
                                            if (_hLib != 0)
                                            {
                                                DMXSend = (TDMXSend)GetAddress(_hLib, "DMXSend", typeof(TDMXSend));
                                                DMXSends = (TDMXSends)GetAddress(_hLib, "DMXSends", typeof(TDMXSends));
                                                DMXOpen = (TDMXOpen)GetAddress(_hLib, "DMXOpen", typeof(TDMXOpen));
                                                DMXClose = (TDMXClose)GetAddress(_hLib, "DMXClose", typeof(TDMXClose));
                                                IsLoaded = true;
                                            }
                                            else
                                            {
                                                IsLoaded = false;
                                            }
                                        });
        }

        public async Task CloseDMXDevice()
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            if (DMXClose != null)
                                            {
                                                DMXClose();
                                                FreeLibrary(_hLib);
                                                IsLoaded = false;
                                            }
                                        });
        }

        public async Task<bool> CheckDMXDeviceStatus()
        {
            if (!IsLoaded)
            {
                return false;
            }
            return await Task.Factory.StartNew(() => DMXOpen());
        }

        public async Task SendSingleChannel(int channelNum, int value)
        {
            if (!IsLoaded)
            {
                return;
            }
            await Task.Factory.StartNew(() => { DMXSend(channelNum, value.ToByte()); });
        }

        public async Task<bool> SendMulitiSingleChannel(IEnumerable<Tuple<int, int>> multiChannels)
        {
            if (!IsLoaded)
            {
                return false;
            }
            return await Task.Factory.StartNew(() =>
                                               {
                                                   foreach (var channel in multiChannels)
                                                   {
                                                       DMXSend(channel.Item1, channel.Item2.ToByte());
                                                   }
                                                   return true;
                                               });
        }

        public async Task SendMulitiChannle(int channelBegin, int channelEnd, int value)
        {
            if (!IsLoaded)
            {
                return;
            }
            await Task.Factory.StartNew(() =>
                                        {
                                            for (var i = 0; i < 512; i++)
                                            {
                                                _dmxData[i] = value.ToByte();
                                            }
                                            DMXSends((channelEnd - channelBegin) + 1, channelBegin, _dmxData);
                                        });
        }

        #region DMX API
        public delegate int TDMXSend(int Channel, byte Value);

        public delegate int TDMXSends(int ChannelCount, int ChannelIndex, byte[] Value);

        public delegate bool TDMXOpen();

        public delegate void TDMXClose();

        TDMXSend DMXSend;
        TDMXSends DMXSends;
        TDMXOpen DMXOpen;
        TDMXClose DMXClose;

        [DllImport("Kernel32")]
        public static extern int GetProcAddress(int handle, string funcname);

        [DllImport("Kernel32")]
        public static extern int LoadLibrary(string funcname);

        [DllImport("Kernel32")]
        public static extern int FreeLibrary(int handle);

        private static Delegate GetAddress(int dllModule, string functionname, Type t)
        {
            var addr = GetProcAddress(dllModule, functionname);
            if (addr == 0)
            {
                return null;
            }
            return Marshal.GetDelegateForFunctionPointer(new IntPtr(addr), t);
        }
        #endregion
    }
}