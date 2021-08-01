using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booster_WordStream.Controllers.WordStatistics
{
    public class FindLarge : IWordStats
    {
        private int num_words;
        private int max_words;

        private Dictionary<string, int> word_frequencies;
        private SortedDictionary<int, SortedSet<string>> words_sorted = new();

        public FindLarge(int max_words)
        {
            this.max_words = max_words;
        }

        public void SetWordDict(ref Dictionary<string, int> word_dict)
        {
            this.word_frequencies = word_dict;
        }

        public Dictionary<string, int> GetWords()
        {
            var out_dict = new Dictionary<string, int>();

            foreach (var cur_data in words_sorted.Reverse())
            {
                foreach (var cur_word in cur_data.Value)
                {
                    out_dict[cur_word] = cur_data.Key;
                }
            }

            return out_dict;
        }

        public void AddWord(string word_in)
        {
            if (word_frequencies.TryGetValue(word_in, out var in_frequency))
            {
                // only update list for first occurance of a word
                if (in_frequency == 1) UpdateWords(word_in);
            }
        }

        /// <summary>
        /// Update the list of largest words with the new word.
        /// </summary>
        private void UpdateWords(string word_in)
        {
            int word_len = word_in.Count();

            // just add to the dictionary
            if (num_words < max_words)
            {
                num_words++;

                // add the new word
                if (words_sorted.ContainsKey(word_len))
                {
                    words_sorted[word_len].Add(word_in);
                }
                else
                {
                    words_sorted[word_len] = new() { word_in };
                }
            }
            else
            {
                var first_len = words_sorted.First().Key;

                // only do anything if larger than smallest word
                if (word_len > first_len)
                {
                    // remove the first word of the first set
                    var first_set = words_sorted.First().Value;
                    if (first_set.Count() <= 1)
                    {
                        // remove what will be an empty set
                        words_sorted.Remove(first_len);
                    }
                    else
                    {
                        // remove the word
                        first_set.Remove(first_set.First());
                    }

                    // add the new word
                    if (words_sorted.ContainsKey(word_len))
                    {
                        words_sorted[word_len].Add(word_in);
                    }
                    else
                    {
                        words_sorted[word_len] = new() { word_in };
                    }
                }
            }
        }
    }
}
