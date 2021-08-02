using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booster_WordStream.Controllers.WordStatistics
{
    public class FindFrequent : IWordStats
    {
        private int num_words;
        private int max_words;

        private Dictionary<string, int> word_frequencies;
        private Dictionary<string, int> words_most_frequent = new();
        private SortedSet<(int, string)> words_sorted = new();

        public FindFrequent(int max_words)
        {
            this.max_words = max_words;
        }

        public void SetWordDict(ref Dictionary<string, int> word_dict)
        {
            this.word_frequencies = word_dict;
        }

        public Dictionary<string, int> GetWords(bool descending = false)
        {
            if (descending)
            {
                return words_sorted.Reverse().ToDictionary((x) => x.Item2, (x) => x.Item1);
            }
            else
            {
                return words_sorted.ToDictionary((x) => x.Item2, (x) => x.Item1);
            }
        }

        public void AddWord(string word_in)
        {
            UpdateWords(word_in);
        }

        public void ClearData()
        {
            num_words = 0;
            words_most_frequent.Clear();
            words_sorted.Clear();
        }

        /// <summary>
        /// Update the list of most frequent words with the new word.
        /// </summary>
        private void UpdateWords(string word_in)
        {
            word_frequencies.TryGetValue(word_in, out var cur_freq);

            // if already one of the most frequent words, just update that
            if (words_most_frequent.ContainsKey(word_in))
            {
                var old_freq = words_most_frequent[word_in];
                words_sorted.Remove((old_freq, word_in));
                words_sorted.Add((cur_freq, word_in));
                words_most_frequent[word_in] = cur_freq;
            }
            // just add word if list is not full yet
            else if (num_words < max_words)
            {
                num_words++;

                // add the new word
                words_sorted.Add((cur_freq, word_in));
                words_most_frequent[word_in] = cur_freq;
            }
            else
            {
                // only do anything if more frequent than least most-frequent word
                var old_word = words_sorted.First();
                if (cur_freq > old_word.Item1)
                {
                    // remove the old word
                    words_sorted.Remove(old_word);
                    words_most_frequent.Remove(old_word.Item2);

                    // add the new word
                    words_sorted.Add((cur_freq, word_in));
                    words_most_frequent[word_in] = cur_freq;
                }
            }

        }
    }
}
