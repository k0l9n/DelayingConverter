using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace converter.Converter
{
    public sealed class DelayingConverter : IConverter
    {
        public DelayingConverter()
        {
        }

        #region IConverter Members

        public string Convert(string input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (!int.TryParse(input, out var delay))
                delay = m_defaultDelay;
            Thread.Sleep(delay);

            return $"I have slept for {delay}ms, because you told me: {input}";
        }

        #endregion

        private const int m_defaultDelay = 250;

    }
    public interface IConverter
    {
        string Convert(string input);

    }
}
