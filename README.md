[![Build status](https://ci.appveyor.com/api/projects/status/tny2ib6gfwcbe28f/branch/master?svg=true)](https://ci.appveyor.com/project/akamyshanov/configurable/branch/master)

# Configurable

Configuration utilities for .NET

## Usage

```
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

            // Craete a reader for the specified path
            var reader = ConfigurationReader.Default<MyConfiguration>(path);

            // Listen for all sorts of errors
            reader.Error += ex => Console.Error.WriteLine(ex);

            // Listen for changes
            var token = reader.OnChanges(Log);

            // Read configuration
            var configuration = reader.Read();

            Log(configuration);

            Console.ReadKey(true);

            // Stop listening for changes
            token.Dispose();
        }

        private static void Log(MyConfiguration myConfiguration)
        {
            Console.WriteLine("Configuration:");
            Console.WriteLine(myConfiguration.Dump());
        }
    }
}

```

## Icon

[Document setting](https://thenounproject.com/term/document-setting/615885/) icon by Aybige from [the Noun Project](http://thenounproject.com/).
