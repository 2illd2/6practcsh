using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;


public class Figure
{
    public string Type { get; set; } = "Прямоугольник";
    public int Length { get; set; } = 45;
    public int Width { get; set; } = 15;

    public Figure()
    {

    }

    public Figure(string? type, int length, int width)
    {
        Type = type;
        Length = length;
        Width = width;
    }
}

public class FileHandler
{
    private string path1;

    public FileHandler(string path)
    {
        path1 = path;
    }

    public List<Figure> Load()
    {
        if (!File.Exists(path1))
        {
            File.Create(path1).Close();
        }

        using (StreamReader sr = new StreamReader(path1))
        {
            string line;
            int length = 0, width = 0;
            string figureType = null;
            List<Figure> figures = new List<Figure>();

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("Тип:"))
                {
                    figureType = line.Substring(4).Trim();
                }
                else if (line.StartsWith("Длина:"))
                {
                    if (!Int32.TryParse(line.Substring(6), out length))
                    {
                        Console.WriteLine($"Нельзя преобразовать '{line.Substring(6)}' в число.");
                        return null;
                    }
                }
                else if (line.StartsWith("Ширина:"))
                {
                    if (!Int32.TryParse(line.Substring(7), out width))
                    {
                        Console.WriteLine($"Нельзя преобразовать '{line.Substring(7)}' в число.");
                        return null;
                    }
                }

                if (figureType != null && length != 0 && width != 0)
                {
                    figures.Add(new Figure(figureType, length, width));
                    figureType = null;
                    length = 0;
                    width = 0;
                }
            }
            return figures;


        }
    }

    public void Save(Figure figure, string filePath)
    {


        string format = Path.GetExtension(filePath).TrimStart('.');
        if (format == "")
        {
            Console.WriteLine("Неправильный путь сохранения файла.");
            return;
        }

        string newPath = Path.ChangeExtension(filePath, format);

        switch (format)
        {
            case "json":
                string json = JsonConvert.SerializeObject(figure);


                File.WriteAllText(newPath, json);
                break;
            case "txt":
                using (StreamWriter str = new StreamWriter(newPath))
                {
                    str.WriteLine($"Тип:{figure.Type}");
                    str.WriteLine($"Длина:{figure.Length}");
                    str.WriteLine($"Ширина:{figure.Width}");
                }
                break;
            
            case "xml":
                XmlSerializer formatter = new XmlSerializer(typeof(Figure));
                using (FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, figure);
                }
                break;
            default:
                Console.WriteLine("Неправильный формат.");
                break;
        }
    }
}





class Program
{
    static void Main(string[] args)
    {

        string Type = "Прямоугольник";
        int Length = 45;
        int Width = 15;
        Console.WriteLine("Введите путь до файла, который вы хотите открыть");
        string path = Console.ReadLine();

        FileHandler fileHandler = new FileHandler(path);
        List<Figure> figures = fileHandler.Load();

        if (figures != null)
        {
            Console.WriteLine("Содержимое файла:");
            Console.WriteLine("Изменить:");
            Console.WriteLine("  " + Type);
            Console.WriteLine("  " + Length);
            Console.WriteLine("  " + Width);
            Console.WriteLine("  Оставить эти данные");
            int pos = Menu.Show(4, 7);
            Console.Clear();
            Console.WriteLine("Нажмите 2 раза на Enter");

            
            string change = "";
            int change2 = 0;
            int change3 = 0;

            if (pos == 4)
            {
                Console.WriteLine("Введите тип фигуры:");
                change = Console.ReadLine();
                Type = change;
                Console.WriteLine("Тип фигуры изменен на: " + Type);
            }
            else if (pos == 5)
            {
                Console.WriteLine("Введите длину:");
                change = Console.ReadLine();
                if (int.TryParse(change, out change2))
                {
                    Length = change2;
                    Console.WriteLine("Длина изменена на: " + Length);
                }
                else
                {
                    Console.WriteLine("Непрпвильный формат ввода.");
                }
            }
            else if (pos == 6)
            {
                Console.WriteLine("Введите ширину:");
                change = Console.ReadLine();
                if (int.TryParse(change, out change3))
                {
                    Width = change3;
                    Console.WriteLine("Ширина изменена на: " + Width);
                }
                else
                {
                    Console.WriteLine("Неправильный формат ввода.");
                }
            }

            
            Console.WriteLine("Содержимое файла:");
            Console.WriteLine(Type);
            Console.WriteLine(Length);
            Console.WriteLine(Width);

            Console.WriteLine("Введите путь до файла куда вы хотите сохранить текст");
            string savePath = Console.ReadLine();

            fileHandler.Save(new Figure(Type, Length, Width), savePath);
            Console.WriteLine("Сохранено! ");
        }
    }
 }
class Menu
{


    public static int Show(int min, int max)
    {
        int pos = 4;
        ConsoleKeyInfo key;
        do
        {
            Console.SetCursorPosition(0, pos);
            Console.WriteLine("->");
            key = Console.ReadKey();
            Console.SetCursorPosition(0, pos);
            Console.WriteLine("  ");

            if (key.Key == ConsoleKey.UpArrow && pos != min)
                pos--;
            else if (key.Key == ConsoleKey.DownArrow & pos != max)
                pos++;
            else if (key.Key == ConsoleKey.Escape)
                return -1;
        } while (key.Key != ConsoleKey.Enter);
        Console.Clear();
        return pos;
    }

}