using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
//using System.Text;
using System.Windows.Forms;

namespace PancreAppPC
{
    class WebSocket
    {
        private Uri mUrl;
        private TcpClient mClient;
        private NetworkStream mStream;
        private bool mHandshakeComplete;
        private Dictionary<string, string> mHeaders;

        public WebSocket(Uri url)
        {
            mUrl = url;

            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            mHeaders = headers;
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
    {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
            builder.Append(ch);
        }

        return builder.ToString();
    }
        public void Connect()
        {
            string host = mUrl.DnsSafeHost;
            string path = mUrl.PathAndQuery;
            string origin = "http://" + host;

            mClient = CreateSocket(mUrl);
            mStream = mClient.GetStream();

            int port = ((IPEndPoint)mClient.Client.RemoteEndPoint).Port;
            if (port != 80)
                host = host + ":" + port;

            StringBuilder extraHeaders = new StringBuilder();
            if (mHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in mHeaders)
                    extraHeaders.Append(header.Key + ": " + header.Value + "\r\n");
            }

            /*string request = "GET " + path + " HTTP/1.1\r\n" +
                             "Upgrade: WebSocket\r\n" +
                             "Connection: Upgrade\r\n" +
                             "Host: " + host + "\r\n" +
                             "Origin: " + origin + "\r\n" +
                             "Sec-WebSocket-Key: GuJF8fisc2/6Q1Xyzu9fw==\r\n" +
"Sec-WebSocket-Version: 13\r\n" +
                             extraHeaders.ToString() + "\r\n";*/

            string request = "GET " + path + " HTTP/1.1\r\n" +
"Host: " + host + "\r\n" +
                             "Origin: " + origin + "\r\n" +
"Upgrade: WebSocket\r\n" +
"Connection: Upgrade\r\n" +
"Sec-WebSocket-Key: a4on2b+RhIScj/u5h6yYMQ==\r\n" +
"Sec-WebSocket-Version: 13\r\n" +
                             extraHeaders.ToString() + "\r\n"; 
            byte[] sendBuffer = Encoding.UTF8.GetBytes(request);

            mStream.Write(sendBuffer, 0, sendBuffer.Length);

            StreamReader reader = new StreamReader(mStream);
            {
                /*string header = reader.ReadLine();
                //if (!header.Equals("HTTP/1.1 101 Web Socket Protocol Handshake"))
                //    throw new IOException("Invalid handshake response");
                MessageBox.Show(header);
                Clipboard.SetText(header);
                header = reader.ReadLine();
                //if (!header.Equals("Upgrade: WebSocket"))
                //    throw new IOException("Invalid handshake response");
                MessageBox.Show(header);
                header = reader.ReadLine();
                //if (!header.Equals("Connection: Upgrade"))
                //    throw new IOException("Invalid handshake response");*/


                //MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine()); MessageBox.Show(reader.ReadLine());
            }

            mHandshakeComplete = true;
        }
//        public string decodeWebSocket (byte[] data){
//    var datalength = data[1] & 127;
//    var indexFirstMask = 2;
//    if (datalength == 126) {
//        indexFirstMask = 4;
//    } else if (datalength == 127) {
//        indexFirstMask = 10;
//    }
//    var masks = data.slice(indexFirstMask,indexFirstMask + 4);
//    var i = indexFirstMask + 4;
//    var index = 0;
//    var output = "";
//    while (i < data.length) {
//        output += String.fromCharCode(data[i++] ^ masks[index++ % 4]);
//    }
//    return output;
//}
//        public byte encodeWebSocket(string bytesRaw){
//    List<byte> bytesFormatted = new List<byte>();
//    bytesFormatted[0] = 129;
//    if (bytesRaw.Length <= 125) {
//        bytesFormatted[1] = bytesRaw.Length;
//    }
//    else if (bytesRaw.Length >= 126 && bytesRaw.length <= 65535)
//    {
//        bytesFormatted[1] = 126;
//        bytesFormatted[2] = ( bytesRaw.length >> 8 ) & 255;
//        bytesFormatted[3] = ( bytesRaw.length      ) & 255;
//    } else {
//        bytesFormatted[1] = 127;
//        bytesFormatted[2] = ( bytesRaw.length >> 56 ) & 255;
//        bytesFormatted[3] = ( bytesRaw.length >> 48 ) & 255;
//        bytesFormatted[4] = ( bytesRaw.length >> 40 ) & 255;
//        bytesFormatted[5] = ( bytesRaw.length >> 32 ) & 255;
//        bytesFormatted[6] = ( bytesRaw.length >> 24 ) & 255;
//        bytesFormatted[7] = ( bytesRaw.length >> 16 ) & 255;
//        bytesFormatted[8] = ( bytesRaw.length >>  8 ) & 255;
//        bytesFormatted[9] = ( bytesRaw.length       ) & 255;
//    }
//    for (var i = 0; i < bytesRaw.length; i++){
//        bytesFormatted.push(bytesRaw.charCodeAt(i));
//    }
//    return bytesFormatted;
//}

        public void Send(string str)
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");

            byte[] sendBuffer = Encoding.UTF8.GetBytes(str);


            

//function decodeWebSocket (data){
//    var datalength = data[1] & 127;
//    var indexFirstMask = 2;
//    if (datalength == 126) {
//        indexFirstMask = 4;
//    } else if (datalength == 127) {
//        indexFirstMask = 10;
//    }
//    var masks = data.slice(indexFirstMask,indexFirstMask + 4);
//    var i = indexFirstMask + 4;
//    var index = 0;
//    var output = "";
//    while (i < data.length) {
//        output += String.fromCharCode(data[i++] ^ masks[index++ % 4]);
//    }
//    return output;
//}


            List<byte> lb = new List<byte>();
            lb.Add(0x00);
            //0x04 = 00001000
            //       0        No mask
            //        0001000 Rest of the 7 bytes left is the length of the payload.
            
            // add the payload
            lb.AddRange(Encoding.UTF8.GetBytes(str));
            lb.Add(0xFF);
            //write it!
            //mStream.WriteByte(0x00);
            mStream.Write(lb.ToArray(), 0, sendBuffer.Length ); //mStream.WriteByte(0xFF);
            //mStream.WriteByte(0x00);
            //mStream.Write(sendBuffer, 0, sendBuffer.Length);
            //mStream.WriteByte(0xff);
            //mStream.Flush();
        }

        public string Recv()
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");
            //byte[] sendBuffer = Encoding.UTF8.GetBytes(send);
            //mStream.WriteByte(0x00);
            //mStream.Write(sendBuffer, 0, sendBuffer.Length);
            //mStream.WriteByte(0xff);
            StringBuilder recvBuffer = new StringBuilder();

            BinaryReader reader = new BinaryReader(mStream);
            byte b = reader.ReadByte();
            if ((b & 0x80) == 0x80)
            {
                // Skip data frame
                int len = 0;
                do
                {
                    b = (byte)(reader.ReadByte() & 0x7f);
                    len += b * 128;
                } while ((b & 0x80) != 0x80);

                for (int i = 0; i < len; i++)
                    reader.ReadByte();
            }

            while (true)
            {
                b = reader.ReadByte();
                if (b == 0xff)
                    break;

                recvBuffer.Append(b);
            }

            return recvBuffer.ToString();
        }

        public void Close()
        {
            mStream.Dispose();
            mClient.Close();
            mStream = null;
            mClient = null;
        }

        private static TcpClient CreateSocket(Uri url)
        {
            string scheme = url.Scheme;
            string host = url.DnsSafeHost;

            int port = url.Port;
            if (port <= 0)
            {
                if (scheme.Equals("wss"))
                    port = 443;
                else if (scheme.Equals("ws"))
                    port = 80;
                else
                    throw new ArgumentException("Unsupported scheme");
            }

            if (scheme.Equals("wss"))
                throw new NotImplementedException("SSL support not implemented yet");
            else
                return new TcpClient(host, port);
        }
    }
}
