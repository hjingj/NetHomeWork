using System;
using System.Linq;
using System.Text;

namespace NetHomeWork
{
    internal class Program
    {
        private static byte[] m_PacketData;
        private static uint m_Pos;

        private const string resultOutput = "0, 0, 0, 24, 0, 0, 0, 109, 66, 219, 250, 225, 0, 0, 0, 12, 0, 33, 0, 111, 0, 108, 0, 108, 0, 101, 0, 72";
        private const string resultReceived = "length: 24, age: 109, score: 109.99, message: Hello!";

        public static void Main(string[] args)
        {
            m_PacketData = new byte[1024];

            // 寫入資料 =========================
            Seek(0);

            // write dummy of length
            // TODO
            Write(1024);

            Write(109);
            Write(109.99f);
            Write("Hello!");

            WriteLength();

            var totalLength = Tell();
            var outputDate = new byte[totalLength];
            Buffer.BlockCopy(m_PacketData, 0, outputDate, 0, outputDate.Length);
            var output = string.Join(", ", outputDate);

            Console.WriteLine($"Output Byte array({totalLength}): {output}");

            if (output != resultOutput)
            {
                Console.WriteLine("Output Byte array Not Correct!!!");
                return;
            }

            // 讀出資料 =========================
            // seek to the head
            Seek(0);

            Read(out int length);
            Read(out int age);
            Read(out float score);
            Read(out string message);

            var received = $"length: {length}, age: {age}, score: {score}, message: {message}";

            Console.WriteLine($"Received Data: {received}");

            if (received != resultReceived)
            {
                Console.WriteLine("Received Date Not Correct!!!");
            }
            Console.ReadLine();
        }

        // =============================================================
        // Write Utilities

        // write an integer into a byte array
        private static bool WriteLength()
        {
            // TODO
            int nowIndex = (int)Tell();
            Seek(0);
            Write(nowIndex - 4);
            Seek((uint)nowIndex);

            return true;
        }

        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            _Write(bytes);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);

            // write byte array length to packet's byte array
            if (Write(bytes.Length) == false)
            {
                return false;
            }

            _Write(bytes);
            return true;
        }

        // =============================================================
        // Read Utilities

        // read an integer from packet's byte array
        private static bool Read(out int i)
        {
            // TODO
            byte[] readByte = new byte[4] { m_PacketData[m_Pos], m_PacketData[m_Pos + 1], m_PacketData[m_Pos + 2], m_PacketData[m_Pos + 3] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(readByte);
            }
            i = BitConverter.ToInt32(readByte, 0);
            m_Pos += 4;

            return true;
        }

        // read an float from packet's byte array
        private static bool Read(out float f)
        {
            // TODO
            byte[] readByte = new byte[sizeof(float)] { m_PacketData[m_Pos], m_PacketData[m_Pos + 1], m_PacketData[m_Pos + 2], m_PacketData[m_Pos + 3] };
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(readByte);
            }
            f = BitConverter.ToSingle(readByte, 0);
            m_Pos += 4;

            return true;
        }

        // read a string from packet's byte array
        private static bool Read(out string str)
        {
            // TODO
            Read(out int stringLengh);
            byte[] readByte = m_PacketData.Skip((int)m_Pos).Take(stringLengh).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(readByte);
            }
            str = Encoding.Unicode.GetString(readByte);
            m_Pos += (uint)stringLengh;

            return true;
        }

        // =============================================================
        // Raw Utilities

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }

            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
        }

        private static uint Tell()
        {
            return m_Pos;
        }

        private static void Seek(uint pos)
        {
            m_Pos = pos;
        }
    }
}
