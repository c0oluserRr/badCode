using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAnalyzer;

namespace badCode
{
    public interface ITextProcessor
    {
        void ProcessText(string text);
        void ProcessTextWithOptions(string text, TextProcessingOptions options);
    }
}
