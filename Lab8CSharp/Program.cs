using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nОберіть завдання (1-5) або 0 для виходу:");
            Console.WriteLine("1. Робота з edu.ua адресами у тексті");
            Console.WriteLine("2. Видалити українські слова, що починаються на голосну");
            Console.WriteLine("3. Видалити у словах попередні входження останньої літери");
            Console.WriteLine("4. Запис чисел у двійковий файл по інтервалу, вивід");
            Console.WriteLine("5. Робота з файлами і папками у d:\\temp");
            Console.Write("Ваш вибір: ");
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "0") break;

            switch (choice)
            {
                case "1": Task1(); break;
                case "2": Task2(); break;
                case "3": Task3(); break;
                case "4": Task4(); break;
                case "5": Task5(); break;
                default:
                    Console.WriteLine("Некоректний вибір.");
                    break;
            }
        }
    }

    // Завдання 1
    static void Task1()
    {
        string inputPath = "input.txt";
        string outputPath = "output_task1.txt";

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл input.txt не знайдено.");
            return;
        }

        string text = File.ReadAllText(inputPath);
        string pattern = @"\b(?:https?:\/\/)?(?:www\.)?\w+\.edu\.ua\b";
        MatchCollection matches = Regex.Matches(text, pattern);

        if (matches.Count == 0)
        {
            Console.WriteLine("Знайдено 0 адрес типу *.edu.ua.");
            return;
        }

        Console.WriteLine($"Знайдено {matches.Count} адрес(и) типу *.edu.ua:");
        for (int i = 0; i < matches.Count; i++)
            Console.WriteLine($"{i + 1}. {matches[i].Value}");

        Console.WriteLine("Хочете замінити певну адресу? (так/ні)");
        string answer = Console.ReadLine()?.Trim().ToLower() ?? "";

        if (answer == "так")
        {
            int chosenIndex = -1;
            while (true)
            {
                Console.WriteLine("Введіть номер адреси для заміни:");
                string input = Console.ReadLine()?.Trim() ?? "";

                if (int.TryParse(input, out chosenIndex) && chosenIndex >= 1 && chosenIndex <= matches.Count)
                    break;
                Console.WriteLine("Некоректний номер. Спробуйте ще раз.");
            }

            Console.WriteLine("Введіть нову адресу:");
            string newAddress = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(newAddress))
            {
                Console.WriteLine("Нова адреса не введена. Заміна скасована.");
            }
            else
            {
                string oldAddress = matches[chosenIndex - 1].Value;
                text = text.Replace(oldAddress, newAddress);
                Console.WriteLine($"Адресу '{oldAddress}' замінено на '{newAddress}'.");
            }
        }
        else
        {
            Console.WriteLine("Заміна адрес відмінена.");
        }

        File.WriteAllText(outputPath, text);
        Console.WriteLine($"Результат записано у файл {outputPath}");
    }

    // Завдання 2
    static void Task2()
    {
        string inputPath = "input.txt";
        string outputPath = "output_task2.txt";

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл input.txt не знайдено.");
            return;
        }

        string text = File.ReadAllText(inputPath);

        // Українські голосні: а, е, є, и, і, ї, о, у, ю, я (врахуємо і великі)
        string pattern = @"\b[аеєиіїоуюяАЕЄИІЇОУЮЯ]\w*\b";

        string result = Regex.Replace(text, pattern, "").Replace("  ", " ");

        File.WriteAllText(outputPath, result);
        Console.WriteLine($"Вилучені українські слова на голосну. Результат у {outputPath}");
    }

    // Завдання 3
    static void Task3()
    {
        string inputPath = "input.txt";
        string outputPath = "output_task3.txt";

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл input.txt не знайдено.");
            return;
        }

        string text = File.ReadAllText(inputPath);

        string pattern = @"\b\w+\b";

        string result = Regex.Replace(text, pattern, m =>
        {
            string word = m.Value;
            if (word.Length < 2) return word;

            char lastChar = word[word.Length - 1];
            int lastIndex = word.Length - 1;

            // Видалити усі входження lastChar, крім останнього
            string withoutPrev = "";

            for (int i = 0; i < lastIndex; i++)
            {
                if (word[i] != lastChar)
                    withoutPrev += word[i];
            }

            withoutPrev += lastChar;
            return withoutPrev;
        });

        File.WriteAllText(outputPath, result);
        Console.WriteLine($"Обробка завершена. Результат у файлі {outputPath}");
    }

    // Завдання 4
    static void Task4()
    {
        string filePath = "numbers.bin";

        Console.WriteLine("Введіть кількість чисел n:");
        if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
        {
            Console.WriteLine("Некоректне число.");
            return;
        }

        int[] numbers = new int[n];
        Console.WriteLine($"Введіть {n} цілих чисел:");

        for (int i = 0; i < n; i++)
        {
            Console.Write($"Число {i + 1}: ");
            while (!int.TryParse(Console.ReadLine(), out numbers[i]))
            {
                Console.WriteLine("Некоректне число, спробуйте ще раз:");
            }
        }

        Console.WriteLine("Введіть початок інтервалу:");
        if (!int.TryParse(Console.ReadLine(), out int intervalStart))
        {
            Console.WriteLine("Некоректне число.");
            return;
        }

        Console.WriteLine("Введіть кінець інтервалу:");
        if (!int.TryParse(Console.ReadLine(), out int intervalEnd))
        {
            Console.WriteLine("Некоректне число.");
            return;
        }

        var filtered = numbers.Where(x => x >= intervalStart && x <= intervalEnd).ToArray();

        using (var bw = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            bw.Write(filtered.Length);
            foreach (var num in filtered)
                bw.Write(num);
        }

        Console.WriteLine($"Записано {filtered.Length} чисел у файл {filePath}.");

        // Читаємо і виводимо вміст файлу
        using (var br = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            int count = br.ReadInt32();
            Console.WriteLine("Вміст файлу:");
            for (int i = 0; i < count; i++)
            {
                int val = br.ReadInt32();
                Console.WriteLine(val);
            }
        }
    }

    // Завдання 5
    static void Task5()
    {
        string basePath = @"d:\temp";
        string surname = "Kornivska";  // замініть на своє прізвище

        string folder1 = Path.Combine(basePath, surname + "1");
        string folder2 = Path.Combine(basePath, surname + "2");

        try
        {
            // 1. Створити папки
            Directory.CreateDirectory(folder1);
            Directory.CreateDirectory(folder2);

            // 2. Створити файли t1.txt і t2.txt у folder1
            string t1Path = Path.Combine(folder1, "t1.txt");
            string t2Path = Path.Combine(folder1, "t2.txt");

            File.WriteAllText(t1Path, "<Шевченко Степан Іванович, 2001> року народження, місце проживання <м. Суми>");
            File.WriteAllText(t2Path, "<Комар Сергій Федорович, 2000 > року народження, місце проживання <м. Київ>");

            // 3. Створити t3.txt у folder2 - копіюємо вміст t1 та t2
            string t3Path = Path.Combine(folder2, "t3.txt");
            string t3Text = File.ReadAllText(t1Path) + Environment.NewLine + File.ReadAllText(t2Path);
            File.WriteAllText(t3Path, t3Text);

            // 4. Вивести інформацію про створені файли
            Console.WriteLine("Інформація про файли:");
            PrintFileInfo(t1Path);
            PrintFileInfo(t2Path);
            PrintFileInfo(t3Path);

            // 5. Перемістити t2.txt у folder2
            string newT2Path = Path.Combine(folder2, "t2.txt");
            if (File.Exists(newT2Path)) File.Delete(newT2Path);
            File.Move(t2Path, newT2Path);

            // 6. Скопіювати t1.txt у folder2
            string copyT1Path = Path.Combine(folder2, "t1.txt");
            File.Copy(t1Path, copyT1Path, true);

            // 7. Перейменувати папку folder2 у ALL, видалити folder1
            string allFolder = Path.Combine(basePath, "ALL");

            if (Directory.Exists(allFolder))
                Directory.Delete(allFolder, true);

            Directory.Move(folder2, allFolder);

            if (Directory.Exists(folder1))
                Directory.Delete(folder1, true);

            // 8. Вивести інформацію про файли у ALL
            Console.WriteLine("\nІнформація про файли у папці ALL:");
            foreach (var file in Directory.GetFiles(allFolder))
            {
                PrintFileInfo(file);
            }

            Console.WriteLine("\nЗавдання виконано.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Помилка: " + ex.Message);
        }
    }

    static void PrintFileInfo(string path)
    {
        var fi = new FileInfo(path);
        Console.WriteLine($"Файл: {fi.Name}, Розмір: {fi.Length} байт, Дата створення: {fi.CreationTime}");
    }
}
