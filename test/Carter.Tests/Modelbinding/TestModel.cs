namespace Carter.Tests.Modelbinding
{
    using System.Collections.Generic;

    public class TestModel
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }

        public double MyDoubleProperty { get; set; }

        public string[] MyArrayProperty { get; set; }

        public IEnumerable<int> MyIntArrayProperty { get; set; }

        public IEnumerable<int> MyIntListProperty { get; set; }
    }
}