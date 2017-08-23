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
using System.Collections.Generic;


namespace SocksSharp.Extensions
{
    internal static class StringExtensions
    {
        public static string Substring(this string str, string left,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (String.IsNullOrEmpty(left))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(left));
            }

            //if (startIndex < 0)
            //{
            //    throw new ArgumentException("Value cannot be less than zero", nameof(left));
            //}

            //if (startIndex >= str.Length)
            //{
            //    throw new ArgumentOutOfRangeException("startIndex",
            //        Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            //}

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            int length = str.Length - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        public static string Substring(this string str,
            string left, StringComparison comparsion = StringComparison.Ordinal)
        {
            return Substring(str, left, 0, comparsion);
        }

        public static string Substring(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            //if (left.Length == 0)
            //{
            //    throw ExceptionHelper.EmptyString("left");
            //}

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            //if (right.Length == 0)
            //{
            //    throw ExceptionHelper.EmptyString("right");
            //}

            //if (startIndex < 0)
            //{
            //    throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            //}

            //if (startIndex >= str.Length)
            //{
            //    throw new ArgumentOutOfRangeException("startIndex",
            //        Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            //}

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                return string.Empty;
            }

            // Вычисляем длину найденной подстроки.
            int length = rightPos - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        public static string Substring(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            return str.Substring(left, right, 0, comparsion);
        }

        public static string LastSubstring(this string str, string left,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            //if (left.Length == 0)
            //{
            //    throw ExceptionHelper.EmptyString("left");
            //}

            //if (startIndex < 0)
            //{
            //    throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            //}

            //if (startIndex >= str.Length)
            //{
            //    throw new ArgumentOutOfRangeException("startIndex",
            //        Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            //}

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Вычисляем длину найденной подстроки.
            int length = str.Length - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        public static string LastSubstring(this string str,
            string left, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return LastSubstring(str, left, str.Length - 1, comparsion);
        }

        public static string LastSubstring(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (left.Length == 0)
            {
                //throw ExceptionHelper.EmptyString("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            //if (right.Length == 0)
            //{
            //    throw ExceptionHelper.EmptyString("right");
            //}

            //if (startIndex < 0)
            //{
            //    throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            //}

            //if (startIndex >= str.Length)
            //{
            //    throw new ArgumentOutOfRangeException("startIndex",
            //        Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            //}

            #endregion

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.LastIndexOf(left, startIndex, comparsion);

            if (leftPosBegin == -1)
            {
                return string.Empty;
            }

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой подстроки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            if (rightPos == -1)
            {
                if (leftPosBegin == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return LastSubstring(str, left, right, leftPosBegin - 1, comparsion);
                }
            }

            // Вычисляем длину найденной подстроки.
            int length = rightPos - leftPosEnd;

            return str.Substring(leftPosEnd, length);
        }

        public static string LastSubstring(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return str.LastSubstring(left, right, str.Length - 1, comparsion);
        }

        public static string[] Substrings(this string str, string left, string right,
            int startIndex, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new string[0];
            }

            #region Проверка параметров

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (left.Length == 0)
            {
                //throw ExceptionHelper.EmptyString("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.Length == 0)
            {
               // throw ExceptionHelper.EmptyString("right");
            }

            if (startIndex < 0)
            {
               // throw ExceptionHelper.CanNotBeLess("startIndex", 0);
            }

            if (startIndex >= str.Length)
            {
                //throw new ArgumentOutOfRangeException("startIndex",
                    //Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            }

            #endregion

            int currentStartIndex = startIndex;
            List<string> strings = new List<string>();

            while (true)
            {
                // Ищем начало позиции левой подстроки.
                int leftPosBegin = str.IndexOf(left, currentStartIndex, comparsion);

                if (leftPosBegin == -1)
                {
                    break;
                }

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;

                // Ищем начало позиции правой строки.
                int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

                if (rightPos == -1)
                {
                    break;
                }

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;

                strings.Add(str.Substring(leftPosEnd, length));

                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }

            return strings.ToArray();
        }

        public static string[] Substrings(this string str, string left, string right,
            StringComparison comparsion = StringComparison.Ordinal)
        {
            return str.Substrings(left, right, 0, comparsion);
        }
    }
}
