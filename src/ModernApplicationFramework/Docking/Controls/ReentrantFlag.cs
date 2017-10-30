/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class ReentrantFlag
    {
        private bool _flag;
        public bool CanEnter => !_flag;

        public ReentrantFlagHandler Enter()
        {
            if (_flag)
                throw new InvalidOperationException();
            return new ReentrantFlagHandler(this);
        }

        public class ReentrantFlagHandler : IDisposable
        {
            private readonly ReentrantFlag _owner;

            public ReentrantFlagHandler(ReentrantFlag owner)
            {
                _owner = owner;
                _owner._flag = true;
            }

            public void Dispose()
            {
                _owner._flag = false;
            }
        }
    }
}