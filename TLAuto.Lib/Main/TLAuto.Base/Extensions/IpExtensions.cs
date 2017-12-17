// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class IpExtensions
    {
        public static IList<string> GetInternetIpsWithoutVirtual()
        {
            var listIp = new List<string>();
            var mcNetworkAdapterConfig = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var mocNetworkAdapterConfig = mcNetworkAdapterConfig.GetInstances();
            foreach (var o in mocNetworkAdapterConfig)
            {
                var mo = (ManagementObject)o;
                var mServiceName = mo["ServiceName"] as string;

                //过滤非真实的网卡  
                if (!(bool)mo["IPEnabled"])
                {
                    continue;
                }
                if ((mServiceName != null) && (mServiceName.ToLower().Contains("vmnetadapter")
                                               || mServiceName.ToLower().Contains("ppoe")
                                               || mServiceName.ToLower().Contains("bthpan")
                                               || mServiceName.ToLower().Contains("tapvpn")
                                               || mServiceName.ToLower().Contains("ndisip")
                                               || mServiceName.ToLower().Contains("sinforvnic")))
                {
                    continue;
                }
                //bool mDHCPEnabled = (bool)mo["IPEnabled"];//是否开启了DHCP  
                //string mCaption = mo["Caption"] as string;
                //string mMACAddress = mo["MACAddress"] as string;
                var mIpAddress = mo["IPAddress"] as string[];
                //string[] mIPSubnet = mo["IPSubnet"] as string[];
                var mDefaultIpGateway = mo["DefaultIPGateway"] as string[];
                var mDnsServerSearchOrder = mo["DNSServerSearchOrder"] as string[];

                //Console.WriteLine(mDHCPEnabled);
                //Console.WriteLine(mCaption);
                //Console.WriteLine(mMACAddress);
                //PrintArray(mIPAddress);
                //PrintArray(mIPSubnet);
                //PrintArray(mDefaultIPGateway);
                //PrintArray(mDNSServerSearchOrder);

                if ((mIpAddress != null) && (mDefaultIpGateway != null) && (mDnsServerSearchOrder != null))
                {
                    listIp.AddRange(mIpAddress.Where(ip => ip != "0.0.0.0"));
                }
                mo.Dispose();
            }
            return listIp;
        }

        public static IList<string> GetInternetIps()
        {
            var myEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddressList = myEntry.AddressList.Where(s => s.AddressFamily == AddressFamily.InterNetwork);
            return ipAddressList.Select(s => s.ToString()).ToList();
        }
    }
}