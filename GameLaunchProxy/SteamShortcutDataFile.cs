using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace GameLaunchProxy
{
    public static class SteamShortcutDataFile
    {
        public static VPropertyCollection Read(string steamShortcutFilePath)
        {
            using (FileStream stream = File.OpenRead(steamShortcutFilePath))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return ReadPropertyArray(reader);
            }
        }

        public static void Write(string steamShortcutFilePath, VPropertyCollection data)
        {
            using (FileStream stream = File.OpenWrite(steamShortcutFilePath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                WritePropertyArray(writer, data);
                writer.Write((byte)0x08);
            }
        }

        private static VProperty ReadProperty(BinaryReader reader)
        {
            byte token = reader.ReadByte();
            string Key = ReadString(reader);
            VToken Value;
            switch (token)
            {
                case 0x00:
                    Value = ReadPropertyArray(reader);
                    break;
                case 0x01:
                    Value = ReadString(reader);
                    break;
                case 0x02:
                    Value = reader.ReadInt32();
                    break;
                default:
                    throw new Exception(string.Format("Unknown Type Byte {0:X2}", token));
            }
            return new VProperty(Key, Value);
        }

        private static void WriteProperty(BinaryWriter writer, VProperty data)
        {
            if (data.Value.GetType() == typeof(VPropertyCollection))
            {
                writer.Write((byte)0x00);
                WriteString(writer, data.Key);
                WritePropertyArray(writer, (VPropertyCollection)data.Value);
                writer.Write((byte)0x08);
            }
            else if (data.Value.GetType() == typeof(VStringToken))
            {
                writer.Write((byte)0x01);
                WriteString(writer, data.Key);
                WriteString(writer, ((VStringToken)data.Value).Value);
            }
            else if (data.Value.GetType() == typeof(VIntToken))
            {
                writer.Write((byte)0x02);
                WriteString(writer, data.Key);
                writer.Write(((VIntToken)data.Value).Value);
            }
            else
            {
                throw new Exception(string.Format("Unknown Type {0}", data.GetType().ToString()));
            }
        }

        private static VPropertyCollection ReadPropertyArray(BinaryReader reader)
        {
            VPropertyCollection Values = new VPropertyCollection();
            while (reader.PeekChar() != 0x08)
            {
                Values.Add(ReadProperty(reader));
            }
            reader.ReadByte();
            return Values;
        }

        private static void WritePropertyArray(BinaryWriter writer, VPropertyCollection data)
        {
            data.Properties.ForEach(dr =>
            {
                WriteProperty(writer, dr);
            });
        }

        private static string ReadString(BinaryReader reader)
        {
            List<byte> values = new List<byte>();
            byte tmp;
            while ((tmp = reader.ReadByte()) != (byte)0x00)
            {
                values.Add(tmp);
            }

            return System.Text.Encoding.UTF8.GetString(values.ToArray());
        }

        private static void WriteString(BinaryWriter writer, string data)
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes(data));
            writer.Write((byte)0x00);
        }
    }

    public abstract class VToken
    {
        static public implicit operator VToken(int value)
        {
            return new VIntToken(value);
        }
        static public implicit operator VToken(string value)
        {
            return new VStringToken(value);
        }
    }
    public class VProperty
    {
        public string Key { get; set; }
        public VToken Value { get; set; }

        public VProperty(string key, VToken value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public class VIntToken : VToken
    {
        public int Value { get; private set; }

        public VIntToken(int value)
        {
            this.Value = value;
        }
    }
    public class VStringToken : VToken
    {
        public string Value { get; private set; }

        public VStringToken(string value)
        {
            this.Value = value;
        }
    }
    public class VPropertyCollection : VToken
    {
        public List<VProperty> Properties { get; private set; }

        public VPropertyCollection()
        {
            Properties = new List<VProperty>();
        }

        public void Add(VProperty vProperty)
        {
            Properties.Add(vProperty);
        }

        public void Add(string key, VToken token)
        {
            Properties.Add(new VProperty(key, token));
        }

        public void Add(VToken token)
        {
            if (!IsNumeric())
                throw new Exception("Collection is not array");

            Properties.Add(new VProperty(Properties.Count > 0 ? (Properties.Max(dr => int.Parse(dr.Key)) + 1).ToString() : "0", token));
        }

        public int Remove(string key)
        {
            if(IsNumeric())
            {
                int countRemoved = Properties.RemoveAll(dr => dr.Key == key);
                int keyNumber;
                if(int.TryParse(key, out keyNumber))
                {
                    Properties.Where(dr => int.Parse(dr.Key) > keyNumber).ToList().ForEach(dr =>
                    {
                        dr.Key = (int.Parse(dr.Key) - 1).ToString();
                    });
                }
                return countRemoved;
            }
            else
            {
                return Properties.RemoveAll(dr => dr.Key == key);
            }
        }

        public int Remove(VToken value)
        {
            if (IsNumeric())
            {
                var qry = Properties.Where(dr => dr.Value == value);
                if (qry.Count() > 0)
                {
                    int keyNumber = int.Parse(qry.First().Key);
                    int countRemoved = Properties.RemoveAll(dr => dr.Value == value);
                    Properties.Where(dr => int.Parse(dr.Key) > keyNumber).ToList().ForEach(dr =>
                    {
                        dr.Key = (int.Parse(dr.Key) - 1).ToString();
                    });
                    return countRemoved;
                }
                return 0;
            }
            else
            {
                return Properties.RemoveAll(dr => dr.Value == value);
            }
        }

        public bool IsNumeric()
        {
            if (Properties.Count == 0)
                return true;

            int tmp;
            if (!Properties.All(dr => int.TryParse(dr.Key, out tmp)))
                return false;

            var baseQry = Properties.Select(dr => dr.Key);
            if (!baseQry.SequenceEqual(baseQry.Distinct()))
                return false;

            int counter = 0;
            bool orderPreserved = true;
            baseQry.Select(dr => int.Parse(dr)).OrderBy(dr => dr).ToList().ForEach(dr =>
            {
                orderPreserved = orderPreserved && (dr == counter);
                counter++;
            });
            return orderPreserved;
        }

        public VToken this[string key]
        {
            get
            {
                return Properties.Where(dr => dr.Key == key).First().Value;
            }
            set
            {
                var qry = Properties.Where(dr => dr.Key == key);
                if(qry.Count() >0)
                {
                    qry.First().Value = value;
                }
                else
                {
                    Properties.Add(new VProperty(key, value));
                }
            }
        }
    }
}