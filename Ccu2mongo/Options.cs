using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ccu2mongo
{
    internal class Options
    {
        [Value(1, Required = true, HelpText = "Solution Path.")]
        public string SolutionPath { get; set; }
    }
}
