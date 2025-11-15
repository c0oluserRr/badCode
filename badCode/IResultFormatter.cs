using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace badCode
{
    public interface IResultFormatter
    {
        void DisplayAnalysisResult(TextAnalysisResult result);
        void DisplayCharacterFrequency(Dictionary<char, int> frequency);
        void DisplayComparisonResult(TextComparisonResult result);
    }
}
