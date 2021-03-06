using System.Collections.Generic;
using Booster_WordStream.Controllers.WordStatistics;

namespace Booster_WordStream.Models
{
    /// <summary>
    /// Holds word collection data, and allows linking various word statistics to input data.
    /// </summary>
    public class WordCollectionData : IWordCollection
    {
        private int num_chars;
        private int num_words;

        private bool ignore_word_case = true;

        private Dictionary<char, int> frequency_char = new();
        private Dictionary<string, int> frequency_word = new();

        private HashSet<IWordStats> stat_types = new();

        /// <summary>
        /// Add a word statistic, to be maintainted when AddWord is called.
        /// </summary>
        /// <typeparam name="T">IWordStats</typeparam>
        public bool AddStat<T>(ref T word_stat)
            where T : IWordStats
        {
            // allow only one instance of a particular object
            if (!stat_types.Contains(word_stat))
            {
                word_stat.SetWordDict(ref frequency_word);
                stat_types.Add(word_stat);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a word statistic.
        /// </summary>
        /// <typeparam name="T">IWordStats</typeparam>
        public bool RemoveStat<T>(ref T word_stat)
            where T : IWordStats
        {
            if (stat_types.Contains(word_stat))
            {
                stat_types.Remove(word_stat);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a string to the collection, updating relevant statistics.
        /// </summary>
        public void AddString(string in_str)
        {
            // add char count
            num_chars += in_str.Length;

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

            if (ignore_word_case == true)
            {
                // convert words to lowercase
                in_word = in_word.ToLower();
            }

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

        /// <summary>
        /// Clear all word and character data.
        /// </summary>
        public void ClearData()
        {
            num_chars = 0;
            num_words = 0;

            frequency_char.Clear();
            frequency_word.Clear();

            foreach (var cur_stat in stat_types)
            {
                cur_stat.ClearData();
            }
        }

        /// <summary>
        /// Specify if word collection should be case sensitive.
        ///   - by default words are stored in lowercase
        /// </summary>
        public void SetIgnoreWordCase(bool new_val)
        {
            ignore_word_case = new_val;
        }

        // Getters
        public int GetNumChars() => this.num_chars;
        public int GetNumWords() => this.num_words;
        public Dictionary<char, int> GetCharFrequency() => this.frequency_char;
        public Dictionary<string, int> GetWordFrequency() => this.frequency_word;
    }
}
