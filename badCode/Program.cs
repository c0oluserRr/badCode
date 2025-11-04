using System;
using System.Collections.Generic;
using System.Linq;

namespace badCode
{
    public class TX
    {
        private int[] n = new int[100];
        private List<string> h = new List<string>();

        public void D(string s)
        {
            if (s == null || s.Length == 0)
                return;

            int wc = 0;
            bool iw = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r' || s[i] == '.' || s[i] == ',')
                {
                    if (iw)
                    {
                        wc++;
                        iw = false;
                    }
                }
                else
                {
                    iw = true;
                }
            }
            if (iw) wc++;

            string lw = "";
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

            Dictionary<char, int> cf = new Dictionary<char, int>();
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

            if (h.Count >= 50)
            {
                h.RemoveAt(0);
            }
            h.Add($"Всего: {wc}, Самое большое: {lw}");

            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] == 0)
                {
                    n[i] = wc;
                    break;
                }
            }

            Console.WriteLine($"Всего: {wc}, Самое большое: {lw}");
        }

        public void P(string s, int t)
        {
            if (t < 1 || t > 5)
                t = 3;

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

            if (t == 1)
            {
                Console.WriteLine($"{c}");
            }
            else if (t == 2)
            {
                Console.WriteLine($"Самое большое: {lw}");
            }
            else if (t == 3)
            {
                Console.WriteLine($"Всего: {c}, самое большое: {lw}");
            }
            else if (t == 4)
            {
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

        public object A(string s, int type)
        {
            switch (type)
            {
                case 1:
                    return s.Split(' ').Length;
                case 2:
                    return s.Split(' ').OrderByDescending(x => x.Length).FirstOrDefault();
                case 3:
                    Dictionary<char, int> d = new Dictionary<char, int>();
                    foreach (char c in s)
                    {
                        if (d.ContainsKey(c)) d[c]++;
                        else d[c] = 1;
                    }
                    return d;
                case 4:
                    return s.Length;
                case 5:
                    return s.Split(' ').Sum(x => x.Length);
                default:
                    return null;
            }
        }
        public void PrintDict(Dictionary<char, int> dict)
        {
            if (dict == null) return;

            foreach (var kvp in dict)
            {
                Console.WriteLine($"'{kvp.Key}': {kvp.Value}");
            }
        }
    }

    public class U
    {
        private List<string> data = new List<string>();
        private int counter = 0;

        public void ProcessText(string text)
        {
            string[] words = text.Split(' ');
            int wordCount = words.Length;
            string longest = words.OrderByDescending(w => w.Length).First();

            data.Add(text);
            counter++;

            Console.WriteLine($"Обработано: {wordCount}, самое большое: {longest}");

            if (data.Count > 100)
            {
                data.RemoveAt(0);
            }

            if (counter % 10 == 0)
            {
                Console.WriteLine($"Всего обработано: {counter}");
            }
        }

        public void DoEverything(string text, bool analyze, bool save, bool log, bool cache)
        {
            if (analyze)
            {
                var words = text.Split(' ').Length;
            }

            if (save)
            {
                data.Add(text);
            }

            if (log)
            {
                Console.WriteLine(text);
            }

            if (cache && data.Count > 50)
            {
                data.RemoveRange(0, 10);
            }
        }
    }

    internal class Program
    {
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
            for (int i = 1; i <= 5; i++)
            {
                var res = analyzer.A(tt, i);

                if (i == 3) 
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