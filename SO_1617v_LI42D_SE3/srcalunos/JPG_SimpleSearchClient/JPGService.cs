using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace JPG_SimpleSearchClient
{
    class JPGService
    {
        public static readonly string SearchCmd = "QUERY_PHOTOS";
        public static readonly string GetPhotoCmd = "GET_PHOTO";
        private string ServerHost = "localhost";
        private static readonly int ServerPort = 8888;
        private static byte CR = 13;
        private static byte LF = 10;

        // Status
        private static readonly int StatusOk = 0;
       

        TcpClient clientSock;
        NetworkStream channel;

        public JPGService()
        {
            clientSock = new TcpClient();
            if (Environment.GetCommandLineArgs().Length == 2)
                ServerHost = Environment.GetCommandLineArgs()[1];

        }

        public void Connect()
        {
            
            clientSock.Connect(ServerHost, ServerPort);
            channel =  clientSock.GetStream();
             
        }

        public void Reconnect()
        {
            if (clientSock != null)
                clientSock.Close();
            clientSock = new TcpClient();
            Connect();
        }

        private byte[] BuildCmd(String cmd, String args)
        {
            MemoryStream ms = new MemoryStream();

            StreamWriter bs = new StreamWriter(ms);
            bs.Write(cmd); bs.Write((char)CR); bs.Write((char)LF);
            bs.Write(args); bs.Write((char)CR); bs.Write((char)LF);
            bs.Flush();
            return ms.ToArray();
        }
        private byte[] BuildSearchCmd(String args)
        {
            return BuildCmd(SearchCmd, args);
        }

        private byte[] BuildGetPhotoCmd(String args)
        {
            return BuildCmd(GetPhotoCmd, args);
        }

        private bool IsDigit(int d)
        {
            return d >= '0' && d <= '9';
        }

        private async Task<int> GetIntAsync()
        {
            int dig;
            int v;
            byte[] buf = new byte[1];

            await channel.ReadAsync(buf, 0, 1);
            v = buf[0]-'0';
            while (IsDigit(dig = channel.ReadByte())) {
                v = v * 10 + (dig - '0');
            }
            if (channel.ReadByte() != LF)
                throw new InvalidDataException();
            return v;
        }

        private async Task<String> GetLineAsync()
        {
            byte[] buf = new byte[1];
            StringBuilder line = new StringBuilder();
            await channel.ReadAsync(buf, 0, 1);

            while (buf[0] != CR)
            {
                line.Append((char)buf[0]);
                channel.Read(buf, 0, 1);
            }
            if (buf[0] == CR) 
            if (channel.ReadByte() != LF)
                throw new InvalidDataException();
            return line.ToString();
        }

        private  async Task<MemoryStream> DownloadPhoto() {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new Byte[4096];
            int response = await GetIntAsync();
            if (response != StatusOk) return ms;
            int fileSize = await GetIntAsync();


            while (fileSize > 0)
            {
                int toRead = Math.Min(fileSize, 4096);
                int nRead = await channel.ReadAsync(buffer, 0, toRead);

                ms.Write(buffer, 0, nRead);
                fileSize -= nRead;
            }
            return ms;
            
        }

         
        public async Task<List<string>> SearchPhotos(String args)
        {
    
            SynchronizationContext oldCtx = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                byte[] cmd = BuildSearchCmd(args);
                await channel.WriteAsync(cmd, 0, cmd.Length);
                List<string> files = new List<String>();
                int response = await GetIntAsync();
                if (response != StatusOk) throw new InvalidDataException();
                string line;
                while ((line = await GetLineAsync()).Length > 0)
                {
                    files.Add(line);
                }
                return files;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldCtx);
            }


        }

        public async Task<MemoryStream> GetPhoto(String arg)
        {
            SynchronizationContext oldCtx = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                byte[] cmd = BuildGetPhotoCmd(arg);
                await channel.WriteAsync(cmd, 0, cmd.Length);
                return await DownloadPhoto();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldCtx);
            }
        }
    }
}
