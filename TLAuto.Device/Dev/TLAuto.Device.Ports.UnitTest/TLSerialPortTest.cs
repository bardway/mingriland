using System;
using System.IO;
using System.IO.Ports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TLAuto.Device.Ports.UnitTest
{
    [TestClass]
    public class TLSerialPortTest
    {
        [TestMethod]
        public void TestTLSerialPort()
        {
            var serialPort = new TLSerialPort();
            try
            {
                serialPort.Open("COM100", 9600, Parity.None, 8, StopBits.One);
            }
            catch (IOException ioexception)
            {
                Assert.IsFalse(false, $"不存在该端口，异常：{ioexception}");
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Assert.IsFalse(false, $"该端口已打开，异常：{invalidOperationException}");
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Assert.IsFalse(false, $"属性值无效，异常：{argumentOutOfRangeException}");
            }
            catch (ArgumentException argumentException)
            {
                Assert.IsFalse(false, $"端口名称不以\"COM\"开头。- 或 -不支持该端口的文件类型，异常：{argumentException}");
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                Assert.IsFalse(false, $"访问被拒绝的端口，异常：{unauthorizedAccessException}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"打开串口出现异常：{ex.Message}");
            }
            serialPort.Close();
            serialPort.Close();
            serialPort.Dispose();
            serialPort.Dispose();
            Assert.IsFalse(serialPort.IsOpen);
        }
    }
}
