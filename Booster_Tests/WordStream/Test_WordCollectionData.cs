using System.Collections.Generic;
using Xunit;

namespace Booster_Tests.WordStream
{
    public class Test_WordCollectionData
    {
        private Booster_WordStream.Models.WordCollectionData word_collection = new();

        /// <summary>
        /// ClearData functions properly resets all data.
        /// </summary>
        [Fact]
        public void Collection_ClearData()
        {
            word_collection.ClearData();

            // reset to empty
            Assert.Equal(0, word_collection.GetNumChars());
            Assert.Equal(0, word_collection.GetNumWords());
            Assert.Empty(word_collection.GetCharFrequency());
            Assert.Empty(word_collection.GetWordFrequency());
        }

        /// <summary>
        /// Check that case is properly ignored (or not) depending on the setting.
        /// </summary>
        [Fact]
        public void Collection_IgnoreCase()
        {
            // set initial state
            word_collection.ClearData();
            word_collection.SetIgnoreCase(true);

            // first word
            word_collection.AddWord("this");
            Assert.Single(word_collection.GetWordFrequency());

            // duplicate word
            word_collection.AddWord("this");
            Assert.Single(word_collection.GetWordFrequency());

            // duplicate word, but different case
            word_collection.AddWord("THIS");
            Assert.Single(word_collection.GetWordFrequency());

            // words with the same case should be counted now
            word_collection.SetIgnoreCase(false);

            // second word
            word_collection.AddWord("that");
            Assert.Equal(2, word_collection.GetWordFrequency().Count);

            // duplicate word
            word_collection.AddWord("that");
            Assert.Equal(2, word_collection.GetWordFrequency().Count);

            // duplicate word, but different case
            word_collection.AddWord("THAT");
            Assert.Equal(3, word_collection.GetWordFrequency().Count);
        }

        /// <summary>
        /// AddWord (and AddWords) adds the expected number of words.
        /// </summary>
        [Fact]
        public void Collection_AddWords()
        {
            // set initial state
            word_collection.ClearData();
            int expected_num = 0;

            word_collection.AddWord("test");
            expected_num += 1;
            Assert.Equal(expected_num, word_collection.GetNumWords());

            word_collection.AddWord("test test");
            expected_num += 1;  // AddWord does not take separators into account
            Assert.Equal(expected_num, word_collection.GetNumWords());

            word_collection.AddWords(new() { "test" });
            expected_num += 1;
            Assert.Equal(expected_num, word_collection.GetNumWords());

            word_collection.AddWords(new() { "test test" });
            expected_num += 1;  // AddWord does not take separators into account
            Assert.Equal(expected_num, word_collection.GetNumWords());

            word_collection.AddWords(new() { "test", "test" });
            expected_num += 2;
            Assert.Equal(expected_num, word_collection.GetNumWords());
        }

        /// <summary>
        /// AddString adds the expected number of characters.
        /// </summary>
        [Fact]
        public void Collection_AddString()
        {
            // set initial state
            word_collection.ClearData();
            int expected_num = 0;

            word_collection.AddString("1");
            expected_num += 1;
            Assert.Equal(expected_num, word_collection.GetNumChars());

            word_collection.AddString("1 3");
            expected_num += 3;
            Assert.Equal(expected_num, word_collection.GetNumChars());

            word_collection.AddString("1 34");
            expected_num += 4;
            Assert.Equal(expected_num, word_collection.GetNumChars());
        }
    }
}
