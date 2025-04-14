namespace Inventory_Management
{
    using System;
    using System.IO;
    using System.Text;

    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Environment variable file (.env) not found");
            }

            var mappings = new Dictionary<string, string>
            {
                { "POSTGRES_DB", "Database" },
                { "POSTGRES_USER", "Username" },
                { "POSTGRES_PASSWORD", "Password" }
            };

            var sb = new StringBuilder("Server=localhost;");

            foreach (var line in File.ReadLines(filePath)
                                     .Select(l => l.Split('=', StringSplitOptions.RemoveEmptyEntries))
                                     .Where(parts => parts.Length == 2))
            {
                if (mappings.TryGetValue(line[0], out var key))
                {
                    sb.Append($"{key}={line[1]};");
                }
            }

            Environment.SetEnvironmentVariable("DefaultConnection", sb.ToString());
        }

        public static void Load2(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Environment variable file (.env) not found");
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}