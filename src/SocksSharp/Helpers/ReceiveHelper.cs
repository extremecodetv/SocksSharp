/*
Copyright © 2012-2015 Ruslan Khuduev <x-rus@list.ru>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */

using System;
using System.IO;
using System.Text;

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
