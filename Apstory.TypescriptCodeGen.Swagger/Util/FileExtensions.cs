namespace Apstory.TypescriptCodeGen.Swagger.Util
{
    public static class FileExtensions
    {
        public static string ToLocalPath(this string path)
        {
            var localPath = Environment.CurrentDirectory;
            return Path.Combine(localPath, path);
        }

        public static string ReadEntireFile(this string path)
        {
            string toReturn = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    toReturn = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading file: " + path);
                Environment.Exit(1);
            }

            return toReturn;
        }

        public static void WriteToFile(this string data, string path)
        {
            try
            {
                var folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(data);
                    sw.Close();
                }

                Console.WriteLine("Generated: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + path + ". Exception: " + ex.Message);
                Environment.Exit(2);
            }
        }

        public static void AppendToFile(this string data, string path)
        {
            try
            {
                var folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                File.AppendAllText(path, data);
                Console.WriteLine("Appended to ExportFile: " + data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + path + ". Exception: " + ex.Message);
                Environment.Exit(2);
            }
        }
    }
}
