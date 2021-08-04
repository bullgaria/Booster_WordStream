using Xunit;
using System.Collections.Generic;
using System.Linq;
using Booster_WordStream.Controllers.WordStatistics;

namespace Booster_Tests.WordStream
{
    public class Test_WordStatistics
    {
        /// <summary>
        /// Test the FindFrequent class.
        /// </summary>
        [Theory]
        [InlineData(new[] { "a" }, 1, "a", "a")]  // a
        [InlineData(new[] { "a", "b" }, 2, "a", "b")]  // (a)-> a (b)-> a, b
        [InlineData(new[] { "a", "b", "c" }, 3, "a", "c")]  // (a)-> a (b)-> a, b (c)-> a, b, c
        [InlineData(new[] { "a", "b", "c", "d" }, 3, "a", "c")]  // (a)-> a (b)-> a, b (c)-> a, b, c (d)-> a, b, c
        [InlineData(new[] { "a", "b", "c", "d", "d" }, 3, "b", "d")]  // (a)-> a (b)-> a, b (c)-> a, b, c (d)-> a, b, c (d)-> a, b, d
        [InlineData(new[] { "a", "b", "c", "d", "d", "c" }, 3, "b", "d")]  // (a)-> a (b)-> a, b (c)-> a, b, c (d)-> a, b, c (d)-> a, b, d (c)-> b, c, d
        [InlineData(new[] { "a", "b", "c", "d", "d", "c", "b", "b" }, 3, "c", "b")]  // (a)-> a (b)-> a, b (c)-> a, b, c (d)-> a, b, c (d)-> a, b, d (c)-> b, c, d (b)-> b, c, d (b)-> c, d, b
        public void WordStat_FindFrequent(string[] word_arr, int expected_num, string expected_first, string expected_last)
        {
            Dictionary<string, int> word_dict = new();

            // set initial state
            var word_stat = new FindFrequent(3);
            word_stat.SetWordDict(ref word_dict);

            // add all words
            foreach (var cur_word in word_arr)
            {
                // update word dictionary
                if (word_dict.ContainsKey(cur_word))
                {
                    word_dict[cur_word] += 1;
                }
                else
                {
                    word_dict[cur_word] = 1;
                }

                // add to stat
                word_stat.AddWord(cur_word);
            }

            // ascending
            var result_words_asc = word_stat.GetWords(false);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_asc.Count());
            // check that the first and last word are correct
            Assert.Equal(expected_first, result_words_asc.First().Item2);
            Assert.Equal(expected_last, result_words_asc.Last().Item2);

            // descending
            var result_words_desc = word_stat.GetWords(true);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_desc.Count());
            // check that the first and last word are correct (first and last are reversed)
            Assert.Equal(expected_first, result_words_desc.Last().Item2);
            Assert.Equal(expected_last, result_words_desc.First().Item2);
        }

        /// <summary>
        /// Test the FindLarge class.
        /// </summary>
        [Theory]
        [InlineData(new[] { "a" }, 1, "a", "a")]
        [InlineData(new[] { "a", "b" }, 2, "a", "b")]
        [InlineData(new[] { "a", "b", "c" }, 3, "a", "c")]
        [InlineData(new[] { "a", "b", "c", "d" }, 3, "a", "c")]
        [InlineData(new[] { "1234", "2345", "345", "45" }, 3, "345", "2345")]
        [InlineData(new[] { "123", "123", "1", "1", "2" }, 3, "1", "123")]
        [InlineData(new[] { "123", "123", "1", "1", "2", "1234" }, 3, "2", "1234")]
        [InlineData(new[] { "123", "123", "1", "1", "2", "1234", "12345" }, 3, "123", "12345")]
        public void WordStat_FindLarge(string[] word_arr, int expected_num, string expected_first, string expected_last)
        {
            Dictionary<string, int> word_dict = new();

            // set initial state
            var word_stat = new FindLarge(3);
            word_stat.SetWordDict(ref word_dict);

            // add all words
            foreach (var cur_word in word_arr)
            {
                // update word dictionary
                if (word_dict.ContainsKey(cur_word))
                {
                    word_dict[cur_word] += 1;
                }
                else
                {
                    word_dict[cur_word] = 1;
                }

                // add to stat
                word_stat.AddWord(cur_word);
            }

            // ascending
            var result_words_asc = word_stat.GetWords(false);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_asc.Count());
            // check that the first and last word are correct
            Assert.Equal(expected_first, result_words_asc.First().Item2);
            Assert.Equal(expected_last, result_words_asc.Last().Item2);

            // descending
            var result_words_desc = word_stat.GetWords(true);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_desc.Count());
            // check that the first and last word are correct (first and last are reversed)
            Assert.Equal(expected_first, result_words_desc.Last().Item2);
            Assert.Equal(expected_last, result_words_desc.First().Item2);
        }

        /// <summary>
        /// Test the FindSmall class.
        /// </summary>
        [Theory]
        [InlineData(new[] { "a" }, 1, "a", "a")]
        [InlineData(new[] { "a", "b" }, 2, "a", "b")]
        [InlineData(new[] { "a", "b", "c" }, 3, "a", "c")]
        [InlineData(new[] { "a", "b", "c", "d" }, 3, "a", "c")]
        [InlineData(new[] { "1234", "2345", "345", "45" }, 3, "45", "1234")]
        [InlineData(new[] { "123", "123", "1", "1", "2" }, 3, "1", "123")]
        public void WordStat_FindSmall(string[] word_arr, int expected_num, string expected_first, string expected_last)
        {
            Dictionary<string, int> word_dict = new();

            // set initial state
            var word_stat = new FindSmall(3);
            word_stat.SetWordDict(ref word_dict);

            // add all words
            foreach (var cur_word in word_arr)
            {
                // update word dictionary
                if (word_dict.ContainsKey(cur_word))
                {
                    word_dict[cur_word] += 1;
                }
                else
                {
                    word_dict[cur_word] = 1;
                }

                // add to stat
                word_stat.AddWord(cur_word);
            }

            // ascending
            var result_words_asc = word_stat.GetWords(false);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_asc.Count());
            // check that the first and last word are correct
            Assert.Equal(expected_first, result_words_asc.First().Item2);
            Assert.Equal(expected_last, result_words_asc.Last().Item2);

            // descending
            var result_words_desc = word_stat.GetWords(true);

            // check that the number of words is correct
            Assert.Equal(expected_num, result_words_desc.Count());
            // check that the first and last word are correct (first and last are reversed)
            Assert.Equal(expected_first, result_words_desc.Last().Item2);
            Assert.Equal(expected_last, result_words_desc.First().Item2);
        }
    }
}
