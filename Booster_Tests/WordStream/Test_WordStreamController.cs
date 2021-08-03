using Xunit;
using System.Linq;

namespace Booster_Tests.WordStream
{
    public class Test_WordStreamController
    {
        private Booster_WordStream.Controllers.WordStreamController<Booster_WordStream.Models.WordCollectionData> stream_controller = new();

        /// <summary>
        /// Starting and stopping the stream controller sets the state correctly.
        /// </summary>
        [Fact]
        public void StreamState_StartStop()
        {
            // initialized to Off
            Assert.Equal(Booster_WordStream.Controllers.StreamState.Off, stream_controller.GetStreamState());

            stream_controller.StartStream().ConfigureAwait(false);
            Assert.Equal(Booster_WordStream.Controllers.StreamState.Running, stream_controller.GetStreamState());

            stream_controller.StopStream();
            Assert.Equal(Booster_WordStream.Controllers.StreamState.Stopped, stream_controller.GetStreamState());

            stream_controller.StartStream().ConfigureAwait(false);
            Assert.Equal(Booster_WordStream.Controllers.StreamState.Running, stream_controller.GetStreamState());

            stream_controller.ResetStream();
            Assert.Equal(Booster_WordStream.Controllers.StreamState.Stopped, stream_controller.GetStreamState());
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
            Booster_WordStream.Controllers.WordStreamController.ProcessWords(buffer_str, out var leftovers, partial_word);

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
            var result = Booster_WordStream.Controllers.WordStreamController.ProcessWords(buffer_str, out var leftovers, partial_word);

            Assert.Equal(expected_result, result.First());
        }
    }
}
