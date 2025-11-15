using badCode;
using System;
using System.Collections.Generic;
using System.Linq;


namespace badCode
{
    public class WordCounter : ITextAnalyzer
    {
        private static readonly char[] DefaultSeparators =
            { ' ', '\t', '\n', '\r', ',', '.', ';', ':', '!', '?' };

        public TextAnalysisResult AnalyzeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new TextAnalysisResult(0, "", 0);

            var words = SplitWords(text);
            int count = words.Length;
            string longest = words.OrderByDescending(w => w.Length).FirstOrDefault() ?? "";
            int totalLen = words.Sum(w => w.Length);

            return new TextAnalysisResult(count, longest, totalLen);
        }

        public Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            var frequency = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (!char.IsWhiteSpace(c))
                {
                    char normalized = char.ToLowerInvariant(c);

                    if (frequency.ContainsKey(normalized))
                        frequency[normalized]++;
                    else
                        frequency[normalized] = 1;
                }
            }
            return frequency;
        }

        private static string[] SplitWords(string text) =>
            text.Split(DefaultSeparators, StringSplitOptions.RemoveEmptyEntries);
    }
    public class AdvancedTextAnalyzer : ITextAnalyzer
    {
        private readonly char[] _seps =
            { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' };

        public TextAnalysisResult AnalyzeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new TextAnalysisResult(0, "", 0);

            var words = text.Split(_seps, StringSplitOptions.RemoveEmptyEntries);

            return new TextAnalysisResult(
                words.Length,
                words.OrderByDescending(w => w.Length).FirstOrDefault() ?? "",
                words.Sum(w => w.Length)
            );
        }

        public Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            var dict = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                {
                    char norm = char.ToLowerInvariant(c);
                    dict[norm] = dict.ContainsKey(norm) ? dict[norm] + 1 : 1;
                }
            }
            return dict;
        }
    }
    public class HistoryManager : IHistoryManager
    {
        private const int Max = 50;
        private readonly List<string> _history = new();
        private readonly object _lock = new();

        public void AddEntry(string entry)
        {
            lock (_lock)
            {
                if (_history.Count >= Max)
                    _history.RemoveAt(0);

                _history.Add(entry);
            }
        }

        public IReadOnlyList<string> GetHistory()
        {
            lock (_lock)
                return _history.ToList().AsReadOnly();
        }

        public void ClearHistory()
        {
            lock (_lock)
                _history.Clear();
        }
    }
    public class StatisticsManager : IStatisticsManager
    {
        private const int Max = 100;
        private readonly List<int> _stats = new();
        private readonly object _lock = new();

        public void AddWordCount(int wordCount)
        {
            lock (_lock)
            {
                if (_stats.Count >= Max)
                    _stats.RemoveAt(0);

                _stats.Add(wordCount);
            }
        }

        public IReadOnlyList<int> GetStatistics()
        {
            lock (_lock)
                return _stats.ToList().AsReadOnly();
        }

        public void ClearStatistics()
        {
            lock (_lock)
                _stats.Clear();
        }
    }
    public class TextAnalysisService : ITextProcessor
    {
        private readonly ITextAnalyzer _analyzer;
        private readonly IHistoryManager _history;
        private readonly IStatisticsManager _stats;
        private readonly IResultFormatter _formatter;

        public TextAnalysisService(
            ITextAnalyzer analyzer,
            IHistoryManager history,
            IStatisticsManager stats,
            IResultFormatter formatter)
        {
            _analyzer = analyzer;
            _history = history;
            _stats = stats;
            _formatter = formatter;
        }

        public void ProcessText(string text)
        {
            var result = _analyzer.AnalyzeText(text);

            _history.AddEntry(
                $"Слов: {result.WordCount}, Длинное: '{result.LongestWord}'");

            _stats.AddWordCount(result.WordCount);
            _formatter.DisplayAnalysisResult(result);
        }

        public void ProcessTextWithOptions(string text, TextProcessingOptions opt)
        {
            TextAnalysisResult? result = null;

            if (opt.Analyze || opt.CollectStatistics)
                result = _analyzer.AnalyzeText(text);

            if (opt.Analyze && result != null)
                _formatter.DisplayAnalysisResult(result);

            if (opt.SaveToHistory)
            {
                string safe = text.Length <= 50 ? text : text[..50];
                _history.AddEntry($"Текст: {safe}");
            }

            if (opt.CollectStatistics && result != null)
                _stats.AddWordCount(result.WordCount);
        }
    }
    public class CharacterFrequencyService
    {
        private readonly ITextAnalyzer _analyzer;
        private readonly IResultFormatter _formatter;

        public CharacterFrequencyService(ITextAnalyzer analyzer, IResultFormatter formatter)
        {
            _analyzer = analyzer;
            _formatter = formatter;
        }

        public void AnalyzeAndDisplayFrequency(string text)
        {
            var freq = _analyzer.AnalyzeCharacterFrequency(text);
            _formatter.DisplayCharacterFrequency(freq);
        }

        public Dictionary<char, int> GetCharacterFrequency(string text) =>
            _analyzer.AnalyzeCharacterFrequency(text);
    }
    public class TextComparator
    {
        private readonly ITextAnalyzer _analyzer;

        public TextComparator(ITextAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public TextComparisonResult CompareTexts(string t1, string t2)
        {
            var r1 = _analyzer.AnalyzeText(t1);
            var r2 = _analyzer.AnalyzeText(t2);

            // поиск реально ОБЩЕГО длинного слова
            var set1 = new HashSet<string>(
                t1.Split(' ', ',', '.', '!', '?', ':', ';'),
                StringComparer.OrdinalIgnoreCase);

            var set2 = new HashSet<string>(
                t2.Split(' ', ',', '.', '!', '?', ':', ';'),
                StringComparer.OrdinalIgnoreCase);

            string common = set1.Intersect(set2)
                .OrderByDescending(w => w.Length)
                .FirstOrDefault() ?? "";

            return new TextComparisonResult(
                Math.Abs(r1.WordCount - r2.WordCount),
                common
            );
        }
    }

    public class ConsoleResultFormatter : IResultFormatter
    {
        public void DisplayAnalysisResult(TextAnalysisResult r)
        {
            Console.WriteLine("Анализ:");
            Console.WriteLine($"- Слова: {r.WordCount}");
            Console.WriteLine($"- Длинное: '{r.LongestWord}'");
            Console.WriteLine($"- Общая длина: {r.TotalWordLength}");
        }

        public void DisplayCharacterFrequency(Dictionary<char, int> freq)
        {
            Console.WriteLine("Частотность:");
            foreach (var p in freq.OrderByDescending(x => x.Value))
                Console.WriteLine($"'{p.Key}': {p.Value}");
        }

        public void DisplayComparisonResult(TextComparisonResult r)
        {
            Console.WriteLine("Сравнение:");
            Console.WriteLine($"- Разница слов: {r.WordCountDifference}");
            Console.WriteLine($"- Общее длинное слово: '{r.CommonLongestWord}'");
        }
    }
    public record TextAnalysisResult(int WordCount, string LongestWord, int TotalWordLength);
    public record TextComparisonResult(int WordCountDifference, string CommonLongestWord);

    public record TextProcessingOptions(
        bool Analyze = true,
        bool SaveToHistory = false,
        bool CollectStatistics = false,
        bool EnableLogging = false);

    internal class Program
    {
        public static void Main()
        {
            var analyzer = new WordCounter();
            var history = new HistoryManager();
            var stats = new StatisticsManager();
            var formatter = new ConsoleResultFormatter();

            var service = new TextAnalysisService(analyzer, history, stats, formatter);
            var freqService = new CharacterFrequencyService(analyzer, formatter);
            var comparator = new TextComparator(analyzer);

            string text1 = "кот,пес,";
            string text2 = "кот и пес гуляют";

            service.ProcessText(text1);
            freqService.AnalyzeAndDisplayFrequency(text1);
            formatter.DisplayComparisonResult(comparator.CompareTexts(text1, text2));
        }
    }
}