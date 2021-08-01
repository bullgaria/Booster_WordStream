using System.Linq;
using Booster_WordStream.Models;
using Booster.CodingTest.Library;

namespace Booster_WordStream.Controllers
{
    public enum StreamState
    {
        Off = 0,
        Stopped = 1,
        Running = 2
    }

    public class WordStreamController
    {
        private static int BufferSize = 4096;

        private StreamState stream_state = StreamState.Off;

        public IWordCollection stream_data { get; init; }

        public WordStreamController(IWordCollection word_collection)
        {
            stream_data = word_collection;
        }

        /// <summary>
        /// Starts a new stream. New stream data will be added to old one, if any exists.
        /// </summary>
        public async void StartStream()
        {
            if (stream_state == StreamState.Running) return;

            using (var booster_stream = new WordStream())
            {
                stream_state = StreamState.Running;

                var buffer = new byte[BufferSize];
                string leftovers = null;

                while (stream_state == StreamState.Running)
                {
                    if (await booster_stream.ReadAsync(buffer, 0, buffer.Length) > 0)
                    {
                        leftovers = ProcessBuffer(buffer, leftovers);
                    }
                    else
                    {
                        // end of stream
                        //   - add any remaining leftovers to stream data and stop stream
                        if (!string.IsNullOrEmpty(leftovers)) stream_data.AddWord(leftovers);
                        stream_state = StreamState.Stopped;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the current stream.
        /// </summary>
        /// <returns>Returns true if the stream was successfully stopped.</returns>
        public bool StopStream()
        {
            // prevent multiple streams from running
            if (stream_state != StreamState.Running) return false;

            stream_state = StreamState.Stopped;
            return true;
        }

        /// <summary>
        /// Get the current stream state.
        /// </summary>
        public StreamState GetStreamState() => this.stream_state;

        /// <summary>
        /// Read in the buffer and add it to the stream data, splitting words on spaces.
        /// </summary>
        /// <param name="buffer_data">The current buffer data, all characters will be added to the stream data.</param>
        /// <param name="partial_word">Any leftover words, to which the incoming buffer data will be appended.</param>
        /// <returns>Any new leftover words.</returns>
        private string ProcessBuffer(byte[] buffer_data, string partial_word = null)
        {
            string buffer_str = System.Text.Encoding.UTF8.GetString(buffer_data);

            // process char statistics
            stream_data.AddString(buffer_str);

            // prefix words with partial word, if any
            buffer_str = (partial_word ?? string.Empty) + buffer_str;
            var word_data = buffer_str.Split(' ').ToList();

            // if the last character was not a space, this is probably a partial word - remove unless needed
            if (buffer_str.Last() != ' ')
            {
                partial_word = word_data.Last();
                word_data.RemoveAt(word_data.Count() - 1);
            }

            // process word statistics
            stream_data.AddWords(word_data);

            return partial_word;
        }
    }
}
