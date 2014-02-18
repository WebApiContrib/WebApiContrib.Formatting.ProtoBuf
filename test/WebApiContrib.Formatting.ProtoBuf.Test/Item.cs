using ProtoBuf;

namespace WebApiContrib.Formatting.ProtoBuf.Test
{
    [ProtoContract]
    public class Item
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
    }
}