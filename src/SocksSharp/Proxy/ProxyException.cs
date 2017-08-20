using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocksSharp.Proxy
{
    public class ProxyException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ProxyException"/> заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке с объяснением причины исключения.</param>
        /// <param name="innerException">Исключение, вызвавшее текущие исключение, или значение <see langword="null"/>.</param>
        public ProxyException(string message, Exception innerException = null)
            : base(message, innerException) { }
        
    }
}
