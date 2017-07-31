using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
