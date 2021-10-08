using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DataContractAttribute = System.Runtime.Serialization.DataContractAttribute;
using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;
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
    public class SerilizationTests
    {
        [DataContract]
        public class SampleDto : IExtensibleDataObject
        {
            [DataMember(Order = 1)]
            public string Name { get; set; }
            public ExtensionDataObject ExtensionData { get; set; }
        }


        [TestMethod]
        public void TestMethod1()
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
    }
}
