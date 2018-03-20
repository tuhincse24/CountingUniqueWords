//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CountingUniqueWords
//{
//    class Program
//    {

//        static void Main(string[] args)
//        {
//            var watch = System.Diagnostics.Stopwatch.StartNew();
//            IWordCounter wordCounter = new ParallelWordCounter();
//            IDictionary<string, int> uniqueWords = null;
//            //int totalWords = 0;
//            for (int i = 0; i < 20; i++)
//            {
//                uniqueWords = wordCounter.CountWords(@"F:\Tuhin\WordCount\wap.txt");
//                //totalWords = uniqueWords.Values.Sum();
//            }
//            //var maxCount = uniqueWords.Values.Max();
//            //var maxOccurence = uniqueWords.First(w=>w.Value== maxCount);
//            watch.Stop();
//            var elapsedMs = watch.ElapsedMilliseconds;
//            Console.WriteLine($"Total word count: {uniqueWords.Count}; and time take to count is: {elapsedMs} ms");
//            // Wait for the user to press a key before exiting
//            Console.ReadKey();

//        } // End of Main method
//        }
//    interface IWordCounter
//    {
//        IDictionary<string, int> CountWords(string path);
//    }
//    class ParallelWordCounter : IWordCounter
//    {
//        public IDictionary<string, int> CountWords(string path)
//        {
//            var result = new ConcurrentDictionary<string, int>();
//            Parallel.ForEach(File.ReadLines(path, Encoding.UTF8), line =>
//            {
//                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                foreach (var word in words)
//                {
//                    result.AddOrUpdate(word, 1, (_, x) => x + 1);
//                }
//            });

//            return result;
//        }
//    }
//}
