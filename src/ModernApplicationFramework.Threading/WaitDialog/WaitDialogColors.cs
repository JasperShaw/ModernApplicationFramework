using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    public static class WaitDialogColors
    {
        private static ComponentResourceKey _waitDialogActiveShadowAndBorder;
        private static ComponentResourceKey _waitDialogTitleBarBackground;
        private static ComponentResourceKey _waitDialogTitleBarForeground;
        private static ComponentResourceKey _waitDialogMessageForeground;
        private static ComponentResourceKey _waitDialogBackground;

        public static ComponentResourceKey WaitDialogActiveShadowAndBorder => _waitDialogActiveShadowAndBorder ??
                                                                              (_waitDialogActiveShadowAndBorder = new ComponentResourceKey(typeof(WaitDialogColors), "WaitDialogActiveShadowAndBorder"));

        public static ComponentResourceKey WaitDialogTitleBarBackground => _waitDialogTitleBarBackground ??
                                                                           (_waitDialogTitleBarBackground = new ComponentResourceKey(typeof(WaitDialogColors), "WaitDialogTitleBarBackground"));

        public static ComponentResourceKey WaitDialogTitleBarForeground => _waitDialogTitleBarForeground ??
                                                                           (_waitDialogTitleBarForeground = new ComponentResourceKey(typeof(WaitDialogColors), "WaitDialogTitleBarForeground"));

        public static ComponentResourceKey WaitDialogMessageForeground => _waitDialogMessageForeground ??
                                                                          (_waitDialogMessageForeground = new ComponentResourceKey(typeof(WaitDialogColors), "WaitDialogMessageForeground"));

        public static ComponentResourceKey WaitDialogBackground => _waitDialogBackground ??
                                                                   (_waitDialogBackground = new ComponentResourceKey(typeof(WaitDialogColors), "WaitDialogBackground"));
    }
}
