using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace badCode
{
    public interface IStatisticsManager
    {
        void AddWordCount(int wordCount);
        IReadOnlyList<int> GetStatistics();
        void ClearStatistics();
    }
}
