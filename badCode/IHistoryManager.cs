using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace badCode
{
    public interface IHistoryManager
    {
        void AddEntry(string entry);
        IReadOnlyList<string> GetHistory();
        void ClearHistory();
    }
}
