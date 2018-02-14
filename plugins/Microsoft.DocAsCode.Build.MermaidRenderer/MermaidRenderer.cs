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

namespace Microsoft.DocAsCore.Build.MermaidRenderer
{
    public class MermaidRendererPart : DfmCustomizedRendererPartBase<IMarkdownRenderer, MarkdownCodeBlockToken, MarkdownBlockContext>
    {
        public override string Name => "MermaidRendererPart";

        public override bool Match(IMarkdownRenderer renderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
            return token.Lang == "mermaid";
        }

        public override StringBuffer Render(IMarkdownRenderer renderer, MarkdownCodeBlockToken token, MarkdownBlockContext context)
        {
            var additionalArguments = ""; //should fetch some additional arguments from docfx config

            var tempInput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString("N")}.mmd");
            var tempOutput = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString("N")}.svg");

            File.WriteAllText(tempInput, token.Code);

            string arguments = $"-i \"{tempInput}\" -o \"{tempOutput}\" {additionalArguments}";

            var mmdcStartInfo = new ProcessStartInfo(@"mmdc.cmd", arguments) // should probably look for a valid install and render a message
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };


            var process = Process.Start(mmdcStartInfo);

            if (!process.WaitForExit(30000))
            {
                process.Kill(); //should probably use task kill /pkill 
                return "mermaid cli timedout";
                //throw new Exception("mermaid cli timedout");
            }


            if (process.ExitCode != 0)
            {
                return "counldn't render the mermaid graph";
                //throw new Exception("counldn't render the mermaid graph"); // should probably render something
            }

            StringBuffer result = "<div class=\"";
            result += renderer.Options.LangPrefix;
            result += "mermaid";
            result += "\">";
            result += File.ReadAllText(tempOutput);
            result += "\n</div>";
            return result;
        }
    }
}
