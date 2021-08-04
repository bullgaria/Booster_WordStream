using System.Collections.Generic;
using System.Linq;

namespace Booster_WordStream.Controllers.WordStatistics
{
    public class FindLarge : IWordStats
    {
        private int num_words;
        private int max_words;

        private Dictionary<string, int> word_frequencies;
        private SortedSet<(int, string)> words_sorted = new();

        public FindLarge(int max_words)
        {
            this.max_words = max_words;
        }

        public void SetWordDict(ref Dictionary<string, int> word_dict)
        {
            this.word_frequencies = word_dict;
        }

        public IEnumerable<(int, string)> GetWords(bool descending = false)
        {
            if (descending)
            {
                return words_sorted.Reverse();
            }
            else
            {
                return words_sorted;
            }
        }

        public void AddWord(string word_in)
        {
            if (word_frequencies.TryGetValue(word_in, out var in_frequency))
            {
                // only update list for first occurance of a word
                if (in_frequency == 1) UpdateWords(word_in);
            }
        }

        public void ClearData()
        {
            num_words = 0;
            words_sorted.Clear();
        }

        /// <summary>
        /// Update the list of largest words with the new word.
        /// </summary>
        private void UpdateWords(string word_in)
        {
            int word_len = word_in.Length;

            // just add word if list is not full yet
            if (num_words < max_words)
            {
                num_words++;

                // add the new word
                words_sorted.Add((word_len, word_in));
            }
            else
            {
                // only do anything if larger than smallest word
                var old_word = words_sorted.First();
                if (word_len > old_word.Item1)
                {
                    // remove the old word
                    words_sorted.Remove(old_word);

                    // add the new word
                    words_sorted.Add((word_len, word_in));
                }
            }
        }
    }
}
