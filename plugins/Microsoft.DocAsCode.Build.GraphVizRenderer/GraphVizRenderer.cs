using Microsoft.DocAsCode.Dfm;
using Microsoft.DocAsCode.MarkdownLite;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DocAsCore.Build.GraphVizRenderer
{
    public class GraphVizRendererPart : DfmCustomizedRendererPartBase<IMarkdownRenderer, MarkdownCodeBlockToken, MarkdownBlockContext>
    {
        public override string Name => "GraphVizRendererPart";

        public override bool Match(IMarkdownRenderer renderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
            return token.Lang == "graphviz";
        }

        public override StringBuffer Render(IMarkdownRenderer renderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
            var additionalArguments = ""; //should fetch some additional arguments from docfx config

            var tempInput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString("N")}.mmd");
            var tempOutput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString("N")}.svg");

            File.WriteAllText(tempInput, token.Code);

            string arguments = $" -Tsvg \"{tempInput}\" -o \"{tempOutput}\" {additionalArguments}";

            var programFilesx86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

            var graphVizDirectory = Directory.GetDirectories(programFilesx86).FirstOrDefault(dir => new DirectoryInfo(dir).Name.StartsWith("graphviz", StringComparison.OrdinalIgnoreCase));

            if (graphVizDirectory == null)
            {
                return "counldn't find graphviz on your system";
            }

            var dotStartInfo = new ProcessStartInfo(Path.Combine(graphVizDirectory, @"bin\dot.exe"), arguments)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };


            var process = Process.Start(dotStartInfo);

            if (!process.WaitForExit(30000))
            {
                process.Kill(); //should probably use task kill /pkill 
                return "dot cli timedout";
                //throw new Exception("mermaid cli timedout");
            }


            if (process.ExitCode != 0)
            {
                return "counldn't render the dot graph";
                //throw new Exception("counldn't render the mermaid graph"); // should probably render something
            }

            StringBuffer result = "<div class=\"";
            result += renderer.Options.LangPrefix;
            result += "graphviz";
            result += "\">";
            result += File.ReadAllText(tempOutput);
            result += "\n</div>";
            return result;
        }
    }
}
