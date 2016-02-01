using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
public class Variant
{
    // Fields
    protected static LinkedList<Variant> m_pool = new LinkedList<Variant>();
    protected object m_val;

    // Methods
    public Variant()
    {
        this.m_val = null;
    }

    public Variant(ByteArray val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(bool val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(byte val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(double val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(short val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(int val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(long val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(sbyte val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(float val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(string val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(ushort val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(uint val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(ulong val)
    {
        this.m_val = null;
        this.m_val = val;
    }

    public Variant(byte[] val, int len)
    {
        this.m_val = null;
        this.m_val = new ByteArray(val, len);
    }

    public string _dump(int offset, string key)
    {
        int num;
        object obj2;
        string str = "";
        string str2 = "";
        for (num = 0; num < offset; num++)
        {
            str2 = str2 + " ";
        }
        if (this.isStr)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            obj2 = str;
            return (string.Concat(new object[] { obj2, '"', this._str.Replace("\0", ""), '"' }) + "\n");
        }
        if (this.isNumber && !this.isIntkeyDct)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            if (this.isDouble || this.isFloat)
            {
                str = str + this._double;
            }
            else
            {
                str = str + this._int64;
            }
            return (str + "\n");
        }
        if (this.isBool)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            return (str + (this._bool ? "true" : "false") + "\n");
        }
        if (this.isByteArray)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            str = str + "<ByteArray:";
            for (num = 0; num < (this.m_val as ByteArray).length; num++)
            {
                if (num != 0)
                {
                    str = str + ",";
                }
                str = str + (this.m_val as ByteArray)[num];
            }
            return (str + ">\n");
        }
        if (this.isDct)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            obj2 = str;
            str = string.Concat(new object[] { obj2, "<Dct:", this.Count, ">\n" });
            foreach (string str3 in this.Keys)
            {
                if (this[str3] != null)
                {
                    str = str + this[str3]._dump(offset + 4, str3);
                }
            }
            return str;
        }
        if (this.isArr)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            obj2 = str;
            str = string.Concat(new object[] { obj2, "<Array:", this.Count, ">\n" });
            for (num = 0; num < this.Length; num++)
            {
                str = str + this[num]._dump(offset + 4, num.ToString());
            }
            return str;
        }
        if (this.isIntkeyDct)
        {
            str = str + str2;
            if (key != "")
            {
                str = str + "[" + key + "]:";
            }
            obj2 = str;
            str = string.Concat(new object[] { obj2, "<IntDct:", this.Count, ">\n" });
            foreach (int num2 in this._intDct.Keys)
            {
                str = str + this[num2]._dump(offset + 4, num2.ToString());
            }
        }
        return str;
    }

    public static Variant alloc()
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            return variant;
        }
        return new Variant();
    }

    public static Variant alloc(ByteArray val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(bool val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(byte val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(double val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(short val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(int val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(long val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(sbyte val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(float val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(string val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(ushort val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(uint val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(ulong val)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = val;
            return variant;
        }
        return new Variant(val);
    }

    public static Variant alloc(byte[] val, int len)
    {
        if (m_pool.Count > 0)
        {
            Variant variant = m_pool.Last.Value;
            m_pool.RemoveLast();
            variant.m_val = new ByteArray(val, len);
            return variant;
        }
        return new Variant(val, len);
    }

    public Variant clone()
    {
        Variant variant = null;
        if (this.isDct)
        {
            variant = new Variant();
            variant.setToDct();
            Dictionary<string, Variant> val = this.m_val as Dictionary<string, Variant>;
            foreach (string str in val.Keys)
            {
                if (val[str] != null)
                {
                    variant[str] = val[str].clone();
                }
                else
                {
                    variant[str] = null;
                }
            }
            return variant;
        }
        if (this.isIntkeyDct)
        {
            variant = new Variant();
            variant.setToIntkeyDct();
            Dictionary<int, Variant> dictionary2 = this.m_val as Dictionary<int, Variant>;
            foreach (int num in dictionary2.Keys)
            {
                if (dictionary2[num] != null)
                {
                    variant[num] = dictionary2[num].clone();
                }
                else
                {
                    variant[num] = null;
                }
            }
            return variant;
        }
        if (this.isArr)
        {
            variant = new Variant();
            variant.setToArray();
            List<Variant> list = this.m_val as List<Variant>;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    variant.pushBack(list[i].clone());
                }
                else
                {
                    variant.pushBack(null);
                }
            }
            return variant;
        }
        if (this.isByteArray)
        {
            return new Variant(this.m_val as ByteArray);
        }
        return new Variant { m_val = this.m_val };
    }

    public bool ContainsKey(int key)
    {
        if (!((this.m_val != null) && (this.m_val is Dictionary<int, Variant>)))
        {
            return false;
        }
        return (this.m_val as Dictionary<int, Variant>).ContainsKey(key);
    }

    public bool ContainsKey(string key)
    {
        if (!((this.m_val != null) && (this.m_val is Dictionary<string, Variant>)))
        {
            return false;
        }
        return (this.m_val as Dictionary<string, Variant>).ContainsKey(key);
    }

    public Variant convertToDct(string key)
    {
        if (this.isDct || this.isIntkeyDct)
        {
            return this.clone();
        }
        if (this.isArr)
        {
            Variant variant = new Variant();
            variant.setToDct();
            List<Variant> val = this.m_val as List<Variant>;
            for (int i = 0; i < val.Count; i++)
            {
                Variant variant2 = val[i];
                if (variant2 != null)
                {
                    variant[variant2[key]._str] = variant2.clone();
                }
            }
            return variant;
        }
        return null;
    }

    public bool deepEqual(Variant v1)
    {
        if (v1 != null)
        {
            if (this == v1)
            {
                return true;
            }
            Variant variant = this;
            if (variant.isDct && v1.isDct)
            {
                Dictionary<string, Variant> val = variant.m_val as Dictionary<string, Variant>;
                Dictionary<string, Variant> dictionary2 = v1.m_val as Dictionary<string, Variant>;
                if (val.Count != dictionary2.Count)
                {
                    return false;
                }
                foreach (string str in val.Keys)
                {
                    if (!val[str].deepEqual(dictionary2[str]))
                    {
                        return false;
                    }
                }
                goto Label_028E;
            }
            if (variant.isIntkeyDct && v1.isIntkeyDct)
            {
                Dictionary<int, Variant> dictionary3 = variant.m_val as Dictionary<int, Variant>;
                Dictionary<int, Variant> dictionary4 = v1.m_val as Dictionary<int, Variant>;
                if (dictionary3.Count != dictionary4.Count)
                {
                    return false;
                }
                foreach (int num in dictionary3.Keys)
                {
                    if (dictionary3[num].deepEqual(dictionary4[num]))
                    {
                        return false;
                    }
                }
                goto Label_028E;
            }
            if (variant.isArr && v1.isArr)
            {
                List<Variant> list = variant.m_val as List<Variant>;
                List<Variant> list2 = v1.m_val as List<Variant>;
                if (list.Count != list2.Count)
                {
                    return false;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].deepEqual(list2[i]))
                    {
                        return false;
                    }
                }
                goto Label_028E;
            }
            if ((variant.m_val == null) && (v1.m_val == null))
            {
                return true;
            }
            if ((variant.m_val != null) && (v1.m_val != null))
            {
                return variant.m_val.Equals(v1.m_val);
            }
        }
        return false;
    Label_028E:
        return true;
    }

    public string dump()
    {
        return this._dump(0, "");
    }

    public static void free(Variant v)
    {
        if (v.m_val is Dictionary<string, Variant>)
        {
            foreach (Variant variant in (v.m_val as Dictionary<string, Variant>).Values)
            {
                free(variant);
            }
        }
        else if (v.m_val is Dictionary<int, Variant>)
        {
            foreach (Variant variant in (v.m_val as Dictionary<int, Variant>).Values)
            {
                free(variant);
            }
        }
        else if (v.m_val is List<Variant>)
        {
            foreach (Variant variant in v.m_val as List<Variant>)
            {
                free(variant);
            }
        }
        v.m_val = null;
        m_pool.AddLast(v);
    }

    public Variant mergeFrom(Variant src)
    {
        if (src != null)
        {
            if (src.isDct)
            {
                if (!this.isDct)
                {
                    this.setToDct();
                }
                Dictionary<string, Variant> val = src.m_val as Dictionary<string, Variant>;
                foreach (string str in val.Keys)
                {
                    if (this.ContainsKey(str) && (this[str] != null))
                    {
                        if (val[str].m_val != null)
                        {
                            this[str].mergeFrom(val[str]);
                        }
                    }
                    else if (src[str] != null)
                    {
                        this[str] = val[str].clone();
                    }
                }
            }
            else if (src.isIntkeyDct)
            {
                if (!this.isIntkeyDct)
                {
                    this.setToIntkeyDct();
                }
                Dictionary<int, Variant> dictionary2 = src.m_val as Dictionary<int, Variant>;
                foreach (int num in dictionary2.Keys)
                {
                    if (this.ContainsKey(num) && (this[num] != null))
                    {
                        this[num].mergeFrom(dictionary2[num]);
                    }
                    else
                    {
                        this[num] = dictionary2[num].clone();
                    }
                }
            }
            else if (src.isArr)
            {
                if (src.Count != 0)
                {
                    if (!this.isArr)
                    {
                        this.setToArray();
                    }
                    this.pushBack(src[0].clone());
                }
            }
            else
            {
                this.m_val = src.m_val;
            }
        }
        return this;
    }

    public static implicit operator Variant(ByteArray val)
    {
        return new Variant(val);
    }

    public static implicit operator ByteArray(Variant val)
    {
        return val._byteAry;
    }

    public static implicit operator bool(Variant val)
    {
        return val._bool;
    }

    public static implicit operator byte(Variant val)
    {
        return val._byte;
    }

    public static implicit operator double(Variant val)
    {
        return val._double;
    }

    public static implicit operator short(Variant val)
    {
        return val._int16;
    }

    public static implicit operator int(Variant val)
    {
        return val._int32;
    }

    public static implicit operator long(Variant val)
    {
        return val._int64;
    }

    public static implicit operator sbyte(Variant val)
    {
        return val._sbyte;
    }

    public static implicit operator float(Variant val)
    {
        return val._float;
    }

    public static implicit operator string(Variant val)
    {
        return val._str;
    }

    public static implicit operator ushort(Variant val)
    {
        return val._uint16;
    }

    public static implicit operator uint(Variant val)
    {
        return val._uint32;
    }

    public static implicit operator ulong(Variant val)
    {
        return val._uint64;
    }

    public static implicit operator Variant(bool val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(byte val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(double val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(short val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(int val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(long val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(sbyte val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(float val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(string val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(ushort val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(uint val)
    {
        return new Variant(val);
    }

    public static implicit operator Variant(ulong val)
    {
        return new Variant(val);
    }
        

    public void pushBack(Variant v)
    {
        if (this.m_val == null)
        {
            this.m_val = new List<Variant>();
        }
        if (this.m_val is List<Variant>)
        {
            (this.m_val as List<Variant>).Add(v);
        }
    }

    public void RemoveKey(string key)
    {
        if ((this.m_val != null) && (this.m_val is Dictionary<string, Variant>))
        {
            (this.m_val as Dictionary<string, Variant>).Remove(key);
        }
    }

    public void setToArray()
    {
        this.m_val = new List<Variant>();
    }

    public void setToByteArray()
    {
        this.setToByteArray(0);
    }

    public void setToByteArray(int cap)
    {
        this.m_val = new ByteArray(cap);
    }

    public void setToDct()
    {
        this.m_val = new Dictionary<string, Variant>();
    }

    public void setToIntkeyDct()
    {
        this.m_val = new Dictionary<int, Variant>();
    }

    // Properties
    public List<Variant> _arr
    {
        get
        {
            if (this.m_val == null)
            {
                this.m_val = new List<Variant>();
            }
            if (this.m_val is List<Variant>)
            {
                return (this.m_val as List<Variant>);
            }
            return null;
        }
    }

    public bool _bool
    {
        get
        {
            if (this.m_val is bool)
            {
                return (bool) this.m_val;
            }
            if (this.m_val is string)
            {
                return ((this.m_val as string).ToLower() == "true");
            }
            return Convert.ToBoolean(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public byte _byte
    {
        get
        {
            if (this.m_val is byte)
            {
                return (byte) this.m_val;
            }
            if (this.m_val is string)
            {
                return byte.Parse(this.m_val as string);
            }
            return Convert.ToByte(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public ByteArray _byteAry
    {
        get
        {
            if (this.m_val is ByteArray)
            {
                return (this.m_val as ByteArray);
            }
            return null;
        }
    }

    public Dictionary<string, Variant> _dct
    {
        get
        {
            if (this.m_val == null)
            {
                this.m_val = new Dictionary<string, Variant>();
            }
            if (this.m_val is Dictionary<string, Variant>)
            {
                return (this.m_val as Dictionary<string, Variant>);
            }
            return null;
        }
    }

    public double _double
    {
        get
        {
            if (this.m_val is double)
            {
                return (double) this.m_val;
            }
            if (this.m_val is string)
            {
                return (double) ((float) double.Parse(this.m_val as string));
            }
            return Convert.ToDouble(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public float _float
    {
        get
        {
            if (this.m_val is float)
            {
                return (float) this.m_val;
            }
            if (this.m_val is string)
            {
                if (((this.m_val as string) == "NaN") || ((this.m_val as string) == "NAN"))
                {
                    return float.NaN;
                }
                return (float) double.Parse(this.m_val as string);
            }
            return Convert.ToSingle(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public int _int
    {
        get
        {
            return this._int32;
        }
        set
        {
            this.m_val = value;
        }
    }

    public short _int16
    {
        get
        {
            if (this.m_val is short)
            {
                return (short) this.m_val;
            }
            if (this.m_val is string)
            {
                return short.Parse(this.m_val as string);
            }
            return Convert.ToInt16(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public int _int32
    {
        get
        {
            if (this.m_val is int)
            {
                return (int) this.m_val;
            }
            if (this.m_val is string)
            {
                return int.Parse(this.m_val as string);
            }
            return Convert.ToInt32(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public long _int64
    {
        get
        {
            if (this.m_val is long)
            {
                return (long) this.m_val;
            }
            if (this.m_val is string)
            {
                return long.Parse(this.m_val as string);
            }
            return Convert.ToInt64(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public Dictionary<int, Variant> _intDct
    {
        get
        {
            if (this.m_val == null)
            {
                this.m_val = new Dictionary<int, Variant>();
            }
            if (this.m_val is Dictionary<int, Variant>)
            {
                return (this.m_val as Dictionary<int, Variant>);
            }
            return null;
        }
    }

    public sbyte _sbyte
    {
        get
        {
            if (this.m_val is sbyte)
            {
                return (sbyte) this.m_val;
            }
            if (this.m_val is string)
            {
                return sbyte.Parse(this.m_val as string);
            }
            return Convert.ToSByte(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public string _str
    {
        get
        {
            if (this.m_val != null)
            {
                if (this.m_val is string)
                {
                    return (this.m_val as string);
                }
                if ((this.m_val is int) || (this.m_val is uint))
                {
                    return this.m_val.ToString();
                }
            }
            return "";
        }
        set
        {
            this.m_val = value;
        }
    }

    public uint _uint
    {
        get
        {
            return this._uint32;
        }
        set
        {
            this.m_val = value;
        }
    }

    public ushort _uint16
    {
        get
        {
            if (this.m_val is ushort)
            {
                return (ushort) this.m_val;
            }
            if (this.m_val is string)
            {
                return ushort.Parse(this.m_val as string);
            }
            return Convert.ToUInt16(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public uint _uint32
    {
        get
        {
            if (this.m_val is uint)
            {
                return (uint) this.m_val;
            }
            if (this.m_val is string)
            {
                return uint.Parse(this.m_val as string);
            }
            return Convert.ToUInt32(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public ulong _uint64
    {
        get
        {
            if (this.m_val is ulong)
            {
                return (ulong) this.m_val;
            }
            if (this.m_val is string)
            {
                return ulong.Parse(this.m_val as string);
            }
            return Convert.ToUInt64(this.m_val);
        }
        set
        {
            this.m_val = value;
        }
    }

    public object _val
    {
        get
        {
            return this.m_val;
        }
        set
        {
            this.m_val = value;
        }
    }

    public int Count
    {
        get
        {
            if (this.m_val != null)
            {
                if (this.m_val is List<Variant>)
                {
                    return (this.m_val as List<Variant>).Count;
                }
                if (this.m_val is string)
                {
                    return (this.m_val as string).Length;
                }
                if (this.m_val is Dictionary<string, Variant>)
                {
                    return (this.m_val as Dictionary<string, Variant>).Keys.Count;
                }
                if (this.m_val is Dictionary<int, Variant>)
                {
                    return (this.m_val as Dictionary<int, Variant>).Keys.Count;
                }
                if (this.m_val is ByteArray)
                {
                    return (this.m_val as ByteArray).length;
                }
            }
            return 0;
        }
    }

    public Dictionary<int, Variant>.KeyCollection IntKeys
    {
        get
        {
            if (!((this.m_val != null) && (this.m_val is Dictionary<int, Variant>)))
            {
                return null;
            }
            return (this.m_val as Dictionary<int, Variant>).Keys;
        }
    }

    public Dictionary<int, Variant>.ValueCollection IntKeyValues
    {
        get
        {
            if (!((this.m_val != null) && (this.m_val is Dictionary<int, Variant>)))
            {
                return null;
            }
            return (this.m_val as Dictionary<int, Variant>).Values;
        }
    }

    public bool isArr
    {
        get
        {
            return (this.m_val is List<Variant>);
        }
    }

    public bool isBool
    {
        get
        {
            return (this.m_val is bool);
        }
    }

    public bool isByte
    {
        get
        {
            return (this.m_val is byte);
        }
    }

    public bool isByteArray
    {
        get
        {
            return (this.m_val is ByteArray);
        }
    }

    public bool isDct
    {
        get
        {
            return (this.m_val is Dictionary<string, Variant>);
        }
    }

    public bool isDouble
    {
        get
        {
            return (this.m_val is double);
        }
    }

    public bool isFloat
    {
        get
        {
            return (this.m_val is float);
        }
    }

    public bool isInt16
    {
        get
        {
            return (this.m_val is short);
        }
    }

    public bool isInt32
    {
        get
        {
            return (this.m_val is int);
        }
    }

    public bool isInt64
    {
        get
        {
            return (this.m_val is long);
        }
    }

    public bool isInteger
    {
        get
        {
            return (((((this.m_val is byte) || (this.m_val is ushort)) || ((this.m_val is uint) || (this.m_val is ulong))) || (((this.m_val is sbyte) || (this.m_val is short)) || (this.m_val is int))) || (this.m_val is long));
        }
    }

    public bool isIntkeyDct
    {
        get
        {
            return (this.m_val is Dictionary<int, Variant>);
        }
    }

    public bool isNumber
    {
        get
        {
            return (((!(this.m_val is Dictionary<string, Variant>) && !(this.m_val is List<Variant>)) && (!(this.m_val is string) && !(this.m_val is bool))) && !(this.m_val is ByteArray));
        }
    }

    public bool isSByte
    {
        get
        {
            return (this.m_val is sbyte);
        }
    }

    public bool isSignedInteger
    {
        get
        {
            return ((((this.m_val is sbyte) || (this.m_val is short)) || (this.m_val is int)) || (this.m_val is long));
        }
    }

    public bool isStr
    {
        get
        {
            return (this.m_val is string);
        }
    }

    public bool isUint16
    {
        get
        {
            return (this.m_val is ushort);
        }
    }

    public bool isUint32
    {
        get
        {
            return (this.m_val is uint);
        }
    }

    public bool isUint64
    {
        get
        {
            return (this.m_val is ulong);
        }
    }

    public Variant this[Variant key]
    {
        get
        {
            if (!key.isStr)
            {
                return null;
            }
            return this[key._str];
        }
        set
        {
            if (key.isStr)
            {
                this[key._str] = value;
            }
        }
    }

    public Variant this[string key]
    {
        get
        {
            if ((this.m_val != null) && (this.m_val is Dictionary<string, Variant>))
            {
                Variant variant = null;
                (this.m_val as Dictionary<string, Variant>).TryGetValue(key, out variant);
                return variant;
            }
            return null;
        }
        set
        {
            if (this.m_val == null)
            {
                this.m_val = new Dictionary<string, Variant>();
            }
            if (this.m_val is Dictionary<string, Variant>)
            {
                (this.m_val as Dictionary<string, Variant>)[key] = value;
            }
        }
    }

    public Variant this[int idx]
    {
        get
        {
            if (this.m_val != null)
            {
                if (this.m_val is List<Variant>)
                {
                    if ((idx >= 0) && (idx < (this.m_val as List<Variant>).Count))
                    {
                        return (this.m_val as List<Variant>)[idx];
                    }
                    return null;
                }
                if (this.m_val is Dictionary<int, Variant>)
                {
                    return (this.m_val as Dictionary<int, Variant>)[idx];
                }
            }
            return null;
        }
        set
        {
            if (this.m_val == null)
            {
                this.m_val = new List<Variant>();
            }
            if (this.m_val is List<Variant>)
            {
                List<Variant> val = this.m_val as List<Variant>;
                while (idx >= val.Count)
                {
                    val.Add(null);
                }
                val[idx] = value;
            }
            else if (this.m_val is Dictionary<int, Variant>)
            {
                (this.m_val as Dictionary<int, Variant>)[idx] = value;
            }
        }
    }

    public Dictionary<string, Variant>.KeyCollection Keys
    {
        get
        {
            if (!((this.m_val != null) && (this.m_val is Dictionary<string, Variant>)))
            {
                return null;
            }
            return (this.m_val as Dictionary<string, Variant>).Keys;
        }
    }

    public int Length
    {
        get
        {
            if (this.m_val is List<Variant>)
            {
                return (this.m_val as List<Variant>).Count;
            }
            if (this.m_val is string)
            {
                return (this.m_val as string).Length;
            }
            if (this.m_val is ByteArray)
            {
                return (this.m_val as ByteArray).length;
            }
            return 0;
        }
    }

    public Dictionary<string, Variant>.ValueCollection Values
    {
        get
        {
            if (!((this.m_val != null) && (this.m_val is Dictionary<string, Variant>)))
            {
                return null;
            }
            return (this.m_val as Dictionary<string, Variant>).Values;
        }
    }
}

}
