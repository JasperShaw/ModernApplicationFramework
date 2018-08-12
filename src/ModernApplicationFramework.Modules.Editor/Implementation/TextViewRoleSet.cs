using System;
using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class TextViewRoleSet : ITextViewRoleSet, IEnumerable<string>
    {
        private readonly List<string> _roles;

        public TextViewRoleSet(IEnumerable<string> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            _roles = new List<string>();
            foreach (var role in roles)
            {
                if (role == null)
                    throw new ArgumentNullException(nameof(roles));
                _roles.Add(role.ToUpperInvariant());
            }
        }

        public bool Contains(string textViewRole)
        {
            if (textViewRole == null)
                throw new ArgumentNullException(nameof(textViewRole));
            var upperInvariant = textViewRole.ToUpperInvariant();
            foreach (var role in _roles)
                if (role == upperInvariant)
                    return true;
            return false;
        }

        public bool ContainsAll(IEnumerable<string> textViewRoles)
        {
            if (textViewRoles == null)
                throw new ArgumentNullException(nameof(textViewRoles));
            foreach (var textViewRole in textViewRoles)
                if (textViewRole != null)
                {
                    var flag = false;
                    var upperInvariant = textViewRole.ToUpperInvariant();
                    foreach (var role in _roles)
                        if (role == upperInvariant)
                        {
                            flag = true;
                            break;
                        }

                    if (!flag)
                        return false;
                }

            return true;
        }

        public bool ContainsAny(IEnumerable<string> textViewRoles)
        {
            if (textViewRoles == null)
                throw new ArgumentNullException(nameof(textViewRoles));
            foreach (var textViewRole in textViewRoles)
                if (textViewRole != null)
                {
                    var upperInvariant = textViewRole.ToUpperInvariant();
                    foreach (var role in _roles)
                        if (role == upperInvariant)
                            return true;
                }

            return false;
        }

        public override string ToString()
        {
            return string.Join(",", _roles);
        }

        public ITextViewRoleSet UnionWith(ITextViewRoleSet roleSet)
        {
            if (roleSet == null)
                throw new ArgumentNullException(nameof(roleSet));
            var stringSet = new HashSet<string>(_roles);
            foreach (var role in (IEnumerable<string>) roleSet)
                if (!stringSet.Contains(role))
                    stringSet.Add(role);
            return new TextViewRoleSet(stringSet);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            foreach (var role in _roles)
                yield return role;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var role in _roles)
                yield return role;
        }
    }
}