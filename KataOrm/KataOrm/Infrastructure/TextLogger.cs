using System;
using System.IO;

namespace KataOrm.Infrastructure
{
    public class TextLogger : ILogger
    {
        private TextWriter _textWriterStore;

        public TextLogger(TextWriter textWriterStore)
        {
            _textWriterStore = textWriterStore;
        }

        public void Log(string messageToLog)
        {
            _textWriterStore.WriteLine(messageToLog);
        }
    }
}