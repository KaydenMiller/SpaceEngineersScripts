using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    
    public class IniSerializer
    {
        public static string Serialize<TValue>(TValue value, IniSerializerOptions options = null) where TValue: Serializable
        {
            var ini = new MyIni();
            
            value.SaveToFields();
            
            foreach (var field in value.Fields)
            {
                var section = field.Key.Split(':');
                ini.Set(section[0], section[1], field.Value.GetString());
            }

            return ini.ToString();
        }

        public static TOutput Deserialize<TOutput>(string value, IniSerializerOptions options = null)
            where TOutput : Serializable, new()
        {
            var ini = new MyIni();
            var success = ini.TryParse(value);

            if (!success)
            {
                throw new Exception("INI Parse Exception");
            }
            
            var output = new TOutput();
            output.LoadFields(ini);
            return output;
        }
    }

    public class IniSerializerOptions
    {
    }

    public class Field
    {
        private string Value { get; set; }
        
        public Field(string value)
        {
            Value = value;
        }
        public Field(int value)
        {
            Value = value.ToString();
        }
        public Field(float value)
        {
            Value = value.ToString("F4");
        }
        public Field(bool value)
        {
            Value = value.ToString();
        }
        public Field(IEnumerable<string> values)
        {
            Value = string.Join(",", values);
        }

        public int GetInt()
        {
            return int.Parse(Value);
        }
        public float GetFloat()
        {
            return float.Parse(Value);
        }
        public string GetString()
        {
            return Value;
        }
        public bool GetBool()
        {
            return bool.Parse(Value);
        }
        public string[] GetStringArray()
        {
            return Value.Split(',');
        }
    }

    public abstract class Serializable
    {
        public string Section { get; }
        protected Serializable(string section)
        {
            Section = section;
        }
        
        public readonly Dictionary<string, Field> Fields = new Dictionary<string, Field>();
        public abstract void SaveToFields();
        public abstract void LoadFields(MyIni ini);
    }
}