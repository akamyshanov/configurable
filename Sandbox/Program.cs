using System;
using System.Linq;
using Configurable;
using ServiceStack.Text;

namespace Sandbox
{
    class MyFooConfiguration
    {
        public int Integer { get; set; }
        public string StringOne { get; set; }
    }

    class MyBarConfiguration
    {
        public decimal Baz { get; set; } = 3;
    }

    class MyConfiguration
    {
        public MyFooConfiguration Foo { get; set; } = new MyFooConfiguration();
        public MyBarConfiguration Bar { get; set; } = new MyBarConfiguration();
        public ushort FooBar { get; set; }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            var path = args.FirstOrDefault() ?? "config.json";
            var reader = ConfigurationReader.Default<MyConfiguration>(path);
            reader.Error += ex => Console.Error.WriteLine(ex);

            reader.Updated += Log;
            var configuration = reader.Read();
            Log(configuration);

            Console.ReadKey(true);
        }

        private static void Log(MyConfiguration myConfiguration)
        {
            Console.WriteLine("Configuration:");
            Console.WriteLine(myConfiguration.Dump());
        }
    }
}
