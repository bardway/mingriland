// --------------------------
//   Author => Lex XIAO
// --------------------------

namespace TLAuto.Device.Contracts
{
    public class WcfResultInfo
    {
        private string _errorMsg;

        public bool IsError { private set; get; }

        public string ErrorMsg
        {
            set
            {
                _errorMsg = value;
                if (!string.IsNullOrEmpty(_errorMsg))
                {
                    IsError = true;
                }
            }
            get => _errorMsg;
        }

        public byte[] Data { set; get; }
    }
}