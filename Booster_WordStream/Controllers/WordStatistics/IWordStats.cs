using System.Collections.Generic;

namespace Booster_WordStream.Controllers.WordStatistics
{
    public interface IWordStats
    {
        void AddWord(string word_in);

        void SetWordDict(ref Dictionary<string, int> word_dict);

        Dictionary<string, int> GetWords();
    }
}
