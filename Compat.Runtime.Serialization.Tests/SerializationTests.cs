using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DataContractAttribute = System.Runtime.Serialization.DataContractAttribute;
using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text.RegularExpressions;
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

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Roundtrip_Dictionary_WithComparer(bool ignoreCase)
        {
            var dict = new Dictionary<string, string>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
            dict.Add("Test1", "Value1");
            dict.Add("Test2", "Value2");

            var ser = new System.Runtime.Serialization.NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, dict);
                // Used when generating hex bytes for other tests when you make a change
                // System.Diagnostics.Debug.WriteLine(ByteArrayToString(stream.ToArray()));
                stream.Position = 0;

                Console.WriteLine("------------------------");
                Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
                Console.WriteLine("------------------------");

                var result = (Dictionary<string, string>)ser.Deserialize(stream);

                Assert.AreEqual(dict.Count, result.Count);
                Assert.AreEqual(dict["Test1"], result["Test1"]);
                Assert.AreEqual(dict["Test2"], result["Test2"]);
            }
        }

        private const string ComparerSampleFullFramework = @"<OrdinalComparer z:Id=""1"" z:Type=""System.OrdinalComparer"" z:Assembly=""0"" xmlns=""http://schemas.datacontract.org/2004/07/System"" " +
            @"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/""> " +
            @"<_ignoreCase>true</_ignoreCase></OrdinalComparer>";

        private const string ComparerSampleNetCore = @"<OrdinalIgnoreCaseComparer z:Id=""1"" z:FactoryType=""OrdinalComparer"" " +
            @"z:Type=""System.OrdinalComparer"" z:Assembly=""mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" " +
            @"xmlns=""http://schemas.datacontract.org/2004/07/System"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" " +
            @"xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"">" +
            // This is what Compat writes, but ...
            //@" <_ignoreCase z:Id=""2"" z:Type=""System.Boolean"" z:Assembly=""mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" xmlns="""">true</_ignoreCase>" + 
            // This is what Compat read expects:
            @"<_ignoreCase>true</_ignoreCase>" +
            @" </OrdinalIgnoreCaseComparer>";

        [TestMethod]
        public void CanSerialize_Correctly()
        {
            var obj = StringComparer.OrdinalIgnoreCase;

            using (var stream = new MemoryStream())
            {
                var ser = new System.Runtime.Serialization.NetDataContractSerializer();
                ser.Serialize(stream, obj);
                stream.Position = 0;
                Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

                var res = ser.Deserialize(stream);
                Assert.AreEqual(obj.GetType(), res.GetType());
            }
        }

        [DataTestMethod]
        [DataRow(ComparerSampleFullFramework, DisplayName = ".NET Framework")]
        [DataRow(ComparerSampleNetCore, DisplayName = ".NET Core")]
        public void CanDeserialize_OtherClr(string data)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var ser = new System.Runtime.Serialization.NetDataContractSerializer();

                var result = (IEqualityComparer<string>)ser.Deserialize(stream);
                Console.WriteLine(result.GetType());

                Assert.IsTrue(result.Equals("HI", "hi"));
            }
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Roundtrip_Custom_WithComparer(bool ignoreCase)
        {
            var comparer = (ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

            var ser = new System.Runtime.Serialization.NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, comparer);
                // Used when generating hex bytes for other tests when you make a change
                // System.Diagnostics.Debug.WriteLine(ByteArrayToString(stream.ToArray()));
                stream.Position = 0;

                Console.WriteLine("------------------------");
                Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
                Console.WriteLine("------------------------");

                var result = (StringComparer)ser.Deserialize(stream);
                Console.WriteLine(result);
            }
        }

        [TestMethod]
        public void Deserializes_Serialized_Data()
        {
            var obj = new SampleDto { Name = "Test " };

            var ser = new System.Runtime.Serialization.NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, obj);
                // Used when generating hex bytes for other tests when you make a change
                // System.Diagnostics.Debug.WriteLine(ByteArrayToString(stream.ToArray()));
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
            byte[] netCoreSerialized = StringToByteArray("3C53657269616C697A6174696F6E54657374732E53616D706C6544746F207A3A49643D223122207A3A547970653D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732E53657269616C697A6174696F6E54657374732B53616D706C6544746F22207A3A417373656D626C793D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C2220786D6C6E733D22687474703A2F2F736368656D61732E64617461636F6E74726163742E6F72672F323030342F30372F436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732220786D6C6E733A693D22687474703A2F2F7777772E77332E6F72672F323030312F584D4C536368656D612D696E7374616E63652220786D6C6E733A7A3D22687474703A2F2F736368656D61732E6D6963726F736F66742E636F6D2F323030332F31302F53657269616C697A6174696F6E2F223E3C4E616D65207A3A49643D2232223E54657374203C2F4E616D653E3C2F53657269616C697A6174696F6E54657374732E53616D706C6544746F3E");
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
            byte[] netFrameworkSerialized = StringToByteArray("3C53657269616C697A6174696F6E54657374732E53616D706C6544746F207A3A49643D223122207A3A547970653D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732E53657269616C697A6174696F6E54657374732B53616D706C6544746F22207A3A417373656D626C793D22436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C2220786D6C6E733D22687474703A2F2F736368656D61732E64617461636F6E74726163742E6F72672F323030342F30372F436F6D7061742E52756E74696D652E53657269616C697A6174696F6E2E54657374732220786D6C6E733A693D22687474703A2F2F7777772E77332E6F72672F323030312F584D4C536368656D612D696E7374616E63652220786D6C6E733A7A3D22687474703A2F2F736368656D61732E6D6963726F736F66742E636F6D2F323030332F31302F53657269616C697A6174696F6E2F223E3C4E616D65207A3A49643D2232223E54657374203C2F4E616D653E3C2F53657269616C697A6174696F6E54657374732E53616D706C6544746F3E");
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
