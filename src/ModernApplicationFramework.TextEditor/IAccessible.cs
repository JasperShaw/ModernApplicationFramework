using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor
{
    [Guid("9C6A644C-CB7E-4F9E-9ACC-EFC464D23A1A")]
    [TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IAccessible
    {
        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <returns>An object.</returns>
        [DispId(-5000)]
        object accParent { [TypeLibFunc(TypeLibFuncFlags.FHidden), DispId(-5000), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.IDispatch)] get; }

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <returns>An integer representing the count.</returns>
        [DispId(-5001)]
        int accChildCount { [TypeLibFunc(TypeLibFuncFlags.FHidden), DispId(-5001), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

        [DispId(-5002)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        object get_accChild([MarshalAs(UnmanagedType.Struct), In] object varChild);

        [DispId(-5003)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accName([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5004)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accValue([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5005)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accDescription([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5006)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object get_accRole([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5007)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object get_accState([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5008)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accHelp([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5009)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int get_accHelpTopic([MarshalAs(UnmanagedType.BStr)] out string pszHelpFile, [MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [DispId(-5010)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accKeyboardShortcut([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <returns>If successful, returns S_OK. Otherwise, returns another standard COM error code.</returns>
        [DispId(-5011)]
        object accFocus { [TypeLibFunc(TypeLibFuncFlags.FHidden), DispId(-5011), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Struct)] get; }

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <returns>An object.</returns>
        [DispId(-5012)]
        object accSelection { [TypeLibFunc(TypeLibFuncFlags.FHidden), DispId(-5012), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Struct)] get; }

        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5013)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string get_accDefaultAction([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <param name="flagsSelect">This parameter is intended for internal use only.</param>
        /// <param name="varChild">This parameter is intended for internal use only.</param>
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5014)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void accSelect([In] int flagsSelect, [MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <param name="pxLeft">This parameter is intended for internal use only.</param>
        /// <param name="pyTop">This parameter is intended for internal use only.</param>
        /// <param name="pcxWidth">This parameter is intended for internal use only.</param>
        /// <param name="pcyHeight">This parameter is intended for internal use only.</param>
        /// <param name="varChild">This parameter is intended for internal use only.</param>
        [DispId(-5015)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, [MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <param name="navDir">This parameter is intended for internal use only.</param>
        /// <param name="varStart">This parameter is intended for internal use only.</param>
        /// <returns>If successful, returns S_OK. For other possible return values, see the documentation for <see langword="IAccessible::accNavigate" />.</returns>
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5016)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object accNavigate([In] int navDir, [MarshalAs(UnmanagedType.Struct), In, Optional] object varStart);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <param name="xLeft">This parameter is intended for internal use only.</param>
        /// <param name="yTop">This parameter is intended for internal use only.</param>
        /// <returns>An object.</returns>
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5017)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object accHitTest([In] int xLeft, [In] int yTop);

        /// <summary>The <see cref="T:Accessibility.IAccessible" /> interface and all of its exposed members are part of a managed wrapper for the Component Object Model (COM) <see langword="IAccessible" /> interface.</summary>
        /// <param name="varChild">This parameter is intended for internal use only.</param>
        [DispId(-5018)]
        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void accDoDefaultAction([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild);

        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5003)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void set_accName([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild, [MarshalAs(UnmanagedType.BStr), In] string pszName);

        [TypeLibFunc(TypeLibFuncFlags.FHidden)]
        [DispId(-5004)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void set_accValue([MarshalAs(UnmanagedType.Struct), In, Optional] object varChild, [MarshalAs(UnmanagedType.BStr), In] string pszValue);
    }
}