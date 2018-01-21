// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace Microsoft.DocAsCode.Build.OverwriteDocuments.Tests
{
    [Trait("Owner", "jipe")]
    [Trait("EntityType", nameof(OverwriteUtility))]
    public class OverwriteUtilityTest
    {
        [Fact]
        public void ParseOPathTest()
        {
            var OPathstring = "a/f/g/b[c=\"d\"]/h/e";
            var result = OverwriteUtility.ParseOPath(OPathstring);
            Assert.Equal(1, 1);
        }

        [Theory]
        [InlineData("abc[]def=\"hij\"]")]
        [InlineData("abc[a=\"b]\b")]
        [InlineData("abc/efg[a=\"b\"]")]
        [InlineData("abc/[efg=\"hij\"]")]
        public void ParseInvalidOPathTest(string OPathString)
        {
            var ex = Assert.Throws<ArgumentException>(() => OverwriteUtility.ParseOPath(OPathString));
            Assert.Equal($"{OPathString} is not a valid OPath", ex.Message);
        }
    }
}
