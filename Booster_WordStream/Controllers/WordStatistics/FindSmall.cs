﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booster_WordStream.Controllers.WordStatistics
{
    public class FindSmall : IWordStats
    {
        private int num_words;
        private int max_words;

        private Dictionary<string, int> word_frequencies;
        private SortedSet<(int, string)> words_sorted = new();

        public FindSmall(int max_words)
        {
            this.max_words = max_words;
        }

        public void SetWordDict(ref Dictionary<string, int> word_dict)
        {
            this.word_frequencies = word_dict;
        }

        public Dictionary<string, int> GetWords()
        {
            return words_sorted.ToDictionary((x) => x.Item2, (x) => x.Item1);
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
        /// Update the list of smallest words with the new word.
        /// </summary>
        private void UpdateWords(string word_in)
        {
            int word_len = word_in.Count();

            // just add the word
            if (num_words < max_words)
            {
                num_words++;

                // add the new word
                words_sorted.Add((word_len, word_in));
            }
            else
            {
                // only do anything if smaller than largest word
                var last_set = words_sorted.Last();
                if (word_len < last_set.Item1)
                {
                    // remove the old word
                    words_sorted.Remove(last_set);

                    // add the new word
                    words_sorted.Add((word_len, word_in));
                }
            }
        }
    }
}