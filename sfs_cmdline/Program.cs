using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace sfs_cmdline
{
    class Program
    {
        public class Options
        {
            [Option('v', Required = false, HelpText = "Print progress messages.")]
            public bool Verbose { get; set; } = false;

            [Option('p',"projected", Required = false, HelpText = "Write projected images into this directory")]
            public string Projected { get; set; } = null;

            [Option('o', "overlaid", Required = false, HelpText = "Write images with boolean mask overlaid into this directory")]
            public string Overlaid { get; set; } = null;

            [Value(0, MetaName = "source", Required = false, HelpText = "Directory containing source images.  Defaults to '.'")]
            public string Source { get; set; } = ".";

            [Option('b', "bounds", Required = true, Separator =':' )]
            public IEnumerable<int> Bounds { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(Run)
              .WithNotParsed(HandleParseError);
        }

        static void Run(Options opts)
        {
            Console.WriteLine($"projected={opts.Projected} overlaid={opts.Overlaid} source={opts.Source}");
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            Environment.Exit(-1);
        }
    }
}
