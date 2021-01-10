using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.TcpService
{
    public abstract class TcpServerServiceOption : Option
    {
        /// <summary>
        /// Socket 监听服务
        /// </summary>
        protected Socket SocketServer;
        public string Host { get; set; }

        public int Port { get; set; }

        public int BufferSize { get; set; } = 0x200;
        public int ReadSize { get; set; } = int.MaxValue;

        public void Start()
        {
            this.SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.SocketServer.SendBufferSize = this.BufferSize;
            this.SocketServer.ReceiveBufferSize = this.BufferSize;

            if (this.Host == null || this.Host.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length != 4)
            {
                this.SocketServer.Bind(new IPEndPoint(System.Net.IPAddress.Any, this.Port));
            }
            else
            {
                string[] strAddress = this.Host.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                byte[] bytAddress = { Byte.Parse(strAddress[0]), Byte.Parse(strAddress[1]), Byte.Parse(strAddress[2]), Byte.Parse(strAddress[3]) };
                this.SocketServer.Bind(new IPEndPoint(new IPAddress(bytAddress), this.Port));
            }

            this.SocketServer.Listen(2);
            Accept();
        }

        private void Accept()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var socket = this.SocketServer.Accept();

                        Receive(socket);
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            });
        }

        //BlockingCollection<byte> Buffers = new BlockingCollection<byte>();

        private void Receive(Socket socket)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] bytes = new byte[this.ReadSize];
                    int readCount = socket.Receive(bytes);

                    byte[] realData = new byte[readCount];

                    Array.Copy(bytes, realData, readCount);

                    DoReceiveData(realData);
                }
            });
        }

        public abstract void DoReceiveData(byte[] data);

    }
}
