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

        // Основной метод анализа текста
        // Выполняет полный анализ: подсчет слов, поиск самого длинного слова, частоту символов
        public void AnalyzeTextComprehensive(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Подсчет слов путем отслеживания переходов между словами и пробелами
            int wordCount = 0;
            bool isInsideWord = false;

            for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                char currentChar = text[currentIndex];
                // Если текущий символ - разделитель
                if (IsWordSeparator(currentChar))
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

            if (isInsideWord)
                wordCount++;

            // Поиск самого длинного слова путем разбиения строки на слова
            string longestWord = FindLongestWord(text);

            // Анализ частоты использования символов
            Dictionary<char, int> characterFrequency = AnalyzeCharacterFrequency(text);

            // Управление историей операций
            UpdateAnalysisHistory(wordCount, longestWord);

            // Сохранение статистики в массив
            UpdateWordCountStatistics(wordCount);

            // Вывод результатов анализа
            Console.WriteLine($"Всего слов: {wordCount}, Самое длинное слово: '{longestWord}'");
        }

        // Метод для обработки текста с различными вариантами вывода
        public void AnalyzeTextWithOutputOptions(string text, int outputType)
        {
            // Валидация параметра вывода
            int validatedOutputType = ValidateOutputType(outputType);

            // Поиск самого длинного слова
            string longestWord = FindLongestWord(text);

            // Альтернативный алгоритм подсчета слов через ручной парсинг
            int wordCount = CountWordsManual(text);

            // Выбор формата вывода
            DisplayAnalysisResults(wordCount, longestWord, validatedOutputType, text);
        }

        // Константы для типов анализа
        private const int AnalysisTypeWordCount = 1;
        private const int AnalysisTypeLongestWord = 2;
        private const int AnalysisTypeCharFrequency = 3;
        private const int AnalysisTypeTextLength = 4;
        private const int AnalysisTypeTotalWordLength = 5;

        // Универсальный метод анализа с возвратом результата
        public object AnalyzeText(string text, int analysisType)
        {
            switch (analysisType)
            {
                case AnalysisTypeWordCount:
                    return CountWordsSimple(text);
                case AnalysisTypeLongestWord:
                    return FindLongestWordSimple(text);
                case AnalysisTypeCharFrequency:
                    return AnalyzeCharacterFrequency(text);
                case AnalysisTypeTextLength:
                    return text.Length;
                case AnalysisTypeTotalWordLength:
                    return CalculateTotalWordLength(text);
                default:
                    return null;
            }
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

        // Вспомогательные приватные методы
        private bool IsWordSeparator(char character)
        {
            char[] separators = { ' ', '\t', '\n', '\r', '.', ',' };
            return separators.Contains(character);
        }

        private string FindLongestWord(string text)
        {
            char[] wordSeparators = { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' };
            string[] words = text.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries);

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

        private Dictionary<char, int> AnalyzeCharacterFrequency(string text)
        {
            Dictionary<char, int> frequency = new Dictionary<char, int>();

            foreach (char currentChar in text)
            {
                if (!char.IsWhiteSpace(currentChar))
                {
                    if (frequency.ContainsKey(currentChar))
                    {
                        frequency[currentChar]++;
                    }
                    else
                    {
                        frequency[currentChar] = 1;
                    }
                }
            }
            return frequency;
        }

        private void UpdateAnalysisHistory(int wordCount, string longestWord)
        {
            if (_analysisHistory.Count >= MaxHistorySize)
            {
                _analysisHistory.RemoveAt(0);
            }
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

        private int CountWordsManual(string text)
        {
            int wordCount = 0;
            int currentIndex = 0;

            while (currentIndex < text.Length)
            {
                // Пропускаем пробельные символы
                while (currentIndex < text.Length && char.IsWhiteSpace(text[currentIndex]))
                    currentIndex++;

                if (currentIndex < text.Length)
                    wordCount++;

                // Пропускаем символы слова
                while (currentIndex < text.Length && !char.IsWhiteSpace(text[currentIndex]))
                    currentIndex++;
            }
            return wordCount;
        }

        private int ValidateOutputType(int outputType)
        {
            return (outputType < MinOutputType || outputType > MaxOutputType) ? DefaultOutputType : outputType;
        }

        private void DisplayAnalysisResults(int wordCount, string longestWord, int outputType, string text)
        {
            // Константы для типов вывода
            const int OutputTypeWordCount = 1;
            const int OutputTypeLongestWord = 2;
            const int OutputTypeBoth = 3;
            const int OutputTypeWithChars = 4;
            const int OutputTypeAlternative = 5;

            switch (outputType)
            {
                case OutputTypeWordCount:
                    Console.WriteLine($"Количество слов: {wordCount}");
                    break;
                case OutputTypeLongestWord:
                    Console.WriteLine($"Самое длинное слово: '{longestWord}'");
                    break;
                case OutputTypeBoth:
                    Console.WriteLine($"Слов: {wordCount}, Самое длинное: '{longestWord}'");
                    break;
                case OutputTypeWithChars:
                    Dictionary<char, int> characterFrequency = AnalyzeCharacterFrequency(text);
                    Console.WriteLine($"Слов: {wordCount}, Уникальных символов: {characterFrequency.Count}");
                    break;
                case OutputTypeAlternative:
                    Console.WriteLine($"Результат анализа: {wordCount} слов, самое длинное: '{longestWord}'");
                    break;
            }
        }

        private int CountWordsSimple(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private string FindLongestWordSimple(string text)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.OrderByDescending(word => word.Length).FirstOrDefault() ?? "";
        }

        private int CalculateTotalWordLength(string text)
        {
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Sum(word => word.Length);
        }
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
            // Анализ текста: разбиение на слова и поиск самого длинного
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int wordCount = words.Length;
            string longestWord = words.OrderByDescending(word => word.Length).First();

            // Сохранение текста и обновление счетчика
            _processedTexts.Add(text);
            _processedCount++;

            Console.WriteLine($"Обработано текстов: {wordCount}, самое длинное слово: '{longestWord}'");

            // Ограничение размера хранилища
            ManageStorageSize();

            // Периодический вывод статистики
            PrintProcessingStatistics();
        }

        // Универсальный метод с выбором операций через флаги
        public void ProcessTextWithOptions(string text, bool analyze, bool save, bool log, bool cleanupCache)
        {
            if (analyze)
            {
                int wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                // Можно добавить дополнительную обработку анализа
            }

            if (save)
            {
                _processedTexts.Add(text);
            }

            if (log)
            {
                Console.WriteLine($"Лог текста: {text}");
            }

            // Очистка кэша при превышении порога
            if (cleanupCache && _processedTexts.Count > CacheCleanupThreshold)
            {
                _processedTexts.RemoveRange(0, CacheCleanupCount);
            }
        }

        private void ManageStorageSize()
        {
            if (_processedTexts.Count > MaxDataSize)
            {
                _processedTexts.RemoveAt(0);
            }
        }

        private void PrintProcessingStatistics()
        {
            if (_processedCount % StatsPrintInterval == 0)
            {
                Console.WriteLine($"Всего обработано текстов: {_processedCount}");
            }
        }
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

            Console.WriteLine("=== Тестирование комплексного анализа ===");
            analyzer.AnalyzeTextComprehensive(testText);
            analyzer.AnalyzeTextWithOutputOptions(testText, 3);

            Console.WriteLine("\n=== Тестирование процессора текстов ===");
            TextProcessor processor = new TextProcessor();
            processor.ProcessAndStoreText(testText);

            Console.WriteLine("\n=== Тестирование универсального анализа ===");
            var wordCountResult = analyzer.AnalyzeText(testText, 1);
            Console.WriteLine($"Количество слов: {wordCountResult}");

            Console.WriteLine("\n=== Тестирование всех типов анализа ===");

            for (int analysisType = MinAnalysisType; analysisType <= MaxAnalysisType; analysisType++)
            {
                var analysisResult = analyzer.AnalyzeText(testText, analysisType);

                if (analysisType == DictionaryAnalysisType)
                {
                    Console.WriteLine($"Тип анализа {analysisType} - Частотность символов:");
                    if (analysisResult is Dictionary<char, int> characterFrequency)
                    {
                        analyzer.PrintCharacterFrequency(characterFrequency);
                    }
                }
                else
                {
                    Console.WriteLine($"Тип анализа {analysisType}: {analysisResult}");
                }
            }
        }
    }
}