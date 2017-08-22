using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Net.Sockets;

namespace SocksSharp.Helpers
{
    internal sealed class ReceiveHelper
    {
        #region Fields

        private const int InitialLineSize = 1000;

        private Stream stream;

        private byte[] buffer;
        private int bufferSize;

        private int linePosition;
        private byte[] lineBuffer = new byte[InitialLineSize];

        #endregion

        #region Properties

        public bool HasData
        {
            get
            {
                return (Length - Position) != 0;
            }
        }

        public int Length { get; private set; }

        public int Position { get; private set; }

        #endregion
        
        public ReceiveHelper( int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new byte[bufferSize];
        }

        #region Methods

        public void Init(Stream stream)
        {
            this.stream = stream;
            linePosition = 0;

            Length = 0;
            Position = 0;
        }

        public string ReadLine()
        {
            linePosition = 0;

            while (true)
            {
                if (Position == Length)
                {
                    Position = 0;
                    Length = stream.Read(buffer, 0, bufferSize);

                    if (Length == 0)
                    {
                        break;
                    }
                }

                byte b = buffer[Position++];

                lineBuffer[linePosition++] = b;

                // Если считан символ '\n'.
                if (b == 10)
                {
                    break;
                }

                // Если достигнут максимальный предел размера буфера линии.
                if (linePosition == lineBuffer.Length)
                {
                    // Увеличиваем размер буфера линии в два раза.
                    byte[] newLineBuffer = new byte[lineBuffer.Length * 2];

                    lineBuffer.CopyTo(newLineBuffer, 0);
                    lineBuffer = newLineBuffer;
                }
            }

            return Encoding.ASCII.GetString(lineBuffer, 0, linePosition);
        }

        public int Read(byte[] buffer, int index, int length)
        {
            int curLength = Length - Position;

            if (curLength > length)
            {
                curLength = length;
            }

            Array.Copy(this.buffer, Position, buffer, index, curLength);

            Position += curLength;

            return curLength;
        }
        
        #endregion
    }
}
