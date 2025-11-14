using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAnalyzer;

namespace badCode
{
    public interface ITextAnalyzer
    {
        TextAnalysisResult AnalyzeText(string text);
        Dictionary<char, int> AnalyzeCharacterFrequency(string text);
    }
}
