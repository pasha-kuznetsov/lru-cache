using System;
using System.Linq;

namespace Tydbits.LruCache
{
    public class Program
    {
        private const string Exit = "EXIT";
        private const string Error = "ERROR";
        private const string Size = "SIZE";
        private const string Get = "GET";
        private const string Got = "GOT";
        private const string Set = "SET";
        private const string Ok = "OK";
        private const string NotFound = "NOTFOUND";

        private ICache _cache;

        private static void Main()
        {
            for (var program = new Program(); ;)
            {
                var input = Console.ReadLine();
                var output = program.Process(input);
                if (string.IsNullOrEmpty(output))
                    break;
                Console.WriteLine(output);
            }
        }

        public string Process(string command)
        {
            try
            {
                var args = (command ?? string.Empty).Trim().Split().Where(NotEmpty).ToArray();
                switch (args[0].ToUpperInvariant())
                {
                    case Size:
                        if (args.Length != 2) return Error;
                        if (_cache != null) return Error;
                        int size;
                        if (!int.TryParse(args[1], out size) || size <= 0)
                            return Error;
                        _cache = new LruCache(size);
                        return Output(Size, Ok);

                    case Get:
                        if (args.Length != 2) return Error;
                        var value = _cache.Get(args[1]);
                        return value == null ? NotFound : Output(Got, value);

                    case Set:
                        if (args.Length != 3) return Error;
                        _cache.Set(args[1], args[2]);
                        return Output(Set, Ok);

                    case Exit:
                        if (args.Length != 1) return Error;
                        return null;

                    default:
                        return Error;
                }
            }
            catch (Exception)
            {
                return Error;
            }
        }

        private static string Output(params object[] args)
        {
            return string.Join(" ", args);
        }

        private static bool NotEmpty(string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}
