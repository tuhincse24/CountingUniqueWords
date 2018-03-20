using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CountingUniqueWords
{
    static class EntryClass
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"No arguments provided.");
                Console.WriteLine($"Please provide a directory name to scan text files as argument.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Word Counting starts...");
            var startAt = DateTime.Now;
            var root = new TrieNode(null, '?');
            var readers = new Dictionary<DataReader, Thread>();

            if (args.Length > 0)
            {
                var files = Directory.GetFiles(args.First(), "*.txt", SearchOption.AllDirectories);
                Parallel.ForEach(files, file =>
                    {
                        var newReader = new DataReader(file, ref root);
                        var newThread = new Thread(newReader.ThreadRun);
                        readers.Add(newReader, newThread);
                        newThread.Start();
                    }
                );
            }

            foreach (var t in readers.Values) t.Join();

            var stopedAt = DateTime.Now;
            Console.WriteLine($"Input data processed in {new TimeSpan(stopedAt.Ticks - startAt.Ticks).TotalSeconds} secs");
            Console.WriteLine();
            Console.WriteLine("Top 10 Most commonly found words:");

            var top10Nodes = new List<TrieNode> { root, root, root, root, root, root, root, root, root, root };
            var distinctWordCount = 0;
            var totalWordCount = 0;
            root.GetTopCounts(ref top10Nodes, ref distinctWordCount, ref totalWordCount);
            top10Nodes.Reverse();
            foreach (var node in top10Nodes)
            {
                Console.WriteLine($"{node} - {node.WordCount} times");
            }

            Console.WriteLine();
            Console.WriteLine($"Total {totalWordCount} words counted");
            Console.WriteLine($"{distinctWordCount} unique words found");
            Console.WriteLine();
            Console.WriteLine("Counting done.");
            Console.ReadLine();
        }
    }

    #region Input data reader

    public class DataReader
    {
        private readonly TrieNode _root;
        private readonly string _path;

        public DataReader(string path, ref TrieNode root)
        {
            _root = root;
            _path = path;
        }

        public void ThreadRun()
        {
            Parallel.ForEach(File.ReadLines(_path, Encoding.UTF8), line =>
            {
                string[] chunks = line.Split(null);
                foreach (var chunk in chunks)
                {
                    _root.AddWord(chunk.Trim());
                }
            });
        }
    }

    #endregion

    #region TRIE data structure implementation

    public class TrieNode : IComparable<TrieNode>
    {
        private readonly char _char;
        public int WordCount;
        private readonly TrieNode _parent;
        private readonly ConcurrentDictionary<char, TrieNode> _children;

        public TrieNode(TrieNode parent, char c)
        {
            _char = c;
            WordCount = 0;
            _parent = parent;
            _children = new ConcurrentDictionary<char, TrieNode>();
        }

        public void AddWord(string word, int index = 0)
        {
            if (index < word.Length)
            {
                var key = word[index];
                if (char.IsLetter(key))
                {
                    if (!_children.ContainsKey(key))
                    {
                        _children.TryAdd(key, new TrieNode(this, key));
                    }
                    _children[key].AddWord(word, index + 1);
                }
                else
                {
                    // not a letter! retry with next char
                    AddWord(word, index + 1);
                }
            }
            else
            {
                if (_parent == null) return;
                lock (this)
                {
                    WordCount++;
                }
            }
        }

        public int GetCount(string word, int index = 0)
        {
            if (index < word.Length)
            {
                var key = word[index];
                if (!_children.ContainsKey(key))
                {
                    return -1;
                }
                return _children[key].GetCount(word, index + 1);
            }
            else
            {
                return WordCount;
            }
        }

        public void GetTopCounts(ref List<TrieNode> mostCounted, ref int distinctWordCount, ref int totalWordCount)
        {
            if (WordCount > 0)
            {
                distinctWordCount++;
                totalWordCount += WordCount;
            }
            if (WordCount > mostCounted[0].WordCount)
            {
                mostCounted[0] = this;
                mostCounted.Sort();
            }
            foreach (var key in _children.Keys)
            {
                _children[key].GetTopCounts(ref mostCounted, ref distinctWordCount, ref totalWordCount);
            }
        }

        public override string ToString()
        {
            if (_parent == null) return "";
            else return _parent.ToString() + _char;
        }

        public int CompareTo(TrieNode other)
        {
            return WordCount.CompareTo(other.WordCount);
        }
    }

    #endregion
}