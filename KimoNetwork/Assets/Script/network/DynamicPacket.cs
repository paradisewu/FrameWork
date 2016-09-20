using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public interface IDynamicData
    {
        void Serialize(DynamicPacket packet);
        void Deserialize(DynamicPacket packet);
    }

    public class DynamicPacket
    {
        public DynamicPacket()
	    {
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
            writer = new BinaryWriter(memStream);
	    }

        public DynamicPacket(Byte[] bytes, int offset,	int count) 
            : this()
        {
            memStream.Write(bytes, offset, count);
            memStream.Seek(0, SeekOrigin.Begin);
        }

        public DynamicPacket(Byte[] bytes)
            : this()
        {
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
        }
		
		// 警告：不要用这个数组的长度。请用GetLength
        public virtual Byte[] GetBuffer()
        {
            return memStream.GetBuffer();
        }
		
		// 数组真实长度正确的函数, 但有内存拷贝的效率问题
        public virtual Byte[] ToArray()
        {
            return memStream.ToArray();
        }

		public virtual int GetLength()
        {
            return (int)memStream.Length;
        }
        
        public void Seek(long offset, SeekOrigin loc)
        {
            memStream.Seek(offset, loc);
        }
 
        public Char ReadChar()
        {
            return reader.ReadChar();
        }
        public void Read(out Char c)
        {
            c = reader.ReadChar();
        }
        public void Write(Char c)
        {
            writer.Write(c);
        }

        public Char[] ReadChars()
        {
            UInt16 count = reader.ReadUInt16();
            return reader.ReadChars(count);
        }
        public Char[] ReadChars(int count)
        {
            return reader.ReadChars(count);
        }
        public void Write(Char[] chars, int count)
        {
            writer.Write(chars, 0, count);
        }
        public void Read(out Char[] chars)
        {
            UInt16 count = reader.ReadUInt16();
            chars = reader.ReadChars(count);
        }
        public void Write(Char[] chars)
        {
            writer.Write((UInt16)chars.Length);
            writer.Write(chars);
        }

        public string ReadString()
        {
            int count = ReadUInt16() - 1; //服务器对所有的字符串后面都加了'0'
            byte[] buf = ReadBytes(count);
            ReadByte();     
            return Encoding.UTF8.GetString(buf);
        }

        public void Read(out string str)
        {
            str = ReadString();
        }

        public void Write(string str)
        {
            byte[] buf = Encoding.UTF8.GetBytes(str);
            Write((UInt16)(buf.Length + 1));
            Write(buf, buf.Length);
            Write((Byte)0);
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }
        public void Read(out bool b)
        {
            b = reader.ReadBoolean();
        }
        public void Write(bool b)
        {
            writer.Write(b);
        }
        
        public Byte ReadByte()
        {
            return reader.ReadByte();
        }
        public void Read(out Byte b)
        {
            b = reader.ReadByte();
        }
        public void Write(Byte b)
        {
            writer.Write(b);
        }

        public SByte ReadSByte()
        {
            return reader.ReadSByte();
        }
        public void Read(out SByte b)
        {
            b = reader.ReadSByte();
        }
        public void Write(SByte b)
        {
            writer.Write(b);
        }

        public Byte[] ReadBytes()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadBytes(count);
        }
        public void Read(out Byte[] bytes)
        {
            bytes = ReadBytes();
        }
        public Byte[] ReadBytes(int count)
        {
            Byte[] bytes = new Byte[count];
            for (Int32 i = 0; i < count; ++i)
            {
                bytes[i] = reader.ReadByte();
            }
            return bytes;
        }
        public void Read(out Byte[] bytes, Int32 count)
        {
            bytes = ReadBytes(count);
        }
        public void Write(Byte[] bytes, int count)
        {
            int n = Math.Min(bytes.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(bytes[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((Byte)0);
            }
        }
        public void Write(Byte[] b)
        {
            writer.Write((UInt16)b.Length);
            Write(b, b.Length);
        }

        public SByte[] ReadSBytes()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadSBytes(count);
        }
        public void Read(out SByte[] bytes)
        {
            bytes = ReadSBytes();
        }
        public SByte[] ReadSBytes(Int32 count)
        {
            SByte[] bytes = new SByte[count];
            for (Int32 i = 0; i < count; ++i)
            {
                bytes[i] = reader.ReadSByte();
            }
            return bytes;
        }
        public void Read(out SByte[] bytes, Int32 count)
        {
            bytes = ReadSBytes(count);
        }
        public void Write(SByte[] bytes)
        {
            writer.Write((UInt16)bytes.Length);
            Write(bytes, bytes.Length);
        }
        public void Write(SByte[] bytes, Int32 count)
        {
            int n = Math.Min(bytes.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(bytes[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((SByte)0);
            }
        }

        public Int16[] ReadShorts()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadShorts(count);
        }
        public Int16[] ReadShorts(Int32 count)
        {
            Int16[] shorts = new Int16[count];
            for (int i = 0; i < count; ++i)
            {
                shorts[i] = reader.ReadInt16();
            }
            return shorts;
        }
        public void Read(out Int16[] shorts)
        {
            shorts = ReadShorts();
        }
        public void Read(out Int16[] shorts, Int32 count)
        {
            shorts = ReadShorts(count);
        }
        public void Write(Int16[] shorts)
        {
            UInt16 count = (UInt16)shorts.Length;
            writer.Write(count);
            Write(shorts, count);
        }
        public void Write(Int16[] shorts, Int32 count)
        {
            int n = Math.Min(shorts.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(shorts[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((Int16)0);
            }
        }

        public UInt16[] ReadUShorts()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadUShorts(count);
        }
        public UInt16[] ReadUShorts(Int32 count)
        {
            UInt16[] ushorts = new UInt16[count];
            for (int i = 0; i < count; ++i)
            {
                ushorts[i] = reader.ReadUInt16();
            }
            return ushorts;
        }
        public void Read(out UInt16[] ushorts)
        {
            ushorts = ReadUShorts();
        }
        public void Read(out UInt16[] ushorts, Int32 count)
        {
            ushorts = ReadUShorts(count);
        }
        public void Write(UInt16[] ushorts)
        {
            UInt16 count = (UInt16)ushorts.Length;
            writer.Write(count);
            Write(ushorts, count);
        }
        public void Write(UInt16[] ushorts, Int32 count)
        {
            int n = Math.Min(ushorts.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(ushorts[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((ushort)0);
            }
        }

        public Int32[] ReadInts()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadInts(count);
        }
        public Int32[] ReadInts(Int32 count)
        {
            Int32[] ints = new Int32[count];
            for (int i = 0; i < count; ++i)
            {
                ints[i] = reader.ReadInt32();
            }
            return ints;
        }
        public void Read(out Int32[] ints)
        {
            ints = ReadInts();
        }
        public void Read(out Int32[] ints, Int32 count)
        {
            ints = ReadInts(count);
        }
        public void Write(Int32[] ints)
        {
            UInt16 count = (UInt16)ints.Length;
            writer.Write(count);
            Write(ints, count);
        }
        public void Write(Int32[] ints, Int32 count)
        {
            int n = Math.Min(ints.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(ints[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((Int32)0);
            }
        }

        public UInt32[] ReadUInts()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadUInts(count);
        }
        public UInt32[] ReadUInts(Int32 count)
        {
            UInt32[] uints = new UInt32[count];
            for (Int32 i = 0; i < count; ++i)
            {
                uints[i] = reader.ReadUInt32();
            }
            return uints;
        }
        public void Read(out UInt32[] uints)
        {
            uints = ReadUInts();
        }
        public void Read(out UInt32[] uints, Int32 count)
        {
            uints = ReadUInts(count);
        }
        public void Write(UInt32[] uints)
        {
            UInt16 count = (UInt16)uints.Length;
            writer.Write(count);
            Write(uints, count);
        }
        public void Write(UInt32[] uints, Int32 count)
        {
            int n = Math.Min(uints.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(uints[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((UInt32)0);
            }
        }

        public Int64[] ReadLongs()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadLongs(count);
        }
        public Int64[] ReadLongs(Int32 count)
        {
            Int64[] longs = new Int64[count];
            for (Int32 i = 0; i < count; ++i)
            {
                longs[i] = reader.ReadInt64();
            }
            return longs;
        }
        public void Read(out Int64[] longs)
        {
            longs = ReadLongs();
        }
        public void Read(out Int64[] longs, Int32 count)
        {
            longs = ReadLongs(count);
        }
        public void Write(Int64[] longs)
        {
            UInt16 count = (UInt16)longs.Length;
            writer.Write(count);
            Write(longs, count);
        }
        public void Write(Int64[] longs, Int32 count)
        {
            int n = Math.Min(longs.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(longs[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((Int64)0);
            }
        }

        public UInt64[] ReadULongs()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadULongs(count);
        }
        public UInt64[] ReadULongs(Int32 count)
        {
            UInt64[] ulongs = new UInt64[count];
            for (Int32 i = 0; i < count; ++i)
            {
                ulongs[i] = reader.ReadUInt64();
            }
            return ulongs;
        }
        public void Read(out UInt64[] ulongs)
        {
            ulongs = ReadULongs();
        }
        public void Read(out UInt64[] ulongs, Int32 count)
        {
            ulongs = ReadULongs(count);
        }
        public void Write(UInt64[] ulongs)
        {
            UInt16 count = (UInt16)ulongs.Length;
            writer.Write(count);
            Write(ulongs, count);
        }
        public void Write(UInt64[] ulongs, Int32 count)
        {
            int n = Math.Min(ulongs.Length, count);
            int i = 0;
            for (; i < n; ++i)
            {
                writer.Write(ulongs[i]);
            }
            for (; i < count; ++i)
            {
                writer.Write((UInt64)0);
            }
        }

        public Int16 ReadInt16()
        {
            return reader.ReadInt16();
        }
        public void Read(out Int16 i)
        {
            i = reader.ReadInt16();
        }
        public void Write(Int16 i)
        {
            writer.Write(i);
        }

        public UInt16 ReadUInt16()
        {
            return reader.ReadUInt16();
        }
        public void Read(out UInt16 i)
        {
            i = reader.ReadUInt16();
        }
        public void Write(UInt16 i)
        {
            writer.Write(i);
        }

        public Int32 ReadInt32()
        {
            return reader.ReadInt32();
        }
        public void Read(out Int32 i)
        {
            i = reader.ReadInt32();
        }
        public void Write(Int32 i)
        {
            writer.Write(i);
        }

        public UInt32 ReadUInt32()
        {
            return reader.ReadUInt32();
        }
        public void Read(out UInt32 i)
        {
            i = reader.ReadUInt32();
        }
        public void Write(UInt32 i)
        {
            writer.Write(i);
        }

        public Int64 ReadInt64()
        {
            return reader.ReadInt64();
        }
        public void Read(out Int64 i)
        {
            i = reader.ReadInt64();
        }
        public void Write(Int64 i)
        {
            writer.Write(i);
        }        

        public UInt64 ReadUInt64()
        {
            return reader.ReadUInt64();
        }
        public void Read(out UInt64 i)
        {
            i = reader.ReadUInt64();
        }
        public void Write(UInt64 i)
        {
            writer.Write(i);
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }
        public void Read(out float f)
        {
            f = reader.ReadSingle();
        }
        public void Write(float f)
        {
            writer.Write(f);
        }

        public Double ReadDouble()
        {
            return reader.ReadDouble();
        }
        public void Read(out Double d)
        {
            d = reader.ReadDouble();
        }
        public void Write(Double d)
        {
            writer.Write(d);
        }

        public Decimal ReadDecimal()
        {
            return reader.ReadDecimal();
        }
        public void Read(out Decimal d)
        {
            d = reader.ReadDecimal();
        }
        public void Write(Decimal d)
        {
            writer.Write(d);
        }

        public T ReadData<T>()
            where T : IDynamicData, new()
        {
            T data = new T();
            data.Deserialize(this);
            return data;
        }
        public void Read<T>(T data)
            where T : IDynamicData
        {
            data.Deserialize(this);
        }
        public void Write<T>(T data)
            where T : IDynamicData
        {
            data.Serialize(this);
        }

        public T[] ReadArray<T>()
            where T : IDynamicData, new()
        {
            UInt16 count = reader.ReadUInt16();
            return ReadArray<T>(count);
        }
        public T[] ReadArray<T>(Int32 count)
            where T : IDynamicData, new()
        {
            T[] array = new T[count];
            for (Int32 i = 0; i < count; ++i)
            {
                array[i] = new T();
                array[i].Deserialize(this);
            }
            return array;
        }
        public void Read<T>(out T[] array)
            where T : IDynamicData, new()
        {
            array = ReadArray<T>();
        }
        public void Read<T>(out T[] array, Int32 count)
            where T : IDynamicData, new()
        {
            array = ReadArray<T>(count);
        }
        public void Write<T>(T[] array)
            where T : IDynamicData, new()
        {
            writer.Write((UInt16)array.Length);
            Write<T>(array, array.Length);
        }
        public void Write<T>(T[] array, Int32 count)
            where T : IDynamicData, new()
        {
            for (Int32 i = 0; i < count; ++i)
            {
                array[i].Serialize(this);
            }
        }

        public List<T> ReadList<T>()
            where T : IDynamicData, new()
        {
            List<T> list = new List<T>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                T data = new T();
                data.Deserialize(this);
                list.Add(data);
            }

            return list;
        }
        public void Read<T>(List<T> list)
            where T : IDynamicData, new()
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                T data = new T();
                data.Deserialize(this);
                list.Add(data);
            }
        }
        public void Read<T>(out List<T> list)
            where T : IDynamicData, new()
        {
            list = ReadList<T>();
        }
        public void Write<T>(List<T> list)
            where T : IDynamicData, new()
        {
            writer.Write((UInt16)list.Count);
            foreach (T data in list)
            {
                data.Serialize(this);
            }
        }

        public List<byte> ReadListByte()
        {
            List<byte> list = new List<byte>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadByte());
            }

            return list;
        }
        public void Read(List<byte> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadByte());
            }
        }
        public void Read(out List<byte> list)
        {
            list = ReadListByte();
        }
        public void Write(List<byte> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (byte i in list)
            {
                writer.Write(i);
            }
        }

        public List<Int16> ReadListInt16()
        {
            List<Int16> list = new List<Int16>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadInt16());
            }

            return list;
        }
        public void Read(List<Int16> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadInt16());
            }
        }
        public void Read(out List<Int16> list)
        {
            list = ReadListInt16();
        }
        public void Write(List<Int16> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (Int16 i in list)
            {
                writer.Write(i);
            }
        }

        public List<UInt16> ReadListUInt16()
        {
            List<UInt16> list = new List<UInt16>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadUInt16());
            }

            return list;
        }
        public void Read(List<UInt16> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadUInt16());
            }
        }
        public void Read(out List<UInt16> list)
        {
            list = ReadListUInt16();
        }
        public void Write(List<UInt16> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (UInt16 i in list)
            {
                writer.Write(i);
            }
        }

        public List<Int32> ReadListInt32()
        {
            List<Int32> list = new List<Int32>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadInt32());
            }

            return list;
        }
        public void Read(List<Int32> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadInt32());
            }
        }
        public void Read(out List<Int32> list)
        {
            list = ReadListInt32();
        }
        public void Write(List<Int32> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (Int32 i in list)
            {
                writer.Write(i);
            }
        }

        public List<UInt32> ReadListUInt32()
        {
            List<UInt32> list = new List<UInt32>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadUInt32());
            }

            return list;
        }
        public void Read(List<UInt32> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadUInt32());
            }
        }
        public void Read(out List<UInt32> list)
        {
            list = ReadListUInt32();
        }
        public void Write(List<UInt32> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (UInt32 i in list)
            {
                writer.Write(i);
            }
        }

        public List<UInt64> ReadListUInt64()
        {
            List<UInt64> list = new List<UInt64>();
            Read(list);
            return list;
        }
        public void Read(List<UInt64> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadUInt64());
            }
        }
        public void Read(out List<UInt64> list)
        {
            list = ReadListUInt64();
        }
        public void Write(List<UInt64> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (UInt64 i in list)
            {
                writer.Write(i);
            }
        }

        public List<float> ReadListFloat()
        {
            List<float> list = new List<float>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadSingle());
            }

            return list;
        }
        public void Read(List<float> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(reader.ReadSingle());
            }
        }
        public void Read(out List<float> list)
        {
            list = ReadListFloat();
        }
        public void Write(List<float> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (float f in list)
            {
                writer.Write(f);
            }
        }

        public List<String> ReadListString()
        {
            List<String> list = new List<String>();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(ReadString());
            }

            return list;
        }
        public void Read(List<String> list)
        {
            list.Clear();
            UInt16 count = reader.ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                list.Add(ReadString());
            }
        }
        public void Read(out List<String> list)
        {
            list = ReadListString();
        }
        public void Write(List<String> list)
        {
            writer.Write((UInt16)list.Count);
            foreach (String s in list)
            {
                Write(s);
            }
        }

        public Dictionary<TK, TV> ReadDiction<TK, TV>()
            where TK : IDynamicData, new()
            where TV : IDynamicData, new()
        {
            Dictionary<TK, TV> dic = new Dictionary<TK, TV>();
            Read<TK, TV>(dic);
            return dic;
        }
        public void Read<TK, TV>(out Dictionary<TK, TV> diction)
            where TK : IDynamicData, new()
            where TV : IDynamicData, new()
        {
            diction = ReadDiction<TK, TV>();
        }
        public void Read<TK, TV>(Dictionary<TK, TV> diction)
            where TK : IDynamicData, new()
            where TV : IDynamicData, new()
        {
            diction.Clear();
            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                TK key = new TK();
                key.Deserialize(this);
                TV value = new TV();
                value.Deserialize(this);

                diction.Add(key, value);
            }
        }
        public void Write<TK, TV>(Dictionary<TK, TV> diction)
            where TK : IDynamicData, new()
            where TV : IDynamicData, new()
        {
            writer.Write((UInt16)diction.Count);
            foreach (KeyValuePair<TK, TV> pair in diction)
            {
                pair.Key.Serialize(this);
                pair.Value.Serialize(this);
            }
        }

        public Dictionary<Int32, T> ReadDictionInt32<T>()
            where T : IDynamicData, new()
        {
            Dictionary<Int32, T> dic = new Dictionary<Int32, T>();
            Read<T>(dic);
            return dic;
        }
        public void Read<T>(out Dictionary<Int32, T> diction)
            where T : IDynamicData, new()
        {
            diction = ReadDictionInt32<T>();
        }
        public void Read<T>(Dictionary<Int32, T> diction)
            where T : IDynamicData, new()
        {
            diction.Clear();
            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                Int32 key = reader.ReadInt32();
                T value = new T();
                value.Deserialize(this);

                diction.Add(key, value);
            }
        }
        public void Write<T>(Dictionary<Int32, T> diction)
            where T : IDynamicData, new()
        {
            writer.Write((UInt16)diction.Count);
            foreach (KeyValuePair<Int32, T> pair in diction)
            {
                writer.Write(pair.Key);
                pair.Value.Serialize(this);
            }
        }

        public Dictionary<Int32, Int32> ReadDictionInt32Int32()
        {
            Dictionary<Int32, Int32> diction = new Dictionary<Int32, Int32>();

            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                Int32 key = reader.ReadInt32();
                Int32 value = reader.ReadInt32();
                diction.Add(key, value);
            }

            return diction;
        }
        public void Read(out Dictionary<Int32, Int32> diction)
        {
            diction = ReadDictionInt32Int32();
        }
        public void Read(Dictionary<Int32, Int32> diction)
        {
            diction.Clear();

            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                Int32 key = reader.ReadInt32();
                Int32 value = reader.ReadInt32();
                diction.Add(key, value);
            }
        }
        public void Write(Dictionary<Int32, Int32> diction)
        {
            writer.Write((UInt16)diction.Count);
            foreach (KeyValuePair<Int32, Int32> pair in diction)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        public Dictionary<String, String> ReadDictionStringString()
        {
            Dictionary<String, String> diction = new Dictionary<String, String>();

            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                String key = ReadString();
                String value = ReadString();
                diction.Add(key, value);
            }

            return diction;
        }
        public void Read(out Dictionary<String, String> diction)
        {
            diction = ReadDictionStringString();
        }
        public void Read(Dictionary<String, String> diction)
        {
            UInt16 count = ReadUInt16();
            for (int i = 0; i < count; ++i)
            {
                String key = ReadString();
                String value = ReadString();
                diction.Add(key, value);
            }
        }
        public void Write(Dictionary<String, String> diction)
        {
            writer.Write((UInt16)diction.Count);
            foreach (KeyValuePair<String, String> pair in diction)
            {
                Write(pair.Key);
                Write(pair.Value);
            }
        }

        protected MemoryStream memStream;
        protected BinaryReader reader;
        protected BinaryWriter writer;
    }
}
