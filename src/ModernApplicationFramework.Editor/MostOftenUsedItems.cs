using System;
using System.Collections.Generic;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    internal static class MostOftenUsedItems
    {
        internal static Dictionary<string, int> TextEditorMouItems = new Dictionary<string, int>
        {
            {
                "Selected Text", 0
            },
            {
                "Inactive Selected Text", 0
            },
            {
                "Indicator Margin", 0
            },
            //{
            //    "User Types", 1
            //},
            //{
            //    "Keyword", 1
            //},
            //{
            //    "Identifier", 1
            //},
            //{
            //    "Operator", 1
            //},
            //{
            //    "Collapsible Text (Collapsed)",
            //    2
            //},
            {
                "Track Changes before save", 2
            },
            {
                "Track Changes after save", 2
            },
            {
                "Track reverted changes", 3
            },
            //{
            //    "outlining.collapsehintadornment", 3
            //},
            //{
            //    "outlining.verticalrule", 3
            //},
            //{
            //    "outlining.square", 3
            //},
            {
                "CurrentLineActiveFormat", 3
            },
            {
                "CurrentLineInactiveFormat", 3
            }
        };
        internal static Guid[] TextEditorCategories = {
            CategoryGuids.GuidEditorTextManager,
            //CategoryGuids.GuidEditorLanguageService,
            //CategoryGuids.GuidEditorTextMarker,
            CategoryGuids.GuidEditorMef
        };
    }
}