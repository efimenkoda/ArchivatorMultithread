using System;
using System.IO;


namespace Archiver
{
    public class ArchiverManager
    {
        private string processArchiver;
        private string inputFile;
        private string outputFile;
        private string[] args;
        public ArchiverManager(string[] args)
        {
            this.args = args;
        }

        private bool isInitializationParametrs()
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Введены не все аргументы: \n compress/decompress [имя исходного файла] [имя результирующего файла]");
                return false;
            }

            if (args[0].ToLower().Equals("compress") || args[0].ToLower().Equals("decompress"))
            {
                processArchiver = args[0].ToLower();
            }
            else
            {
                Console.WriteLine("Неверно введен первый аргумент: compress/decompress");
                return false;
            }
            inputFile = args[1];
            if (inputFile.Length == 0 || !File.Exists(inputFile))
            {
                Console.WriteLine("Расположение входного файла введено некорректно или файла не существует");
                return false;
            }

            outputFile = args[2];
            if (inputFile.Length == 0 || File.Exists(outputFile))
            {
                Console.WriteLine("Расположение выходного файла не введено или такой файл уже существует");
                return false;
            }            

            return true;
        }

        public void Start()
        {
            try
            {
                if (isInitializationParametrs())
                {
                    AbstractArchiver abstractArchiver;
                    if (processArchiver.Equals("compress"))
                    {
                        Console.WriteLine("Compressing...");
                        abstractArchiver = new Compress(inputFile, outputFile);
                    }
                    else
                    {
                        Console.WriteLine("Decompressing...");
                        abstractArchiver = new Decompress(inputFile, outputFile);
                    }
                    abstractArchiver.GetProccess();
                }
                else
                {
                    Console.WriteLine(1);
                    return;
                }
                Console.WriteLine(0);
            }
            catch (Exception)
            {
                Console.WriteLine("Произошла ошибка во время сжатия/распаковки");
                Console.WriteLine(1);
            }

        }
    }
}
