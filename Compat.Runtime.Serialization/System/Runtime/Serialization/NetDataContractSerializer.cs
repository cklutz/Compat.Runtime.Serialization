#if NETFRAMEWORK
[assembly: System.Runtime.CompilerServices.TypeForwardedToAttribute(typeof(System.Runtime.Serialization.NetDataContractSerializer))]
#else
using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Xml;

namespace System.Runtime.Serialization
{
    public class NetDataContractSerializer : XmlObjectSerializer
    {
        private readonly Compat.Runtime.Serialization.NetDataContractSerializer m_impl;

        public NetDataContractSerializer()
            : this(new StreamingContext(StreamingContextStates.All))
        {
        }

        public NetDataContractSerializer(StreamingContext context)
            : this(context, int.MaxValue, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(StreamingContext context,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            FormatterAssemblyStyle assemblyFormat,
            ISurrogateSelector surrogateSelector)
        {
            m_impl = new Compat.Runtime.Serialization.NetDataContractSerializer(context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }

        public NetDataContractSerializer(string rootName, string rootNamespace)
            : this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), int.MaxValue, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(string rootName, string rootNamespace,
            StreamingContext context,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            FormatterAssemblyStyle assemblyFormat,
            ISurrogateSelector surrogateSelector)
        {
            XmlDictionary dictionary = new XmlDictionary(2);
            m_impl = new Compat.Runtime.Serialization.NetDataContractSerializer(rootName, rootNamespace, context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }

        public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
            : this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), int.MaxValue, false, FormatterAssemblyStyle.Full, null)
        {
        }

        public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace,
            StreamingContext context,
            int maxItemsInObjectGraph,
            bool ignoreExtensionDataObject,
            FormatterAssemblyStyle assemblyFormat,
            ISurrogateSelector surrogateSelector)
        {
            m_impl = new Compat.Runtime.Serialization.NetDataContractSerializer(rootName, rootNamespace, context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
        }


        public StreamingContext Context
        {
            get => m_impl.Context;
            set => m_impl.Context = value;
        }

        public SerializationBinder Binder
        {
            get => m_impl.Binder;
            set => m_impl.Binder = value;
        }

        public ISurrogateSelector SurrogateSelector
        {
            get => m_impl.SurrogateSelector;
            set => m_impl.SurrogateSelector = value;
        }

        public FormatterAssemblyStyle AssemblyFormat
        {
            get => m_impl.AssemblyFormat;
            set => m_impl.AssemblyFormat = value;
        }

        public int MaxItemsInObjectGraph => m_impl.MaxItemsInObjectGraph;

        public bool IgnoreExtensionDataObject => m_impl.IgnoreExtensionDataObject;

        public void Serialize(Stream stream, object graph) => m_impl.Serialize(stream, graph);

        public object Deserialize(Stream stream) => m_impl.Deserialize(stream);

        public object Deserialize(XmlReader reader) => m_impl.Deserialize(reader);

        public override void WriteObject(XmlWriter writer, object graph) => m_impl.WriteObject(writer, graph);

        public override void WriteStartObject(XmlWriter writer, object graph) => m_impl.WriteStartObject(writer, graph);

        public override void WriteObjectContent(XmlWriter writer, object graph) => m_impl.WriteObjectContent(writer, graph);

        public override void WriteEndObject(XmlWriter writer) => m_impl.WriteEndObject(writer);

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph) => m_impl.WriteStartObject(writer, graph);

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph) => m_impl.WriteObjectContent(writer, graph);

        public static bool UnsafeTypeForwardingEnabled => Compat.Runtime.Serialization.NetDataContractSerializer.UnsafeTypeForwardingEnabled;

        public override void WriteEndObject(XmlDictionaryWriter writer) => WriteEndObject(writer);

        public override object ReadObject(XmlReader reader) => m_impl.ReadObject(reader);

        public override object ReadObject(XmlReader reader, bool verifyObjectName) => m_impl.ReadObject(reader, verifyObjectName);

        public override bool IsStartObject(XmlReader reader) => m_impl.IsStartObject(reader);

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName) => m_impl.ReadObject(reader, verifyObjectName);

        public override bool IsStartObject(XmlDictionaryReader reader) => m_impl.IsStartObject(reader);

    }
}
#endif
