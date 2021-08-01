﻿using System.Collections.Generic;
using System.Linq;
using Booster_WordStream.Controllers.WordStatistics;

namespace Booster_WordStream.Models
{
    /// <summary>
    /// Holds word collection data, and allows linking various word statistics to input data.
    /// </summary>
    public class WordCollectionData : IWordCollection
    {
        public int num_space { get; protected set; }
        public int num_chars { get; protected set; }
        public int num_words { get; protected set; }

        private Dictionary<string, int> frequency_word = new();
        private Dictionary<char, int> frequency_char = new();

        private List<IWordStats> stat_types = new();

        /// <summary>
        /// Add a word statistic, to be maintainted when AddWord is called.
        /// </summary>
        /// <typeparam name="T">IWordStats</typeparam>
        public void AddStat<T>(ref T word_stat)
            where T : IWordStats
        {
            word_stat.SetWordDict(ref frequency_word);
            stat_types.Add(word_stat);
        }

        /// <summary>
        /// Add a string to the collection, updating relevant statistics.
        /// </summary>
        public void AddString(string in_str)
        {
            // add char count
            num_chars += in_str.Count();
            num_space += in_str.Where((c) => c == ' ').Count();  // NOTE: is whitespace counted?

            foreach (var cur_char in in_str)
            {
                // add to frequency counter
                if (frequency_char.ContainsKey(cur_char))
                {
                    frequency_char[cur_char] += 1;
                }
                else
                {
                    frequency_char[cur_char] = 1;
                }
            }
        }

        /// <summary>
        /// Adds a single word to the collection and updating all word statistics.
        /// </summary>
        public void AddWord(string in_word)
        {
            // add word count
            num_words += 1;

            // add to frequency counter
            if (frequency_word.ContainsKey(in_word))
            {
                frequency_word[in_word] += 1;
            }
            else
            {
                frequency_word[in_word] = 1;
            }

            // maintain word statistics
            foreach (var cur_stat in stat_types)
            {
                cur_stat.AddWord(in_word);
            }
        }

        /// <summary>
        /// Adds a list of words to the collection.
        ///   - This is the equivalent to calling AddWord for each word in the list.
        /// </summary>
        public void AddWords(List<string> word_data)
        {
            foreach (var cur_word in word_data)
            {
                // add each word in list
                this.AddWord(cur_word);
            }
        }
    }
}