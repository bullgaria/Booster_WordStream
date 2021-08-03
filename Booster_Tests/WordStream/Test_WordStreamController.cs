using Xunit;
using System.Linq;
using Booster_WordStream.Models;
using Booster_WordStream.Controllers;

namespace Booster_Tests.WordStream
{
    public class Test_WordStreamController
    {
        private WordStreamController<WordCollectionData> stream_controller = new();

        /// <summary>
        /// Starting and stopping the stream controller sets the state correctly.
        /// </summary>
        [Fact]
        public void StreamState_StartStop()
        {
            // initialized to Off
            Assert.Equal(StreamState.Off, stream_controller.GetStreamState());

            stream_controller.StartStream().ConfigureAwait(false);
            Assert.Equal(StreamState.Running, stream_controller.GetStreamState());

            stream_controller.StopStream();
            Assert.Equal(StreamState.Stopped, stream_controller.GetStreamState());

            stream_controller.StartStream().ConfigureAwait(false);
            Assert.Equal(StreamState.Running, stream_controller.GetStreamState());

            stream_controller.ResetStream();
            Assert.Equal(StreamState.Stopped, stream_controller.GetStreamState());
        }

        /// <summary>
        /// Leftover strings are correctly parsed.
        /// </summary>
        [Theory]
        [InlineData("aa bb cc dd", "dd")]
        [InlineData("aa bb cc dd", "dd", "ee")]
        [InlineData("aa bb cc dd ", null)]
        [InlineData("aa bb cc dd ", null, "ee")]
        public void ProcessWords_Leftovers(string buffer_str, string expected_result, string partial_word = null)
        {
            WordStreamController.ProcessWords(buffer_str, out var leftovers, partial_word);

            Assert.Equal(expected_result, leftovers);
        }

        /// <summary>
        /// The first word is properly read, accounting for partial words.
        /// </summary>
        [Theory]
        [InlineData("aa bb cc dd", "aa")]
        [InlineData("aa bb cc dd", "eeaa", "ee")]
        [InlineData(" aa bb cc dd", "aa")]
        [InlineData(" aa bb cc dd", "ee", "ee")]
        public void ProcessWords_FirstWord(string buffer_str, string expected_result, string partial_word = null)
        {
            var result = WordStreamController.ProcessWords(buffer_str, out _, partial_word);

            Assert.Equal(expected_result, result.First());
        }
    }
}
