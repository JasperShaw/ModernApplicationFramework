using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ModernApplicationFramework.Caliburn.Collections;

namespace ModernApplicationFramework.Caliburn.Platform.Core
{
    public class NameTransformer : BindableCollection<NameTransformer.Rule>
    {
        private const RegexOptions Options = RegexOptions.None;

        bool _useEagerRuleSelection = true;

        /// <summary>
        /// Flag to indicate if transformations from all matched rules are returned. Otherwise, transformations from only the first matched rule are returned.
        /// </summary>
        public bool UseEagerRuleSelection
        {
            get { return _useEagerRuleSelection; }
            set { _useEagerRuleSelection = value; }
        }

        /// <summary>
        ///  Adds a transform using a single replacement value and a global filter pattern.
        /// </summary>
        /// <param name = "replacePattern">Regular expression pattern for replacing text</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <param name = "globalFilterPattern">Regular expression pattern for global filtering</param>
        public void AddRule(string replacePattern, string replaceValue, string globalFilterPattern = null)
        {
            AddRule(replacePattern, new[] {replaceValue}, globalFilterPattern);
        }

        /// <summary>
        ///  Adds a transform using a list of replacement values and a global filter pattern.
        /// </summary>
        /// <param name = "replacePattern">Regular expression pattern for replacing text</param>
        /// <param name = "replaceValueList">The list of replacement values</param>
        /// <param name = "globalFilterPattern">Regular expression pattern for global filtering</param>
        public void AddRule(string replacePattern, IEnumerable<string> replaceValueList,
            string globalFilterPattern = null)
        {
            Add(new Rule
            {
                ReplacePattern = replacePattern,
                ReplacementValues = replaceValueList,
                GlobalFilterPattern = globalFilterPattern
            });
        }

        /// <summary>
        /// Gets the list of transformations for a given name.
        /// </summary>
        /// <param name = "source">The name to transform into the resolved name list</param>
        /// <returns>The transformed names.</returns>
        public IEnumerable<string> Transform(string source)
        {
            return Transform(source, r => r);
        }

        /// <summary>
        /// Gets the list of transformations for a given name.
        /// </summary>
        /// <param name = "source">The name to transform into the resolved name list</param>
        /// <param name = "getReplaceString">A function to do a transform on each item in the ReplaceValueList prior to applying the regular expression transform</param>
        /// <returns>The transformed names.</returns>
        public IEnumerable<string> Transform(string source, Func<string, string> getReplaceString)
        {
            var nameList = new List<string>();
            var rules = this.Reverse();

            foreach (
                var rule in
                    rules.Where(
                        rule =>
                            string.IsNullOrEmpty(rule.GlobalFilterPattern) ||
                            rule.GlobalFilterPatternRegex.IsMatch(source))
                        .Where(rule => rule.ReplacePatternRegex.IsMatch(source)))
            {
                nameList.AddRange(
                    rule.ReplacementValues
                        .Select(getReplaceString)
                        .Select(repString => rule.ReplacePatternRegex.Replace(source, repString))
                    );

                if (!_useEagerRuleSelection)
                {
                    break;
                }
            }

            return nameList;
        }

        ///<summary>
        /// A rule that describes a name transform.
        ///</summary>
        public class Rule
        {
            /// <summary>
            /// Regular expression pattern for global filtering
            /// </summary>
            public string GlobalFilterPattern;

            /// <summary>
            /// The list of replacement values
            /// </summary>
            public IEnumerable<string> ReplacementValues;

            /// <summary>
            /// Regular expression pattern for replacing text
            /// </summary>
            public string ReplacePattern;

            private Regex _globalFilterPatternRegex;
            private Regex _replacePatternRegex;

            /// <summary>
            /// Regular expression for global filtering
            /// </summary>
            public Regex GlobalFilterPatternRegex
                => _globalFilterPatternRegex ?? (_globalFilterPatternRegex = new Regex(GlobalFilterPattern, Options));

            /// <summary>
            /// Regular expression for replacing text
            /// </summary>
            public Regex ReplacePatternRegex
                => _replacePatternRegex ?? (_replacePatternRegex = new Regex(ReplacePattern, Options));
        }
    }
}