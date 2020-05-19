using BaseComManager;
using PackageManager;
using System;
using System.IO.Ports;
using System.Text;

namespace SerialComManager
{
    public class SerialListener : BaseListener<SerialDestInfo, SerialInitInfo>, IBaseListener<SerialDestInfo, SerialInitInfo>
    {
        #region Events

        public override event NetworkPackageReceived PackageReceived;
        public override event NetworkBytesReceived BytesPackageReceived;
        public override event NetworkTextReceived TextReceived;
        public override event SendPackage<SerialDestInfo> SendPackage;
        public override event SendMessage<SerialDestInfo> SendMessage;
        public override event SendByteArray<SerialDestInfo> SendByteArray;

        #endregion

        #region Properties...

        private NetworkPackageGenerator _packageGenerator;

        private SerialPort _serialPort; 
        #endregion

        public SerialListener()
        {

        }

        public override void Initialize(NetworkPackageGenerator packageGenerator, SerialInitInfo initInfo)
        {
            _packageGenerator = packageGenerator;
            _serialPort       = new SerialPort();
            _serialPort.PortName = initInfo.Port;
            _serialPort.BaudRate = initInfo.BaudRate;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        public override void Connect()
        {
            //_serialPort.Open();
        }

        public override void Disconnect()
        {
            _serialPort.Close();
        }

        public override bool Send(string message, SerialDestInfo param)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            return SendBytes(bytes);
        }

        public override bool Send(byte[] bytes, SerialDestInfo param)
        {
            return SendBytes(bytes);
        }

        public override bool Send(NetworkPackage package, SerialDestInfo param)
        {
            byte[] bytes = package.GenerateByteArray();
            return SendBytes(bytes); 
        }

        public override bool SendFromApi(string message, SerialDestInfo param)
        {
            return this.SendMessage(message, param);
        }

        public override bool SendFromApi(byte[] bytes, SerialDestInfo param)
        {
            return this.SendByteArray(bytes, param);
        }

        public override bool SendFromApi(NetworkPackage package, SerialDestInfo param)
        {
            return this.SendPackage(package, param);
        }





        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] bytes = GetBytes();
            ReceivePackage(bytes);
            ReceiveBytes(bytes);
            ReceiveText(bytes);
        }

        private byte[] GetBytes()
        {
            byte[] bytes = null;

            int byteCount = _serialPort.BytesToRead;
            bytes = new byte[byteCount];
            _serialPort.Read(bytes, 0, byteCount);

            return bytes;
        }

        private void ReceivePackage(byte[] bytes)
        {
            try
            {
                NetworkPackage networkPackage = _packageGenerator.Generate(bytes);
                PackageReceived?.Invoke(networkPackage);
            }
            catch
            {

            }
        }

        private void ReceiveBytes(byte[] bytes)
        {
            try
            {
                BytesPackageReceived?.Invoke(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReceiveText(byte[] bytes)
        {
            try
            {
                TextReceived?.Invoke(Encoding.ASCII.GetString(bytes));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool SendBytes(byte[] bytes)
        {
            _serialPort.Write(bytes, 0, bytes.Length);
            return true;
        }

    }
}
