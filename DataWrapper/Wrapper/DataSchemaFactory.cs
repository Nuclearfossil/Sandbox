using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DataWrapper
{

    public class Field
    {
        public string Name { get; private set; }
        public DataType BoundType { get; private set; }
        private readonly char[] delimiters = { '<', '>' };

        public Field(FieldInfo sourceField, DataType dataType)
        {
            if (sourceField.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
            {
                // Actual name is between the <>
                Name = sourceField.Name.Split(delimiters)[1];
            }
            else
            {
                Name = sourceField.Name;
            }
            BoundType = dataType;
        }
    }

    public class Schema
    {
        public readonly string Name;
        public Schema(Type type)
        {
            Name = type.FullName;
            Fields = new List<Field>();
        }

        public List<Field> Fields;
    }

    public class DataType
    {
        private string mBaseName = string.Empty;
        private string mCategory = string.Empty;
        public Type BoundType { get; private set; }
        public Schema Schema { get; set; }

        // Builtin Types do not have a schema
        public bool IsBuiltInType { get; private set; }

        public DataType(Type type, bool isBuiltinType = false)
        {
            BoundType = type;
            FullName = type.FullName;
            IsBuiltInType = isBuiltinType;
            Schema = null;

            if (mBaseName.Equals(string.Empty))
            {
                string[] paths = FullName.Split('.');
                BaseName = paths[paths.Length - 1];
            }
        }

        public string FullName { get; private set; }
        public string BaseName { get; private set; }
    }

    public class DataSchemaFactory
    {
        private static Type sListType = typeof(IList);
        private Dictionary<string, DataType> mKnownTypes;
        private Dictionary<Type, DataType> mBaseTypes;

        public DataSchemaFactory()
        {
            mKnownTypes = new Dictionary<string, DataType>();
            AddBaseTypes();
        }

        public void ParseAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                DataObjectAttribute doa = type.GetCustomAttribute<DataObjectAttribute>();
                if (doa != null)
                {
                    DataType dt = new DataType(type);
                    mKnownTypes.Add(dt.FullName, dt);
                }
            }
        }

        public void FixupKnownTypes()
        {
            foreach (var type in mKnownTypes)
            {
                if ((type.Value.IsBuiltInType == false) && (type.Value.Schema == null))
                {
                    type.Value.Schema = BuildSchemaFromType(type.Value.BoundType);
                }
            }
        }

        private Schema BuildSchemaFromType(Type type)
        {
            List<DataType> QueuedDataTypes = new List<DataType>();

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            Schema result = new Schema(type);

            foreach (var field in fields)
            {
                // get the DataObject in the field
                if (field.FieldType.IsGenericType)
                {
                    if (typeof(IList<>).IsAssignableFrom(field.FieldType.GetGenericTypeDefinition()))
                    {
                        Console.WriteLine("Is an IList");
                    }
                }
                if (field.FieldType.IsArray || sListType.IsAssignableFrom(field.FieldType))
                {
                    Console.WriteLine("Dealing with an array/list type");
                }
                else
                {
                    DataType dt = mKnownTypes[field.FieldType.FullName];
                    Field schemaField = new Field(field, dt);
                    if ((dt.IsBuiltInType) == false && (dt.Schema == null))
                    {
                        QueuedDataTypes.Add(dt);
                    }

                    result.Fields.Add(schemaField);
                }
            }

            foreach (var item in QueuedDataTypes)
            {
                BuildSchemaFromType(item.BoundType);
            }
            return result; ;
        }

        private void AddBaseTypes()
        {
            mBaseTypes = new Dictionary<Type, DataType>();
            mBaseTypes.Add(typeof(char), new DataType(typeof(char), true));
            mBaseTypes.Add(typeof(byte), new DataType(typeof(byte), true));
            mBaseTypes.Add(typeof(sbyte), new DataType(typeof(sbyte), true));
            mBaseTypes.Add(typeof(decimal), new DataType(typeof(decimal), true));
            mBaseTypes.Add(typeof(float), new DataType(typeof(float), true));
            mBaseTypes.Add(typeof(Int16), new DataType(typeof(Int16), true));
            mBaseTypes.Add(typeof(Int32), new DataType(typeof(Int32), true)); // Don't need to add an `int` as it maps to this
            mBaseTypes.Add(typeof(Int64), new DataType(typeof(Int64), true));
            mBaseTypes.Add(typeof(bool), new DataType(typeof(bool), true));
            mBaseTypes.Add(typeof(double), new DataType(typeof(double), true));
            mBaseTypes.Add(typeof(string), new DataType(typeof(string), true));

            foreach (var basetype in mBaseTypes)
            {
                mKnownTypes.Add(basetype.Value.FullName, basetype.Value);
            }
        }

        public void DebugKnowTypes()
        {
            foreach (var baseType in mKnownTypes)
            {
                Type type = baseType.Value.BoundType;
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                PropertyInfo[] properties = type.GetProperties();

                Console.WriteLine("=======================================");
                Console.WriteLine("{0} Type: [{1}] <Should Match> [{2}] BaseName: [{3}]",
                                  baseType.Value.IsBuiltInType ? "Builtin" : "Custom",
                                  type.FullName,
                                  baseType.Value.FullName,
                                  baseType.Value.BaseName);
                if (baseType.Value.IsBuiltInType == false)
                {
                    Console.WriteLine("Fields from Type ----------------------");
                    Array.ForEach(fields, (field) => Console.WriteLine(string.Format("   | Field: {0} {1}", field.Name, field.FieldType)));
                    Console.WriteLine("Properties from Type ------------------");
                    Array.ForEach(properties, (property) => Console.WriteLine(string.Format("   | Property: {0} {1}", property.Name, property.GetType().Name)));
                    Console.WriteLine("Schema (if any) -----------------------");
                    // What does the schema look like?
                    DebugSchema(baseType.Value);
                }

                Console.WriteLine("=======================================");
            }
        }

        private static int mIndentLevel = 0;
        private void DebugSchema(DataType type)
        {
            if (type.IsBuiltInType)
            {
                return;  // No need to process builtin types
            }

            string indent = new string(' ', 4 + 4 * mIndentLevel);
            Schema node = type.Schema;

            foreach (var item in node.Fields)
            {
                Console.WriteLine("{0}| {1} [{2}]", indent, item.Name, item.BoundType.FullName);
                mIndentLevel++;
                DebugSchema(item.BoundType);
                mIndentLevel--;
            }
        }

        private void DoNothing()
        {
        }
    }
}
