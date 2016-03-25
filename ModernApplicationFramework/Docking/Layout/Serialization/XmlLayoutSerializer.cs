/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout.Serialization
{
    public class XmlLayoutSerializer : LayoutSerializer
    {
        public XmlLayoutSerializer(DockingManager manager)
            : base(manager)
        { 

        }

        public void Serialize(System.Xml.XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(writer, Manager.Layout);
        }
        public void Serialize(TextWriter writer)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(writer, Manager.Layout);
        }
        public void Serialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(LayoutRoot));
            serializer.Serialize(stream, Manager.Layout);
        }

        public void Serialize(string filepath)
        {
            using (var stream = new StreamWriter(filepath))
                Serialize(stream);
        }

        public void Deserialize(Stream stream)
        {
            try
            {
                StartDeserialization();
                var serializer = new XmlSerializer(typeof(LayoutRoot));
                var layout = serializer.Deserialize(stream) as LayoutRoot;
                FixupLayout(layout);
                Manager.Layout = layout;
            }
            finally
            {
                EndDeserialization();
            }
        }

        public void Deserialize(TextReader reader)
        {
            try
            {
                StartDeserialization();

                LayoutRoot layout = new LayoutRoot();
                XmlReader xmlReader = new XmlTextReader(reader);
                layout.XmlDeserialize(xmlReader);

                //var serializer = new XmlSerializer(typeof(LayoutRoot));
                //var layout = serializer.Deserialize(reader) as LayoutRoot;

                FixupLayout(layout);
                Manager.Layout = layout;
            }
            finally
            {
                //
            }
        }

        public void Deserialize(System.Xml.XmlReader reader)
        {
            try
            {
                StartDeserialization();
                var serializer = new XmlSerializer(typeof(LayoutRoot));
                var layout = serializer.Deserialize(reader) as LayoutRoot;
                FixupLayout(layout);
                Manager.Layout = layout;
            }
            finally
            {
                EndDeserialization();
            }
        }

        public void Deserialize(string filepath)
        {
            using (var stream = new StreamReader(filepath))
                Deserialize(stream);
        }
    }
}
