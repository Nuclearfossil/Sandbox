using System;
using System.Collections.Generic;


namespace DataWrapper
{
    public class DataObjectAttribute : Attribute
    { }

    public class DataElement
    {
        public string mName;
    }

    public class POCDataElement<T> : DataElement
    {
        POCDataElement(T src)
        {
            mValue = src;
        }
        public T mValue;
    }

    public class CompountDataElement<T> : DataElement
    {
        public List<DataElement> mValues;
    }

    public class StructDataElement<T> : DataElement
    {
        public List<DataElement> mValue;
    }

    public class CollectionDataObject<T> : DataElement where T : DataElement
    {

    }

    public class DataObject
    {
        public DataObject(Type dataType)
        {
            // Iterate over the Fields in the datatype to build our dataset.

        }

        public List<DataElement> mElements;
    }

    public static class DataObjectFactory
    {
        private static Dictionary<Type, DataObject> mCollection = new Dictionary<Type, DataObject>();

        public static void AddType(Type dataType)
        { }

        public static DataElement GetDataObject(string typename)
        {
            return null;
        }
    }
}
