using System;
using System.Collections.Generic;
using System.Text;

namespace PackeetGenerator
{
    class PacketFormat
    {
        // {0} 패킷 등록
        public static string managerFormat = 
@"using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{{
    #region Singleton

    private static PacketManager _instance = new PacketManager();

    public static PacketManager Instance => _instance;

    public PacketManager()
    {{
        Register();
    }}

    #endregion

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv =
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler =
        new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {{
{0}
    }}

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        if (_handler.TryGetValue(pkt.Protocol, out var action))
        {{
            action?.Invoke(session, pkt);
        }}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var action))
        {{
            action?.Invoke(session, buffer);
        }}
    }}
}}";
        // {0} 패킷 이름
        public static string managerRegisterFormat =
@"        _onRecv.Add((ushort) PacketId.{0}, MakePacket<{0}>);
        _handler.Add((ushort) PacketId.{0}, PacketHandler.{0}Handler);";

        // {0} 패킷 이름/번호 목록
        // {1} 패킷 목록
        public static string fileFormat =
            @"using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketId
{{
    {0}
}}

interface IPacket
{{
    ushort Protocol {{ get; }}
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}}

{1}
";

        public static string packetEnumFormat = @"{0} = {1},";

        // {0} 패킷이름
        // {1} 멤버변수
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        public static string packetFormat =
            @"class {0} : IPacket
{{
    {1}

    public ushort Protocol => (ushort) PacketId.{0};

    public void Read(ArraySegment<byte> seg)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(seg.Array, seg.Offset, seg.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> seg = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(seg.Array, seg.Offset, seg.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort) PacketId.{0});
        count += sizeof(ushort);
        {3}
        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 번수 형식
        // {1} 번수 이름
        public static string memberFormat = @"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버변수
        // {3} 멤버 변수 Read
        // {4} 멤버 변수 Write
        public static string memberListFormat =
            @"
public class {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}

public List<{0}> {1}s = new List<{0}>();
";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string readListFormat =
            @"{1}s.Clear();
ushort {1}Length = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);;
for (var i = 0; i < {1}Length; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        public static string writeListFormat =
            @"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (short) {1}s.Count);
count += sizeof(ushort);

foreach ({0} skill in {1}s)
    success &= {1}.Write(s, ref count);
";

        // {0} 번수이름
        // {1} To~ 변수 형식
        // {2} 변수 형식
        public static string readFormat =
            @"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});";

        // {0} 번수이름
        // {1} 변수 형식
        public static string readByteFormat =
            @"this.{0} = ({1})seg.Array[seg.Offset + count];
count += sizeof({1});";

        // {0} 번수이름
        // {1} 변수 형식
        public static string writeByteFormat =
            @"seg.Array[seg.Offset + count] = (byte)this.{0};
count += sizeof({1});";

        // {0} 변수 이름
        public static string readStringFormat =
            @"ushort {0}Length = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Length));
count += {0}Length;";

        // {0} 번수 이름
        // {1} 변수 형식
        public static string writeFormat =
            @"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});";

        // {0} 변수 이름
        public static string writeStringFormat =
            @"ushort {0}Length = (ushort) Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, seg.Array,
seg.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Length);
count += sizeof(ushort);
count += {0}Length;";
    }
}