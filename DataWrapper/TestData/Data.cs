using System.Collections.Generic;

namespace DataWrapper.TestData
{
    [DataObject]
    public class Data
    {
        public int FirstNumber;
        public double SecondNumber;
        public string Value;

        private string PrivateStringValue01;
        private int PrivateIntegerValue01;
    }

    [DataObject]
    public class DataInPropeties
    {
        public int PropertyValue01 { get; set; }
        public string PropertyValue02 { get; set; }

        public Data ContainedData { get; set; }
    }

    [DataObject]
    public class ListTypes
    {
        public List<Data> mData;
        public int[] mArrayOfInts;
        public string[] mArrayOfStrings;
        public Data[] mArrayOfData;
    }

    [DataObject]
    struct TestStruct01
    {
        int FirstNumber;
        double SecondNumber;
        string Value;
    }

    [DataObject]
    struct TestStruct02
    {
        float FirstFloat;
        string Value;

        TestStruct01 DataSet;
    }

    [DataObject]
    struct TestStruct03
    {
        Data mData01;
        int mIntegerValue;
        DataInPropeties mData02;
        float mFloatValue;
        string mStringValue;
    }

    public struct PropertyStruct01
    {
        float value01;
        int value02;
        string value03;

        public float Value01
        {
            get
            {
                return value01;
            }
            set
            {
                value01 = value;
            }
        }

        public int Value02
        {
            get
            {
                return value02;
            }
            set
            {
                value02 = value;
            }
        }

        public string Value03
        {
            get
            {
                return value03;
            }
            set
            {
                value03 = value;
            }
        }
    }
}
