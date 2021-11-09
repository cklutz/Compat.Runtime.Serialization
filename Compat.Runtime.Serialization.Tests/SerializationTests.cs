using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DataContractAttribute = System.Runtime.Serialization.DataContractAttribute;
using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;
using System;
#if NETFRAMEWORK
using IExtensibleDataObject = System.Runtime.Serialization.IExtensibleDataObject;
using ExtensionDataObject = System.Runtime.Serialization.ExtensionDataObject;
#else
using IExtensibleDataObject = System.Runtime.Serialization.IExtensibleDataObject;
using ExtensionDataObject = System.Runtime.Serialization.ExtensionDataObject;
#endif

namespace Compat.Runtime.Serialization.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [DataContract]
        public class SampleDto : IExtensibleDataObject
        {
            [DataMember(Order = 1)]
            public string Name { get; set; }
            public ExtensionDataObject ExtensionData { get; set; }
        }


        [TestMethod]
        public void Deserializes_Serialized_Data()
        {
            var obj = new SampleDto { Name = "Test " };

            var ser = new System.Runtime.Serialization.NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, obj);
                stream.Position = 0;

                var result = (SampleDto)ser.Deserialize(stream);

                Assert.AreEqual(obj.Name, result.Name);
            }
        }

#if NETFRAMEWORK
        [TestMethod]
#endif
        public void NetFramework_Deserializes_NetCore()
        {
            // .NET 5 Serialized Result Of:
            // var obj = new SampleDto { Name = "Test " };
            byte[] netCoreSerialized = StringToByteArray("3C536572696C697A6174696F6E54657374732E53616D706C6544746F207A3A49643D223122207A3A547970653D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732E536572696C697A6174696F6E54657374732B53616D706C6544746F22207A3A417373656D626C793D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C2220786D6C6E733D22687474703A2F2F736368656D61732E64617461636F6E74726163742E6F72672F323030342F30372F436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732220786D6C6E733A693D22687474703A2F2F7777772E77332E6F72672F323030312F584D4C536368656D612D696E7374616E63652220786D6C6E733A7A3D22687474703A2F2F736368656D61732E6D6963726F736F66742E636F6D2F323030332F31302F53657269616C697A6174696F6E2F223E3C4E616D65207A3A49643D2232223E54657374203C2F4E616D653E3C2F536572696C697A6174696F6E54657374732E53616D706C6544746F3E");
            var ser = new System.Runtime.Serialization.NetDataContractSerializer();

            using (var stream = new MemoryStream(netCoreSerialized))
            {
                var result = (SampleDto)ser.Deserialize(stream);

                Assert.AreEqual("Test ", result.Name);
            }
        }

#if !NETFRAMEWORK
        [TestMethod]
#endif
        public void NetCore_Deserializes_NetFramework()
        {
            // .NET Framework Serialized Result Of:
            // var obj = new SampleDto { Name = "Test " };
            byte[] netFrameworkSerialized = StringToByteArray("3C536572696C697A6174696F6E54657374732E53616D706C6544746F207A3A49643D223122207A3A547970653D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732E536572696C697A6174696F6E54657374732B53616D706C6544746F22207A3A417373656D626C793D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C2220786D6C6E733D22687474703A2F2F736368656D61732E64617461636F6E74726163742E6F72672F323030342F30372F436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732220786D6C6E733A693D22687474703A2F2F7777772E77332E6F72672F323030312F584D4C536368656D612D696E7374616E63652220786D6C6E733A7A3D22687474703A2F2F736368656D61732E6D6963726F736F66742E636F6D2F323030332F31302F53657269616C697A6174696F6E2F223E3C4E616D65207A3A49643D2232223E54657374203C2F4E616D653E3C2F536572696C697A6174696F6E54657374732E53616D706C6544746F3E");
            var ser = new System.Runtime.Serialization.NetDataContractSerializer();

            using (var stream = new MemoryStream(netFrameworkSerialized))
            {
                var result = (SampleDto)ser.Deserialize(stream);

                Assert.AreEqual("Test ", result.Name);
            }
        }

        // From: https://stackoverflow.com/a/311179/448
        // Used to temporarily write to Console.Out during tests to capture serialized data for later tests
        private static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        // From: https://stackoverflow.com/a/311179/448
        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }
}
