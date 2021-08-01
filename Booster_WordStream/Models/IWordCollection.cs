using Booster_WordStream.Controllers.WordStatistics;
using System.Collections.Generic;

namespace Booster_WordStream.Models
{
    public interface IWordCollection
    {
        public void AddStat<T>(ref T word_stat) where T : IWordStats;
        void AddString(string in_str);
        void AddWord(string in_word);
        void AddWords(List<string> word_data);

        void ClearData();

        int GetNumChars();
        int GetNumWords();
    }
}
