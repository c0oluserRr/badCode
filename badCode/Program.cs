using System;
using System.Collections.Generic;
using System.Linq;

namespace badCode
{
    //основной класс для анализа текста
    public class TX
    {
        // Константы для замены магических чисел
        private const int MaxStatisticsSize = 100;
        private const int MaxHistorySize = 50;
        private const int MinOutputType = 1;
        private const int MaxOutputType = 5;
        private const int DefaultOutputType = 3;

        //массив для хранения статистики по количеству слов в предыдущих анализах
        private int[] n = new int[MaxStatisticsSize];
        // Список для хранения истории операций анализа
        private List<string> h = new List<string>();

        //основной метод анализа текста
        //выполняет полный анализ: подсчет слов, поиск самого длинного слова, частоту символов
        public void D(string s)
        {
            if (s == null || s.Length == 0)
                return;

            //подсчет слов путем отслеживания переходов между словами и пробелами
            int wc = 0; //wc - счетчик слов
            bool iw = false; //iw - флаг нахождения внутри слова
            for (int i = 0; i < s.Length; i++)
            {
                // если текущий символ - разделитель
                if (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r' || s[i] == '.' || s[i] == ',')
                {
                    if (iw) //если до этого были внутри слова
                    {
                        wc++; //увеличение счетчика слов
                        iw = false;
                    }
                }
                else
                {
                    iw = true;
                }
            }
            if (iw) wc++;

            // поиск самого длинного слова путем разбиения строки на слова
            string lw = ""; // lw - самое длинное слово
            string[] w = s.Split(' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':');
            foreach (string word in w)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    if (word.Length > lw.Length)
                    {
                        lw = word;
                    }
                }
            }

            // анализ частоты использования символов
            Dictionary<char, int> cf = new Dictionary<char, int>(); // cf - частота символов
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (!char.IsWhiteSpace(c))
                {
                    if (cf.ContainsKey(c))
                    {
                        cf[c] = cf[c] + 1;
                    }
                    else
                    {
                        cf.Add(c, 1);
                    }
                }
            }

            // управление историей операций - ограничение размера
            if (h.Count >= MaxHistorySize)
            {
                h.RemoveAt(0);
            }
            h.Add($"Всего: {wc}, Самое большое: {lw}");

            // сохранение статистики в массив
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] == 0)
                {
                    n[i] = wc;
                    break;
                }
            }

            // вывод результатов анализа
            Console.WriteLine($"Всего: {wc}, Самое большое: {lw}");
        }

        // метод для обработки текста с различными вариантами вывода
        public void P(string s, int t)
        {
            // Валидация параметра вывода
            if (t < MinOutputType || t > MaxOutputType)
                t = DefaultOutputType;

            // поиск самого длинного слова 
            string[] w = s.Split(' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':');
            string lw = "";
            foreach (string word in w)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    if (word.Length > lw.Length)
                    {
                        lw = word;
                    }
                }
            }

            // альтернативный алгоритм подсчета слов через ручной парсинг
            int c = 0;
            int i = 0;
            while (i < s.Length)
            {
                while (i < s.Length && (s[i] == ' ' || s[i] == '\t' || s[i] == '\n'))
                    i++;
                if (i < s.Length)
                    c++;
                while (i < s.Length && !(s[i] == ' ' || s[i] == '\t' || s[i] == '\n'))
                    i++;
            }

            // Константы для типов вывода
            const int OutputTypeWordCount = 1;
            const int OutputTypeLongestWord = 2;
            const int OutputTypeBoth = 3;
            const int OutputTypeWithChars = 4;
            const int OutputTypeAlternative = 5;

            // выбор формата вывода в зависимости от параметра t
            if (t == OutputTypeWordCount)
            {
                Console.WriteLine($"{c}");
            }
            else if (t == OutputTypeLongestWord)
            {
                Console.WriteLine($"Самое большое: {lw}");
            }
            else if (t == OutputTypeBoth)
            {
                Console.WriteLine($"Всего: {c}, самое большое: {lw}");
            }
            else if (t == OutputTypeWithChars)
            {
                // подсчет уникальных символов
                Dictionary<char, int> cf = new Dictionary<char, int>();
                for (int j = 0; j < s.Length; j++)
                {
                    char ch = s[j];
                    if (!char.IsWhiteSpace(ch))
                    {
                        if (cf.ContainsKey(ch))
                        {
                            cf[ch] = cf[ch] + 1;
                        }
                        else
                        {
                            cf.Add(ch, 1);
                        }
                    }
                }
                Console.WriteLine($"Всего: {c}, знаков: {cf.Count}");
            }
            else
            {
                Console.WriteLine($"Все: {c}, {lw}");
            }
        }

        // Константы для типов анализа
        private const int AnalysisTypeWordCount = 1;
        private const int AnalysisTypeLongestWord = 2;
        private const int AnalysisTypeCharFrequency = 3;
        private const int AnalysisTypeTextLength = 4;
        private const int AnalysisTypeTotalWordLength = 5;

        // универсальный метод анализа с возвратом результата
        public object A(string s, int type)
        {
            switch (type)
            {
                case AnalysisTypeWordCount:
                    return s.Split(' ').Length; // количество слов через простое разбиение
                case AnalysisTypeLongestWord:
                    return s.Split(' ').OrderByDescending(x => x.Length).FirstOrDefault(); // самое длинное слово
                case AnalysisTypeCharFrequency:
                    // частота всех символов в тексте
                    Dictionary<char, int> d = new Dictionary<char, int>();
                    foreach (char c in s)
                    {
                        if (d.ContainsKey(c)) d[c]++;
                        else d[c] = 1;
                    }
                    return d;
                case AnalysisTypeTextLength:
                    return s.Length; //общая длина текста
                case AnalysisTypeTotalWordLength:
                    return s.Split(' ').Sum(x => x.Length); //сумма длин всех слов
                default:
                    return null;
            }
        }

        // вспомогательный метод для красивого вывода словаря с частотностью символов
        public void PrintDict(Dictionary<char, int> dict)
        {
            if (dict == null) return;

            foreach (var kvp in dict)
            {
                Console.WriteLine($"'{kvp.Key}': {kvp.Value}");
            }
        }
    }

    // класс для обработки и хранения текстовых данных
    public class U
    {
        // Константы для ограничений хранилища
        private const int MaxDataSize = 100;
        private const int StatsPrintInterval = 10;
        private const int CacheCleanupThreshold = 50;
        private const int CacheCleanupCount = 10;

        private List<string> data = new List<string>();
        private int counter = 0;

        //метод обработки текста с сохранением в историю
        public void ProcessText(string text)
        {
            //анализ текста: разбиение на слова и поиск самого длинного
            string[] words = text.Split(' ');
            int wordCount = words.Length;
            string longest = words.OrderByDescending(w => w.Length).First();

            //сохранение текста и обновление счетчика
            data.Add(text);
            counter++;

            Console.WriteLine($"Обработано: {wordCount}, самое большое: {longest}");

            // Ограничение размера хранилища
            if (data.Count > MaxDataSize)
            {
                data.RemoveAt(0);
            }

            // Периодический вывод статистики
            if (counter % StatsPrintInterval == 0)
            {
                Console.WriteLine($"Всего обработано: {counter}");
            }
        }

        // универсальный метод с выбором операций через флаги
        public void DoEverything(string text, bool analyze, bool save, bool log, bool cache)
        {
            if (analyze)
            {
                var words = text.Split(' ').Length; // простой анализ - подсчет слов
            }

            if (save)
            {
                data.Add(text);
            }

            if (log)
            {
                Console.WriteLine(text);
            }

            // Очистка кэша при превышении порога
            if (cache && data.Count > CacheCleanupThreshold)
            {
                data.RemoveRange(0, CacheCleanupCount);
            }
        }
    }

    // основной класс программы 
    internal class Program
    {
        // Константы для демонстрации
        private const int MinAnalysisType = 1;
        private const int MaxAnalysisType = 5;
        private const int DictionaryAnalysisType = 3;

        //демонстрация работы всех классов
        public static void Main(string[] args)
        {
            TX analyzer = new TX();

            string tt = "Тестовый текст для проверки";

            Console.WriteLine("Тестирование");
            analyzer.D(tt);
            analyzer.P(tt, 3);

            Console.WriteLine("\nТест класса U");
            U processor = new U();
            processor.ProcessText(tt);

            Console.WriteLine("\nТест switch метода");
            var result = analyzer.A(tt, 1);
            Console.WriteLine($"Всего: {result}");

            Console.WriteLine("\nТест с разными типами");
            //тестирование всех типов анализа
            for (int i = MinAnalysisType; i <= MaxAnalysisType; i++)
            {
                var res = analyzer.A(tt, i);

                if (i == DictionaryAnalysisType) // особый случай для словаря с частотностью символов
                {
                    Console.WriteLine($"Тип {i}:");
                    if (res is Dictionary<char, int> dict)
                    {
                        foreach (var kvp in dict)
                        {
                            Console.WriteLine($"  '{kvp.Key}': {kvp.Value}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Тип {i}: {res}");
                }
            }
        }
    }
}