using System;
using System.Collections.Generic;
using System.Linq;

namespace TextAnalyzer
{
    //интерфейсы для разделения ответственности
    public interface ITextAnalyzer
    {
        TextAnalysisResult AnalyzeText(string text);
        Dictionary<char, int> AnalyzeCharacterFrequency(string text);
    }

    public interface ITextProcessor
    {
        void ProcessText(string text);
        void ProcessTextWithOptions(string text, TextProcessingOptions options);
    }

    public interface IHistoryManager
    {
        void AddEntry(string entry);
        IReadOnlyList<string> GetHistory();
        void ClearHistory();
    }

    public interface IStatisticsManager
    {
        void AddWordCount(int wordCount);
        IReadOnlyList<int> GetStatistics();
        void ClearStatistics();
    }

    //классы для конкретных реализаций
    public class WordCounter : ITextAnalyzer
    {
        public TextAnalysisResult AnalyzeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new TextAnalysisResult(0, "", 0);

            int wordCount = CountWords(text);
            string longestWord = FindLongestWord(text);
            int totalLength = CalculateTotalWordLength(text);

            return new TextAnalysisResult(wordCount, longestWord, totalLength);
        }

        public Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            var frequency = new Dictionary<char, int>();

            foreach (char currentChar in text)
            {
                if (!char.IsWhiteSpace(currentChar))
                {
                    frequency[currentChar] = frequency.ContainsKey(currentChar)
                        ? frequency[currentChar] + 1
                        : 1;
                }
            }

            return frequency;
        }

        private int CountWords(string text)
        {
            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private string FindLongestWord(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.OrderByDescending(word => word.Length).FirstOrDefault() ?? "";
        }

        private int CalculateTotalWordLength(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Sum(word => word.Length);
        }
    }

    public class AdvancedTextAnalyzer : ITextAnalyzer
    {
        private readonly char[] _advancedSeparators = { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' };

        public TextAnalysisResult AnalyzeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new TextAnalysisResult(0, "", 0);

            int wordCount = CountWordsByStateTracking(text);
            string longestWord = FindLongestWordWithSeparators(text);
            int totalLength = CalculateTotalWordLengthWithSeparators(text);

            return new TextAnalysisResult(wordCount, longestWord, totalLength);
        }

        public Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            var frequency = new Dictionary<char, int>();

            foreach (char currentChar in text)
            {
                if (char.IsLetterOrDigit(currentChar))
                {
                    frequency[currentChar] = frequency.ContainsKey(currentChar)
                        ? frequency[currentChar] + 1
                        : 1;
                }
            }

            return frequency;
        }

        private int CountWordsByStateTracking(string text)
        {
            int wordCount = 0;
            bool isInsideWord = false;

            foreach (char currentChar in text)
            {
                if (_advancedSeparators.Contains(currentChar))
                {
                    if (isInsideWord)
                    {
                        wordCount++;
                        isInsideWord = false;
                    }
                }
                else
                {
                    isInsideWord = true;
                }
            }

            if (isInsideWord) wordCount++;
            return wordCount;
        }

        private string FindLongestWordWithSeparators(string text)
        {
            var words = text.Split(_advancedSeparators, StringSplitOptions.RemoveEmptyEntries);
            return words.OrderByDescending(word => word.Length).FirstOrDefault() ?? "";
        }

        private int CalculateTotalWordLengthWithSeparators(string text)
        {
            var words = text.Split(_advancedSeparators, StringSplitOptions.RemoveEmptyEntries);
            return words.Sum(word => word.Length);
        }
    }

    public class HistoryManager : IHistoryManager
    {
        private const int MaxHistorySize = 50;
        private readonly List<string> _history = new List<string>();

        public void AddEntry(string entry)
        {
            if (_history.Count >= MaxHistorySize)
            {
                _history.RemoveAt(0);
            }
            _history.Add(entry);
        }

        public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();

        public void ClearHistory() => _history.Clear();
    }

    public class StatisticsManager : IStatisticsManager
    {
        private const int MaxStatisticsSize = 100;
        private readonly List<int> _statistics = new List<int>();

        public void AddWordCount(int wordCount)
        {
            if (_statistics.Count >= MaxStatisticsSize)
            {
                _statistics.RemoveAt(0);
            }
            _statistics.Add(wordCount);
        }

        public IReadOnlyList<int> GetStatistics() => _statistics.AsReadOnly();

        public void ClearStatistics() => _statistics.Clear();
    }

    //основной сервис, использующий зависимости через интерфейсы
    public class TextAnalysisService : ITextProcessor
    {
        private readonly ITextAnalyzer _textAnalyzer;
        private readonly IHistoryManager _historyManager;
        private readonly IStatisticsManager _statisticsManager;
        private readonly IResultFormatter _resultFormatter;

        public TextAnalysisService(
            ITextAnalyzer textAnalyzer,
            IHistoryManager historyManager,
            IStatisticsManager statisticsManager,
            IResultFormatter resultFormatter)
        {
            _textAnalyzer = textAnalyzer;
            _historyManager = historyManager;
            _statisticsManager = statisticsManager;
            _resultFormatter = resultFormatter;
        }

        public void ProcessText(string text)
        {
            var result = _textAnalyzer.AnalyzeText(text);

            _historyManager.AddEntry($"Слов: {result.WordCount}, Самое длинное слово: '{result.LongestWord}'");
            _statisticsManager.AddWordCount(result.WordCount);

            _resultFormatter.DisplayAnalysisResult(result);
        }

        public void ProcessTextWithOptions(string text, TextProcessingOptions options)
        {
            if (options.Analyze)
            {
                var result = _textAnalyzer.AnalyzeText(text);
                _resultFormatter.DisplayAnalysisResult(result);
            }

            if (options.SaveToHistory)
            {
                _historyManager.AddEntry($"Текст: {text.Substring(0, Math.Min(50, text.Length))}...");
            }

            if (options.CollectStatistics)
            {
                var result = _textAnalyzer.AnalyzeText(text);
                _statisticsManager.AddWordCount(result.WordCount);
            }
        }
    }

    //сервис для работы с частотой символов
    public class CharacterFrequencyService
    {
        private readonly ITextAnalyzer _textAnalyzer;
        private readonly IResultFormatter _resultFormatter;

        public CharacterFrequencyService(ITextAnalyzer textAnalyzer, IResultFormatter resultFormatter)
        {
            _textAnalyzer = textAnalyzer;
            _resultFormatter = resultFormatter;
        }

        public void AnalyzeAndDisplayFrequency(string text)
        {
            var frequency = _textAnalyzer.AnalyzeCharacterFrequency(text);
            _resultFormatter.DisplayCharacterFrequency(frequency);
        }

        public Dictionary<char, int> GetCharacterFrequency(string text)
        {
            return _textAnalyzer.AnalyzeCharacterFrequency(text);
        }
    }

    //интерфейс и реализации для форматирования результатов
    public interface IResultFormatter
    {
        void DisplayAnalysisResult(TextAnalysisResult result);
        void DisplayCharacterFrequency(Dictionary<char, int> frequency);
        void DisplayComparisonResult(TextComparisonResult result);
    }

    public class ConsoleResultFormatter : IResultFormatter
    {
        public void DisplayAnalysisResult(TextAnalysisResult result)
        {
            Console.WriteLine($"Анализ текста:");
            Console.WriteLine($"- Количество слов: {result.WordCount}");
            Console.WriteLine($"- Самое длинное слово: '{result.LongestWord}'");
            Console.WriteLine($"- Общая длина слов: {result.TotalWordLength}");
        }

        public void DisplayCharacterFrequency(Dictionary<char, int> frequency)
        {
            Console.WriteLine("Частотность символов:");
            foreach (var entry in frequency.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"  '{entry.Key}': {entry.Value} раз");
            }
        }

        public void DisplayComparisonResult(TextComparisonResult result)
        {
            Console.WriteLine($"Сравнение текстов:");
            Console.WriteLine($"- Разница в количестве слов: {result.WordCountDifference}");
            Console.WriteLine($"- Общий самый длинный слово: '{result.CommonLongestWord}'");
        }
    }

    //DTO классы для передачи данных
    public record TextAnalysisResult(int WordCount, string LongestWord, int TotalWordLength);

    public record TextComparisonResult(int WordCountDifference, string CommonLongestWord);

    public record TextProcessingOptions(
        bool Analyze = true,
        bool SaveToHistory = false,
        bool CollectStatistics = false,
        bool EnableLogging = false);

    //класс для создания анализаторов
    public static class TextAnalyzerFactory
    {
        public static ITextAnalyzer CreateSimpleAnalyzer() => new WordCounter();

        public static ITextAnalyzer CreateAdvancedAnalyzer() => new AdvancedTextAnalyzer();

        public static ITextAnalyzer CreateAnalyzer(bool useAdvanced = false)
            => useAdvanced ? CreateAdvancedAnalyzer() : CreateSimpleAnalyzer();
    }

    //класс для сравнения текстов
    public class TextComparator
    {
        private readonly ITextAnalyzer _textAnalyzer;

        public TextComparator(ITextAnalyzer textAnalyzer)
        {
            _textAnalyzer = textAnalyzer;
        }

        public TextComparisonResult CompareTexts(string text1, string text2)
        {
            var result1 = _textAnalyzer.AnalyzeText(text1);
            var result2 = _textAnalyzer.AnalyzeText(text2);

            int wordCountDifference = Math.Abs(result1.WordCount - result2.WordCount);
            string commonLongestWord = result1.LongestWord.Length >= result2.LongestWord.Length
                ? result1.LongestWord
                : result2.LongestWord;

            return new TextComparisonResult(wordCountDifference, commonLongestWord);
        }
    }

    //главный класс приложения
    internal class Program
    {
        public static void Main(string[] args)
        {
            //настройка зависимостей 
            var simpleAnalyzer = TextAnalyzerFactory.CreateSimpleAnalyzer();
            var historyManager = new HistoryManager();
            var statisticsManager = new StatisticsManager();
            var resultFormatter = new ConsoleResultFormatter();

            //создание сервисов
            var textAnalysisService = new TextAnalysisService(
                simpleAnalyzer,
                historyManager,
                statisticsManager,
                resultFormatter);

            var characterFrequencyService = new CharacterFrequencyService(simpleAnalyzer, resultFormatter);
            var textComparator = new TextComparator(simpleAnalyzer);

            //демонстрация работы
            var testText = "текст солид си шарп много букв я люблю буквы";

            Console.WriteLine("Базовый анализ текста");
            textAnalysisService.ProcessText(testText);

            Console.WriteLine("\nАнализ частотности символов");
            characterFrequencyService.AnalyzeAndDisplayFrequency(testText);

            Console.WriteLine("\nСравнение текстов");
            var text2 = "второй текст сравнение лето зима осень весна вторник среда октябрь";
            var comparisonResult = textComparator.CompareTexts(testText, text2);
            resultFormatter.DisplayComparisonResult(comparisonResult);

            Console.WriteLine("\nОбработка с опциями");
            var processingOptions = new TextProcessingOptions(
                Analyze: true,
                SaveToHistory: true,
                CollectStatistics: true);

            textAnalysisService.ProcessTextWithOptions(testText, processingOptions);

            Console.WriteLine("\nИстория операций");
            var history = historyManager.GetHistory();
            foreach (var entry in history)
            {
                Console.WriteLine($"- {entry}");
            }

            Console.WriteLine("\nСтатистика");
            var stats = statisticsManager.GetStatistics();
            Console.WriteLine($"Всего анализов: {stats.Count}");
            Console.WriteLine($"Среднее количество слов: {stats.Average():F2}");
        }
    }
}