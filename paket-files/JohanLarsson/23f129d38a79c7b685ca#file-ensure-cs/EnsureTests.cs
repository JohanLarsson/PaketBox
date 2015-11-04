using System;
using System.Collections.Generic;

using NUnit.Framework;

public class EnsureTests
{
    [TestCaseSource(typeof(Valids))]
    public void FormatHappyPath(FormatData data)
    {
        Assert.DoesNotThrow(() => Ensure.Format(data.Format, data.Args));
    }

    [TestCaseSource(typeof(InValids))]
    public void FormatThrows(FormatData data)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Ensure.Format(data.Format, data.Args));
        Console.WriteLine(ex.Message);
    }

    private class Valids : List<FormatData>
    {
        public Valids()
        {
            Add(new FormatData(@"some string", null));
            Add(new FormatData(@"some string", new object[0]));
            Add(new FormatData(@"string with {0} parameter", new object[] { 1 }));
            Add(new FormatData(@"string with {0} parameter {0} in to places", new object[] { 1 }));
            Add(new FormatData(@"string with {0} parameter {1} in to places", new object[] { 2, 2 }));
            Add(new FormatData(@"string with {0} parameter {1} in to places {0}", new object[] { 2, 2 }));
            Add(new FormatData("string with {0} parameter {1} in {2} places", new object[] { 1, 2, 3 }));
        }
    }

    private class InValids : List<FormatData>
    {
        public InValids()
        {
            Add(new FormatData("some string", new object[] { 1 }));
            Add(new FormatData("string with {0} parameter", null));
            Add(new FormatData("string with {1} parameter", new object[] { 1 }));
            Add(new FormatData("string with {0} parameter {2}", new object[] { 1, 2 }));
            Add(new FormatData("string with {0} parameter", new object[0]));
            Add(new FormatData("string with {0} parameter", new object[] { 1, 2 }));
            Add(new FormatData("string with {0} parameter {0} in to places", null));
            Add(new FormatData("string with {0} parameter {0} in to places", new object[0]));
            Add(new FormatData("string with {0} parameter {0} in to places", new object[] { 1, 2 }));
            Add(new FormatData("string with {0} parameter {1} in to places", null));
            Add(new FormatData("string with {0} parameter {1} in to places", new object[0]));
            Add(new FormatData("string with {0} parameter {1} in to places", new object[] { 1 }));
            Add(new FormatData("string with {0} parameter {1} in to places", new object[] { 1, 2, 3 }));
        }
    }

    public class FormatData
    {
        public readonly string Format;
        public readonly object[] Args;

        public FormatData(string format, object[] args)
        {
            Format = format;
            Args = args;
        }

        public override string ToString()
        {
            var args = Args == null
                           ? "null"
                           : Args.Length == 0
                                 ? "object[0]"
                                 : string.Join(",", Args);
            return string.Format("Format: {0}, Args: {1}", Format, args);
        }
    }
}