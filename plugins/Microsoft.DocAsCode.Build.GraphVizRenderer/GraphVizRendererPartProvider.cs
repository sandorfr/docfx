using Microsoft.DocAsCode.Dfm;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DocAsCore.Build.GraphVizRenderer
{
    [Export(typeof(IDfmCustomizedRendererPartProvider))]
    public class GraphVizRendererPartProvider : IDfmCustomizedRendererPartProvider
    {
        public IEnumerable<IDfmCustomizedRendererPart> CreateParts(IReadOnlyDictionary<string, object> parameters)
        {
            yield return new GraphVizRendererPart();
        }
    }
}
