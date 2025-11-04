using System;
using System.Collections.Generic;
using System.Linq;

namespace TextAnalyzer
{
    // Основной класс для анализа текста
    public class TextAnalyzer
    {
        // Константы для замены магических чисел
        private const int MaxStatisticsSize = 100;
        private const int MaxHistorySize = 50;
        private const int MinOutputType = 1;
        private const int MaxOutputType = 5;
        private const int DefaultOutputType = 3;

        // Массив для хранения статистики по количеству слов в предыдущих анализах
        private int[] _wordCountStatistics = new int[MaxStatisticsSize];
        // Список для хранения истории операций анализа
        private List<string> _analysisHistory = new List<string>();

        // Константы для типов анализа
        private const int AnalysisTypeWordCount = 1;
        private const int AnalysisTypeLongestWord = 2;
        private const int AnalysisTypeCharFrequency = 3;
        private const int AnalysisTypeTextLength = 4;
        private const int AnalysisTypeTotalWordLength = 5;

        // Основной метод анализа текста
        public void AnalyzeTextComprehensive(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int wordCount = CountWordsByStateTracking(text);
            string longestWord = FindLongestWord(text);
            Dictionary<char, int> characterFrequency = AnalyzeCharacterFrequency(text);

            UpdateAnalysisHistory(wordCount, longestWord);
            UpdateWordCountStatistics(wordCount);

            DisplayComprehensiveResults(wordCount, longestWord);
        }

        // Метод для обработки текста с различными вариантами вывода
        public void AnalyzeTextWithOutputOptions(string text, int outputType)
        {
            int validatedOutputType = ValidateOutputType(outputType);
            string longestWord = FindLongestWord(text);
            int wordCount = CountWordsManual(text);

            DisplayAnalysisResults(wordCount, longestWord, validatedOutputType, text);
        }

        // Универсальный метод анализа с возвратом результата
        public object AnalyzeText(string text, int analysisType)
        {
            return analysisType switch
            {
                AnalysisTypeWordCount => CountWordsSimple(text),
                AnalysisTypeLongestWord => FindLongestWordSimple(text),
                AnalysisTypeCharFrequency => AnalyzeCharacterFrequency(text),
                AnalysisTypeTextLength => text.Length,
                AnalysisTypeTotalWordLength => CalculateTotalWordLength(text),
                _ => null
            };
        }

        // Вспомогательный метод для красивого вывода словаря с частотностью символов
        public void PrintCharacterFrequency(Dictionary<char, int> characterFrequency)
        {
            if (characterFrequency == null)
                return;

            foreach (var characterEntry in characterFrequency)
            {
                Console.WriteLine($"Символ '{characterEntry.Key}': {characterEntry.Value} раз");
            }
        }

        #region Методы подсчета слов

        private int CountWordsByStateTracking(string text)
        {
            int wordCount = 0;
            bool isInsideWord = false;

            for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                char currentChar = text[currentIndex];

                if (IsWordSeparator(currentChar))
                {
                    HandleWordSeparator(ref isInsideWord, ref wordCount);
                }
                else
                {
                    isInsideWord = true;
                }
            }

            AddLastWordIfNeeded(ref isInsideWord, ref wordCount);
            return wordCount;
        }

        private void HandleWordSeparator(ref bool isInsideWord, ref int wordCount)
        {
            if (isInsideWord)
            {
                wordCount++;
                isInsideWord = false;
            }
        }

        private void AddLastWordIfNeeded(ref bool isInsideWord, ref int wordCount)
        {
            if (isInsideWord)
                wordCount++;
        }

        private int CountWordsManual(string text)
        {
            int wordCount = 0;
            int currentIndex = 0;

            while (currentIndex < text.Length)
            {
                SkipWhitespaceCharacters(text, ref currentIndex);
                IncrementWordCountIfValid(text, ref currentIndex, ref wordCount);
                SkipWordCharacters(text, ref currentIndex);
            }

            return wordCount;
        }

        private void SkipWhitespaceCharacters(string text, ref int currentIndex)
        {
            while (currentIndex < text.Length && char.IsWhiteSpace(text[currentIndex]))
                currentIndex++;
        }

        private void IncrementWordCountIfValid(string text, ref int currentIndex, ref int wordCount)
        {
            if (currentIndex < text.Length)
                wordCount++;
        }

        private void SkipWordCharacters(string text, ref int currentIndex)
        {
            while (currentIndex < text.Length && !char.IsWhiteSpace(text[currentIndex]))
                currentIndex++;
        }

        private int CountWordsSimple(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        #endregion

        #region Методы поиска самого длинного слова

        private string FindLongestWord(string text)
        {
            string[] words = SplitTextIntoWords(text);
            return FindLongestWordInArray(words);
        }

        private string[] SplitTextIntoWords(string text)
        {
            char[] wordSeparators = { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' };
            return text.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        private string FindLongestWordInArray(string[] words)
        {
            string longestWord = "";

            foreach (string word in words)
            {
                if (word.Length > longestWord.Length)
                {
                    longestWord = word;
                }
            }

            return longestWord;
        }

        private string FindLongestWordSimple(string text)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.OrderByDescending(word => word.Length).FirstOrDefault() ?? "";
        }

        #endregion

        #region Методы анализа символов

        private Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            Dictionary<char, int> frequency = new Dictionary<char, int>();

            foreach (char currentChar in text)
            {
                if (!char.IsWhiteSpace(currentChar))
                {
                    UpdateCharacterFrequency(frequency, currentChar);
                }
            }

            return frequency;
        }

        private void UpdateCharacterFrequency(Dictionary<char, int> frequency, char character)
        {
            if (frequency.ContainsKey(character))
            {
                frequency[character]++;
            }
            else
            {
                frequency[character] = 1;
            }
        }

        private int CalculateTotalWordLength(string text)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Sum(word => word.Length);
        }

        #endregion

        #region Методы управления данными и историей

        private void UpdateAnalysisHistory(int wordCount, string longestWord)
        {
            ManageHistorySize();
            AddHistoryEntry(wordCount, longestWord);
        }

        private void ManageHistorySize()
        {
            if (_analysisHistory.Count >= MaxHistorySize)
            {
                _analysisHistory.RemoveAt(0);
            }
        }

        private void AddHistoryEntry(int wordCount, string longestWord)
        {
            _analysisHistory.Add($"Слов: {wordCount}, Самое длинное: '{longestWord}'");
        }

        private void UpdateWordCountStatistics(int wordCount)
        {
            for (int i = 0; i < _wordCountStatistics.Length; i++)
            {
                if (_wordCountStatistics[i] == 0)
                {
                    _wordCountStatistics[i] = wordCount;
                    break;
                }
            }
        }

        #endregion

        #region Методы вывода результатов

        private void DisplayComprehensiveResults(int wordCount, string longestWord)
        {
            Console.WriteLine($"Всего слов: {wordCount}, Самое длинное слово: '{longestWord}'");
        }

        private void DisplayAnalysisResults(int wordCount, string longestWord, int outputType, string text)
        {
            switch (outputType)
            {
                case 1:
                    DisplayWordCountOnly(wordCount);
                    break;
                case 2:
                    DisplayLongestWordOnly(longestWord);
                    break;
                case 3:
                    DisplayBothMetrics(wordCount, longestWord);
                    break;
                case 4:
                    DisplayWithCharacterCount(wordCount, text);
                    break;
                case 5:
                    DisplayAlternativeFormat(wordCount, longestWord);
                    break;
            }
        }

        private void DisplayWordCountOnly(int wordCount)
        {
            Console.WriteLine($"Количество слов: {wordCount}");
        }

        private void DisplayLongestWordOnly(string longestWord)
        {
            Console.WriteLine($"Самое длинное слово: '{longestWord}'");
        }

        private void DisplayBothMetrics(int wordCount, string longestWord)
        {
            Console.WriteLine($"Слов: {wordCount}, Самое длинное: '{longestWord}'");
        }

        private void DisplayWithCharacterCount(int wordCount, string text)
        {
            Dictionary<char, int> characterFrequency = AnalyzeCharacterFrequency(text);
            Console.WriteLine($"Слов: {wordCount}, Уникальных символов: {characterFrequency.Count}");
        }

        private void DisplayAlternativeFormat(int wordCount, string longestWord)
        {
            Console.WriteLine($"Результат анализа: {wordCount} слов, самое длинное: '{longestWord}'");
        }

        #endregion

        #region Вспомогательные методы

        private bool IsWordSeparator(char character)
        {
            char[] separators = { ' ', '\t', '\n', '\r', '.', ',' };
            return separators.Contains(character);
        }

        private int ValidateOutputType(int outputType)
        {
            return (outputType < MinOutputType || outputType > MaxOutputType) ? DefaultOutputType : outputType;
        }

        #endregion
    }

    // Класс для обработки и хранения текстовых данных
    public class TextProcessor
    {
        // Константы для ограничений хранилища
        private const int MaxDataSize = 100;
        private const int StatsPrintInterval = 10;
        private const int CacheCleanupThreshold = 50;
        private const int CacheCleanupCount = 10;

        private List<string> _processedTexts = new List<string>();
        private int _processedCount = 0;

        // Метод обработки текста с сохранением в историю
        public void ProcessAndStoreText(string text)
        {
            TextAnalysisResult analysisResult = AnalyzeText(text);
            StoreTextData(text);
            DisplayProcessingResults(analysisResult);
            PerformStorageMaintenance();
        }

        // Универсальный метод с выбором операций через флаги
        public void ProcessTextWithOptions(string text, bool analyze, bool save, bool log, bool cleanupCache)
        {
            ExecuteAnalysisIfRequested(text, analyze);
            SaveTextIfRequested(text, save);
            LogTextIfRequested(text, log);
            CleanupCacheIfRequested(cleanupCache);
        }

        #region Методы анализа текста

        private TextAnalysisResult AnalyzeText(string text)
        {
            string[] words = SplitTextIntoWords(text);
            int wordCount = words.Length;
            string longestWord = FindLongestWord(words);

            return new TextAnalysisResult(wordCount, longestWord);
        }

        private string[] SplitTextIntoWords(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private string FindLongestWord(string[] words)
        {
            return words.OrderByDescending(word => word.Length).First();
        }

        #endregion

        #region Методы управления данными

        private void StoreTextData(string text)
        {
            _processedTexts.Add(text);
            _processedCount++;
        }

        private void PerformStorageMaintenance()
        {
            ManageStorageSize();
            PrintProcessingStatistics();
        }

        private void ManageStorageSize()
        {
            if (_processedTexts.Count > MaxDataSize)
            {
                _processedTexts.RemoveAt(0);
            }
        }

        private void CleanupCacheIfRequested(bool cleanupCache)
        {
            if (cleanupCache && _processedTexts.Count > CacheCleanupThreshold)
            {
                _processedTexts.RemoveRange(0, CacheCleanupCount);
            }
        }

        #endregion

        #region Методы вывода и логирования

        private void DisplayProcessingResults(TextAnalysisResult result)
        {
            Console.WriteLine($"Обработано текстов: {result.WordCount}, самое длинное слово: '{result.LongestWord}'");
        }

        private void PrintProcessingStatistics()
        {
            if (_processedCount % StatsPrintInterval == 0)
            {
                Console.WriteLine($"Всего обработано текстов: {_processedCount}");
            }
        }

        private void ExecuteAnalysisIfRequested(string text, bool analyze)
        {
            if (analyze)
            {
                int wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                // Дополнительная логика анализа может быть добавлена здесь
            }
        }

        private void SaveTextIfRequested(string text, bool save)
        {
            if (save)
            {
                _processedTexts.Add(text);
            }
        }

        private void LogTextIfRequested(string text, bool log)
        {
            if (log)
            {
                Console.WriteLine($"Лог текста: {text}");
            }
        }

        #endregion

        // Вспомогательный класс для хранения результатов анализа
        private record TextAnalysisResult(int WordCount, string LongestWord);
    }

    // Основной класс программы 
    internal class Program
    {
        // Константы для демонстрации
        private const int MinAnalysisType = 1;
        private const int MaxAnalysisType = 5;
        private const int DictionaryAnalysisType = 3;

        // Демонстрация работы всех классов
        public static void Main(string[] args)
        {
            TextAnalyzer analyzer = new TextAnalyzer();
            string testText = "Тестовый текст для проверки работы анализатора";

            DemonstrateComprehensiveAnalysis(analyzer, testText);
            DemonstrateTextProcessor(testText);
            DemonstrateUniversalAnalysis(analyzer, testText);
        }

        private static void DemonstrateComprehensiveAnalysis(TextAnalyzer analyzer, string text)
        {
            Console.WriteLine("=== Тестирование комплексного анализа ===");
            analyzer.AnalyzeTextComprehensive(text);
            analyzer.AnalyzeTextWithOutputOptions(text, 3);
        }

        private static void DemonstrateTextProcessor(string text)
        {
            Console.WriteLine("\n=== Тестирование процессора текстов ===");
            TextProcessor processor = new TextProcessor();
            processor.ProcessAndStoreText(text);
        }

        private static void DemonstrateUniversalAnalysis(TextAnalyzer analyzer, string text)
        {
            Console.WriteLine("\n=== Тестирование универсального анализа ===");
            DemonstrateWordCountAnalysis(analyzer, text);
            DemonstrateAllAnalysisTypes(analyzer, text);
        }

        private static void DemonstrateWordCountAnalysis(TextAnalyzer analyzer, string text)
        {
            var wordCountResult = analyzer.AnalyzeText(text, 1);
            Console.WriteLine($"Количество слов: {wordCountResult}");
        }

        private static void DemonstrateAllAnalysisTypes(TextAnalyzer analyzer, string text)
        {
            Console.WriteLine("\n=== Тестирование всех типов анализа ===");

            for (int analysisType = MinAnalysisType; analysisType <= MaxAnalysisType; analysisType++)
            {
                DemonstrateSingleAnalysisType(analyzer, text, analysisType);
            }
        }

        private static void DemonstrateSingleAnalysisType(TextAnalyzer analyzer, string text, int analysisType)
        {
            var analysisResult = analyzer.AnalyzeText(text, analysisType);

            if (analysisType == DictionaryAnalysisType)
            {
                DisplayCharacterFrequencyAnalysis(analyzer, analysisType, analysisResult);
            }
            else
            {
                Console.WriteLine($"Тип анализа {analysisType}: {analysisResult}");
            }
        }

        private static void DisplayCharacterFrequencyAnalysis(TextAnalyzer analyzer, int analysisType, object analysisResult)
        {
            Console.WriteLine($"Тип анализа {analysisType} - Частотность символов:");

            if (analysisResult is Dictionary<char, int> characterFrequency)
            {
                analyzer.PrintCharacterFrequency(characterFrequency);
            }
        }
    }
}