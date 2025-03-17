using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using System.CodeDom.Compiler;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace VtolVrRankedMissionSetup.VT
{
    public static class VTSerializer
    {
        public static T DeserializeFromFile<T>(string path) => (T)DeserializeFromFile(path);

        public static void SerializeToFile(object obj, string path)
        {
            using FileStream fileStream = new(path, FileMode.Create);
            using StreamWriter writer = new(fileStream);

            writer.NewLine = "\n";

            SerializeObject(obj, writer);
        }

        private static object DeserializeFromFile(string path)
        {
            FileInfo fileInfo = new(path);
            string extension = fileInfo.Extension.ToUpper();

            using FileStream fileStream = new(path, FileMode.Open);
            using StreamReader reader = new(fileStream);

            string? typeLine = reader.ReadLine();

            Type? baseType = ParseType(extension, typeLine!);

            if (baseType == null)
                throw new InvalidOperationException($"Base type of file \"{path}\" is not defined");

            return ParseObject(extension, baseType, reader);
        }

        private static object ParseObject(string extension, Type type, StreamReader reader)
        {
            if (type.IsArray)
            {
                return ParseArray(extension, type, reader);
            }

            PropertyInfo[] properties = type.GetProperties();

            if (!reader.ReadLine()!.EndsWith('{'))
                throw new Exception("File is not valid. invalid line found.");

            ConstructorInfo? constructorInfo = type.GetConstructor([]);

            if (constructorInfo == null)
                throw new Exception($"default constructor for type {type.FullName} was not found");

            object obj = constructorInfo.Invoke([]);

            while (true)
            {
                string trimmedLine = reader.ReadLine()!.TrimStart();

                if (trimmedLine.EndsWith('}'))
                {
                    return obj;
                }

                if (char.IsLower(trimmedLine[0]))
                {
                    string[] parts = trimmedLine.Split(' ');
                    string typestring = char.ToUpper(parts[0][0]) + parts[0][1..];

                    PropertyInfo? property = properties.FirstOrDefault(p => p.Name == typestring);

                    if (property != null)
                    {
                        object value = ParseValue(property, string.Join(' ', parts[2..]));

                        property.SetValue(obj, value);
                    }

                    continue;
                }

                PropertyInfo? childProperty = properties.FirstOrDefault(p => p.Name == trimmedLine);

                if (childProperty != null)
                {
                    object value = ParseObject(extension, childProperty.PropertyType, reader);

                    childProperty.SetValue(obj, value);
                    continue;
                }

                ParseUnknown(reader);
            }
        }

        private static object ParseArray(string extension, Type type, StreamReader reader)
        {
            if (!reader.ReadLine()!.EndsWith('{'))
                throw new Exception("File is not valid. invalid line found.");

            List<object> objects = [];

            while (true)
            {
                string trimmedLine = reader.ReadLine()!.TrimStart();

                if (trimmedLine.EndsWith('}'))
                {
                    ConstructorInfo constructorInfo = type.GetConstructor([typeof(int)])!;
                    Array array = (Array)constructorInfo.Invoke([objects.Count]);

                    for (int i = 0; i < objects.Count; ++i)
                    {
                        array.SetValue(objects[i], i);
                    }

                    return array;
                }

                Type? valueType = ParseType(extension, trimmedLine);

                if (valueType != null)
                {
                    objects.Add(ParseObject(extension, valueType, reader));
                    continue;
                }

                ParseUnknown(reader);
            }
        }

        private static void ParseUnknown(StreamReader reader)
        {
            int count = 0;
            do
            {
                string line = reader.ReadLine()!;

                if (line.EndsWith('{'))
                    ++count;
                else if (line.EndsWith('}'))
                    --count;

            } while (count > 0);
        }

        private static Type? ParseType(string extension, string typeLine)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(typeLine);

            typeLine = typeLine.Trim();

            return typeof(VTSerializer).Assembly.GetType($"VtolVrRankedMissionSetup{extension}.{typeLine}");
        }

        private static object ParseValue(PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(double))
                return double.Parse(value);
            if (property.PropertyType == typeof(float))
                return float.Parse(value);

            if (property.PropertyType == typeof(int))
                return int.Parse(value);
            if (property.PropertyType == typeof(uint))
                return uint.Parse(value);

            if (property.PropertyType == typeof(bool))
                return value == "True";

            if (property.PropertyType == typeof(Vector3))
            {
                string[] parts = value.Trim('(', ')').Split(',');

                return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
            }
            if (property.PropertyType == typeof(Vector2))
            {
                string[] parts = value.Trim('(', ')').Split(',');

                return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
            }

            return value;
        }

        private static void SerializeObject(object obj, StreamWriter writer, int indents = 0, string? name = null, PropertyInfo? propertyInfo = null)
        {
            Type type = obj.GetType();

            if (type.IsArray || (propertyInfo?.GetCustomAttribute<VTInlineArrayAttribute>() != null))
            {
                if (type == typeof(Empty))
                {
                    // This object was null
                    return;
                }

                SerializeArray((Array)obj, writer, indents, name!, propertyInfo!);
                return;
            }

            string indentation = new('\t', indents);
            ++indents;

            if (string.IsNullOrWhiteSpace(name))
                writer.WriteLine($"{indentation}{type.Name}");
            else
                writer.WriteLine($"{indentation}{name}");

            writer.WriteLine($"{indentation}{{");

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                object? value = prop.GetValue(obj);

                VTIgnoreAttribute? vtIgnore = prop.GetCustomAttribute<VTIgnoreAttribute>();
                if (vtIgnore != null)
                {
                    if (ShouldIgnoreValue(prop, value, vtIgnore))
                        continue;
                }

                IdLinkAttribute? idLink = prop.GetCustomAttribute<IdLinkAttribute>();
                if (idLink != null)
                {
                    WriteIdLink(idLink, prop, value, writer, indents);
                    continue;
                }

                if (TryWriteSimple(prop, value, writer, indents))
                    continue;

                object child = value ?? new Empty();

                SerializeObject(child, writer, indents, GetName(prop, false), prop);
            }

            writer.WriteLine($"{indentation}}}");
        }

        private static void SerializeArray(Array array, StreamWriter writer, int indents, string name, PropertyInfo propertyInfo)
        {
            bool inline = propertyInfo.GetCustomAttribute<VTInlineArrayAttribute>() != null;

            string indentation = null!;
            if (!inline)
            {
                indentation = new('\t', indents);
                ++indents;

                writer.WriteLine($"{indentation}{name}");
                writer.WriteLine($"{indentation}{{");
            }

            foreach (object obj in array)
            {
                VTNameAttribute? typeName = obj.GetType().GetCustomAttribute<VTNameAttribute>(true);

                SerializeObject(obj, writer, indents, typeName?.Name);
            }

            if (!inline)
                writer.WriteLine($"{indentation}}}");
        }

        private static bool TryWriteSimple(PropertyInfo prop, object? value, StreamWriter writer, int indents)
        {
            if (prop.PropertyType != typeof(string) &&
                prop.PropertyType != typeof(bool) &&
                prop.PropertyType != typeof(double) &&
                prop.PropertyType != typeof(float) &&
                prop.PropertyType != typeof(int) &&
                prop.PropertyType != typeof(uint) &&
                prop.PropertyType != typeof(Vector3) &&
                prop.PropertyType != typeof(Vector2) &&
                prop.PropertyType != typeof(TimeSpan) &&
                !prop.PropertyType.IsEnum)
            {
                return false;
            }

            string propValue;

            if (prop.PropertyType == typeof(bool))
            {
                bool b = (bool)value!;
                propValue = b ? "True" : "False";
            }
            else if (prop.PropertyType == typeof(Vector3))
            {
                Vector3 v = (Vector3)value!;
                propValue = $"({v.X}, {v.Y}, {v.Z})";
            }
            else if (prop.PropertyType == typeof(Vector2))
            {
                Vector2 v = (Vector2)value!;
                propValue = $"({v.X}, {v.Y})";
            }
            else if (prop.PropertyType == typeof(TimeSpan))
            {
                TimeSpan ts = (TimeSpan)value!;
                propValue = ts.TotalSeconds.ToString();
            }
            else if (prop.PropertyType.IsEnum)
            {
                propValue = value?.ToString()?.Replace('_', ' ') ?? "null";
            }
            else
            {
                propValue = value?.ToString() ?? "null";
            }

            string indentation = new('\t', indents);

            writer.WriteLine($"{indentation}{GetName(prop)} = {propValue}");
            return true;
        }

        private static void WriteIdLink(IdLinkAttribute idLink, PropertyInfo prop, object? value, StreamWriter writer, int indents)
        {
            string indentation = new('\t', indents);

            Type? valueType = value?.GetType();
            object? id;
            if (valueType?.IsArray ?? false)
            {
                id = string.Empty;

                foreach (object obj in (Array)value!)
                {
                    PropertyInfo idProp = obj.GetType().GetProperties().Single(p => p.GetCustomAttribute<IdAttribute>() != null);
                    id += $"{idProp.GetValue(obj)};";
                }
            }
            else
            {
                PropertyInfo? idProp = valueType?.GetProperties().Single(p => p.GetCustomAttribute<IdAttribute>() != null);
                id = idProp?.GetValue(value);
            }

            VTIgnoreAttribute? ignore = prop.GetCustomAttribute<VTIgnoreAttribute>();

            if (id == null && ignore?.Condition == VTIgnoreCondition.WhenWritingNull)
            {
                return;
            }

            writer.WriteLine($"{indentation}{idLink.PropertyName} = {(id != null ? $"{idLink.ValuePrefix}{id}" : "null")}");
        }

        private static bool ShouldIgnoreValue(PropertyInfo prop, object? value, VTIgnoreAttribute vtIgnore)
        {
            if (prop.PropertyType.IsValueType)
            {
                if (vtIgnore.Condition == VTIgnoreCondition.WhenWritingNull)
                {
                    if (Nullable.GetUnderlyingType(prop.PropertyType) == null)
                        return false;

                    return value == null;
                }
                else if (vtIgnore.Condition == VTIgnoreCondition.WhenWritingDefault)
                {
                    return value?.Equals(Activator.CreateInstance(prop.PropertyType)) ?? true;
                }

                return true;
            }

            if (vtIgnore.Condition == VTIgnoreCondition.WhenWritingDefault ||
                vtIgnore.Condition == VTIgnoreCondition.WhenWritingNull)
            {
                return value == null;
            }

            return true;
        }

        private static string GetName(PropertyInfo prop, bool lower = true)
        {
            VTNameAttribute? nameAttr = prop.GetCustomAttribute<VTNameAttribute>();
            string propName;

            if (nameAttr == null)
            {
                propName = prop.Name;

                if (lower)
                    propName = char.ToLower(propName[0]) + propName[1..];
            }
            else
                propName = nameAttr.Name;

            return propName;
        }
    }
}
