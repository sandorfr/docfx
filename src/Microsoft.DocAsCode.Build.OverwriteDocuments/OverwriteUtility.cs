// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Build.OverwriteDocuments
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class OverwriteUtility
    {
        private static readonly Regex OPathRegex =
            new Regex(
                @"^(?<propertyName>\w+)(\[(?<Key>\w+)=""(?<Value>[\w\(\)\.\{\}\[\]\|\*\^/~#@!`,_<>:]+)""\])?)[/$]",
                RegexOptions.Compiled);

        public static List<OPathSegment> ParseOPath(string OPathString)
        {
            if (string.IsNullOrEmpty(OPathString))
            {
                throw new ArgumentException("OPathString cannot be null or empty.", nameof(OPathString));
            }

            var match = OPathRegex.Match(OPathString);
            if (match.Success)
            {
                return new List<OPathSegment>();
            }
            else
            {
                throw new ArgumentException($"{OPathString} is not a valid OPath");
            }

            //var segments = OPathString.Split('/');
            //var OPathSegments = new List<OPathSegment>();
            //foreach (var segment in segments)
            //{
            //    var index = segment.IndexOf('[');
            //    switch (index)
            //    {
            //        case -1:
            //            OPathSegments.Add(new OPathSegment
            //            {
            //                SegmentName = segment,
            //                OriginalSegmentString = segment
            //            });
            //            break;
            //        case 0:
            //            throw new ArgumentException($"There is a invalid segment {segment}");
            //        default:
            //            var keyValue = segment.Substring(index).Trim('[', ']').Split('=');
            //            if (keyValue.Length == 2)
            //            {
            //                OPathSegments.Add(new OPathSegment
            //                {
            //                    SegmentName = segment.Remove(index),
            //                    key = keyValue[0].Trim(' '),
            //                    Value = keyValue[1].Trim(' ', '"'),
            //                    OriginalSegmentString = segment
            //                });
            //            }
            //            else
            //            {
            //                throw new ArgumentException($"There is a invalid segment {segment}");
            //            }
            //            break;
            //    }
            //}
            //return OPathSegments;
        }
    }
}
