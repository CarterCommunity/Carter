namespace Carter.Tests.Modelbinding
{
    using System;
    using System.Collections.Generic;

    public class TestModel
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }

        public double MyDoubleProperty { get; set; }

        public string[] MyArrayProperty { get; set; }

        public IEnumerable<int> MyIntArrayProperty { get; set; }

        public IEnumerable<int> MyIntListProperty { get; set; }

        public Guid MyGuidProperty { get; set; }

        public bool MyBoolProperty { get; set; }

        public DateTime MyDateTimeProperty { get; set; }

        public bool? MyNullableBoolProperty { get; set; }

        public int? MyNullableIntProperty { get; set; }

        public DateTime? MyNullableDateTimeProperty { get; set; }

        public DateTime MyDateTimeWithMillisecondsProperty { get; set; }

        public Uri MyUriProperty { get; set; }

        public Guid? MyNullableGuidProperty { get; set; }

        public bool? MyEmptyNullableBoolProperty { get; set; }

        public int? MyEmptyNullableIntProperty { get; set; }

        public Guid MyEmptyGuidProperty { get; set; }

        public Guid? MyEmptyNullableGuidProperty { get; set; }

        public DateTime? MyEmptyNullableDateTimeProperty { get; set; }

        public Decimal MyDecimalProperty { get; set; }

        public Decimal MyFormattedDecimalProperty { get ; set; }
    }
}