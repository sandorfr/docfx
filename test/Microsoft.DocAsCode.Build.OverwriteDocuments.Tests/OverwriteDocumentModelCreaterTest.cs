// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Build.OverwriteDocuments.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Markdig;

    using Microsoft.DocAsCode.Build.OverwriteDocuments;
    using Microsoft.DocAsCode.Tests.Common;

    using Xunit;

    [Trait("Owner", "jipe")]
    [Trait("EntityType", "OverwriteDocumentModelCreater")]
    public class OverwriteDocumentModelCreaterTest: TestBase
    {
        [Fact]
        public void YamlCodeBlockTest()
        {
            var yamlCodeBlockString = "a:b\nc:d\ne:f";
            var actual = OverwriteDocumentModelCreater.ConvertYamlCodeBlock(yamlCodeBlockString);
            var expected = new Dictionary<string, object>()
            {
                {"a", "b"},
                {"c", "d"},
                {"e", "f"}
            };
            Assert.Equal(actual.ToString(), expected.ToString());
        }

        [Fact]
        public void ContentConvertTest()
        {
            var testBlockList = Markdown.Parse("Test").ToList();

            string[] testOPaths =
            {
                "summary",
                "return/description",
                "return/type",
                "function/parameters[id=\"para1\"]/description",
                "function/parameters[id=\"para1\"]/type",
                "function/parameters[id=\"para2\"]/description",
            };
            var contents = new List<MarkdownPropertyModel>();
            foreach (var item in testOPaths)
            {
                contents.Add(new MarkdownPropertyModel
                {
                    PropertyName = item,
                    PropertyNameSource = Markdown.Parse($"## `{item}`")[0],
                    PropertyValue = testBlockList
                });
            }
            var contentsMetadata = OverwriteDocumentModelCreater.ConvertContents(contents);
            Assert.Equal(3, contentsMetadata.Count);
            Assert.Equal("summary,return,function", ExtractDictionaryKeys(contentsMetadata));
            Assert.Equal(2, ((Dictionary<string, object>) contentsMetadata["return"]).Count);
            Assert.Equal("description,type",
                ExtractDictionaryKeys((Dictionary<string, object>) contentsMetadata["return"]));
            Assert.Single((Dictionary<string, object>) contentsMetadata["function"]);
            Assert.Equal(2,
                ((List<Dictionary<string, object>>) ((Dictionary<string, object>) contentsMetadata["function"])["parameters"]).Count);
            Assert.Equal("id,description,type",
                ExtractDictionaryKeys(
                    ((List<Dictionary<string, object>>) ((Dictionary<string, object>) contentsMetadata["function"])["parameters"])[0]));
            Assert.Equal("id,description",
                ExtractDictionaryKeys(
                    ((List<Dictionary<string, object>>) ((Dictionary<string, object>) contentsMetadata["function"])["parameters"])[1]));
        }

        [Fact]
        public void DuplicateTest()
        {
            var testBlockList = Markdown.Parse("Test").ToList();
            string[] testOPaths =
            {
                "function/parameters/description",
                "function/parameters[id=\"para1\"]/type",
            };
            var contents = new List<MarkdownPropertyModel>();
            foreach (var item in testOPaths)
            {
                contents.Add(new MarkdownPropertyModel
                {
                    PropertyName = item,
                    PropertyNameSource = Markdown.Parse($"## `{item}`")[0],
                    PropertyValue = testBlockList
                });
            }
            var ex = Assert.Throws<MarkdownFragmentsException>(() => OverwriteDocumentModelCreater.ConvertContents(contents));
            Assert.Equal("", ex.Message);
            Assert.Equal(0, ex.Position);
        }

        private string ExtractDictionaryKeys(Dictionary<string, object> dict)
        {
            return dict.Keys.ToList().Aggregate((a, b) => a + "," + b);
        }
    }
}
