using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public static class WebExt
    {
        static UTF8Encoding _encoder = new UTF8Encoding();
        //static List<byte> _buf = new List<byte>();

        public static string GetParam(this Uri u, string par)
        {
            var q = u.Query;
            if (q.StartsWith("?")) q = q.Substring(1);
            var pp = q.SplitTable('&', '=');
            return pp.Find(x => x[0] == par)?[1];
        }

        public static async Task Send(this WebSocket ws, Event ev)
        {
            //CLog.Info($"Send: {msg.Count}b");
            var buffer = new byte[Event.IdSize];
            buffer[0] = (byte)(ev.Id / 256);
            buffer[1] = (byte)(ev.Id % 256);
            if (ev.Data != null)
            {
                var d = _encoder.GetBytes(ev.Data);
                buffer = buffer.Concat(d).ToArray();
            }
            var sendBuf = new ArraySegment<byte>(buffer);
            await ws.SendAsync(sendBuf, WebSocketMessageType.Text, true /* is last part of message */, CancellationToken.None);
        }

        public static async Task<Event> Receive(this WebSocket ws, int maxSize = 1 * 1024 * 1024)
        {
            // A read buffer, and a memory stream to stuff unknown number of chunks into:
            byte[] buf = new byte[4 * 1024];
            var ev = new Event();
            using (var ms = new MemoryStream())
            {
                ArraySegment<byte> arrayBuf = new ArraySegment<byte>(buf);
                if (ws.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult chunkResult;
                    do
                    {
                        chunkResult = await ws.ReceiveAsync(arrayBuf, CancellationToken.None);
                        if (chunkResult.Count < Event.IdSize)
                        {
                            CLog.Error("Message size == " + chunkResult.Count);
                            return ev;
                        }
                        var arr = arrayBuf.Array;
                        ms.Write(arr, arrayBuf.Offset + Event.IdSize, chunkResult.Count - Event.IdSize);
                        ev.Id = arr[0];
                        ev.Id *= 256;
                        ev.Id += arr[1];
                        CLog.Info($"Receive: {chunkResult.Count}b");
                        if (chunkResult.Count > maxSize)
                        {
                            CLog.Error("Message is bigger than expected: " + chunkResult.Count);
                        }
                    } while (!chunkResult.EndOfMessage);
                    ms.Seek(0, SeekOrigin.Begin);

                    // Looking for UTF-8 JSON type messages.
                    if (chunkResult.MessageType == WebSocketMessageType.Text)
                    {
                        var s = ms.StreamToString(Encoding.UTF8);
                        CLog.Info($"Receive: {s}");
                        ev.Data = s;
                        return ev;
                    }
                    else CLog.Error("Receive: Message is not string");
                }
            }
            return ev;
        }

        /// <summary>
        /// Converts memory stream into string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="ms">Memory Stream.</param>
        /// <param name="encoding">Encoding.</param>
        public static string StreamToString(this MemoryStream ms, Encoding encoding)
        {
            string readString = "";
            if (encoding == Encoding.UTF8)
            {
                using (var reader = new StreamReader(ms, encoding))
                {
                    readString = reader.ReadToEnd();
                }
            }
            return readString;
        }
    }

    public struct Event
    {
        public const int IdSize = 2;

        public short Id { get; set; }
        public string Data { get; set; }

        public Event(short id, string data)
        {
            Id = id;
            Data = data;
        }

        public override string ToString() => Id + ": " + Data?.SubstringFirst(30);
    }
}