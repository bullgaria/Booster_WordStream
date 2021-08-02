using System;
using System.Linq;
using System.Threading.Tasks;
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

    public class WordStreamController<T>
        where T: IWordCollection, new()
    {
        private static char[] WordSeparators = { ' ', '.', '\t', '\n', '\r' };
        private static int BufferSize = 4096;

        private int RefreshRate = 60;

        private StreamState stream_state = StreamState.Off;

        public T stream_data { get; } = new();

        public WordStreamController(int refresh_rate = 0)
        {
            if (refresh_rate > 0 && refresh_rate <= 1000) RefreshRate = refresh_rate;
        }

        /// <summary>
        /// Starts a new stream. New stream data will be added to old one, if any exists.
        /// </summary>
        public async Task StartStream()
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
                        // blazor WASM only runs on one thread, so a delay is needed to allow UI updates
                        await Task.Delay(1000 / RefreshRate);
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
            // stream is not running
            if (stream_state != StreamState.Running) return false;

            stream_state = StreamState.Stopped;
            return true;
        }

        /// <summary>
        /// Stop the stream and reset current stream data.
        /// </summary>
        /// <returns>Returns true if the stream was successfully stopped.</returns>
        public void ResetStream()
        {
            StopStream();
            stream_data.ClearData();
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
            var word_data = buffer_str.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

            // if the last character was not a separator, this is probably a partial word
            if (!WordSeparators.Contains(buffer_str.Last()))
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
