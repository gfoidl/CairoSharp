// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Fonts;

namespace CairoSharp.Tests;

[TestFixture]
public unsafe class UserDataKeyTests
{
    [Test]
    public void Two_objects_get_and_set_with_same_key___OK()
    {
        using FontFace fontFace1 = new ToyFontFace("Arial");
        using FontFace fontFace2 = new ToyFontFace("Arial", slant: Cairo.Drawing.Text.FontSlant.Italic);

        Data data1 = new() { Value = 1 };
        Data data2 = new() { Value = 2 };

        UserDataKey userDataKey = new();

        fontFace1.SetUserData(ref userDataKey, &data1, null);
        fontFace2.SetUserData(ref userDataKey, &data2, null);

        Data* data1Actual = (Data*)fontFace1.GetUserData(ref userDataKey);
        Data* data2Actual = (Data*)fontFace2.GetUserData(ref userDataKey);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(data1Actual->Value, Is.EqualTo(data1.Value));
            Assert.That(data2Actual->Value, Is.EqualTo(data2.Value));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void Two_objects_get_and_set_with_different_key___OK()
    {
        using FontFace fontFace1 = new ToyFontFace("Arial");
        using FontFace fontFace2 = new ToyFontFace("Arial", slant: Cairo.Drawing.Text.FontSlant.Italic);

        Data data1 = new() { Value = 1 };
        Data data2 = new() { Value = 2 };

        UserDataKey userDataKey1 = new();
        UserDataKey userDataKey2 = new();

        fontFace1.SetUserData(ref userDataKey1, &data1, null);
        fontFace2.SetUserData(ref userDataKey2, &data2, null);

        Data* data1Actual = (Data*)fontFace1.GetUserData(ref userDataKey1);
        Data* data2Actual = (Data*)fontFace2.GetUserData(ref userDataKey2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(data1Actual->Value, Is.EqualTo(data1.Value));
            Assert.That(data2Actual->Value, Is.EqualTo(data2.Value));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void Get_and_set_with_key_in_object_and_GCs_between___throws_NullReference()
    {
        using FontFace fontFace = new ToyFontFace("Arial");
        Data data               = new() { Value = 1 };
        TestObject testObject   = new();

        int gen0    = GC.GetGeneration(testObject);
        void* addr0 = Unsafe.AsPointer(ref testObject.UserDataKey);

        fontFace.SetUserData(ref testObject.UserDataKey, &data, null);

        GC.Collect();   // testObject Gen0 -> Gen1
        GC.Collect();   // testObject Gen1 -> Gen2

        int gen1    = GC.GetGeneration(testObject);
        void* addr1 = Unsafe.AsPointer(ref testObject.UserDataKey);

        TestContext.Out.WriteLine($"""
            Address relocation:
            GC gen: {gen0}, before: 0x{(nint)addr0:x2}
            GC gen: {gen1}, after:  0x{(nint)addr1:x2}
            """);

        Data* actual = (Data*)fontFace.GetUserData(ref testObject.UserDataKey);

        Assume.That(addr0 != addr1);

        Assert.Throws<NullReferenceException>(() => _ = actual->Value);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void Get_and_set_with_key_in_object_that_is_pinned_and_GCs_between___OK()
    {
        using FontFace fontFace = new ToyFontFace("Arial");
        Data data               = new() { Value = 1 };
        TestObject testObject   = new();

        GCHandle gcHandle = GCHandle.Alloc(testObject, GCHandleType.Pinned);

        int gen0    = GC.GetGeneration(testObject);
        void* addr0 = Unsafe.AsPointer(ref testObject.UserDataKey);

        fontFace.SetUserData(ref testObject.UserDataKey, &data, null);

        GC.Collect();   // testObject Gen0 -> Gen1
        GC.Collect();   // testObject Gen1 -> Gen2

        int gen1    = GC.GetGeneration(testObject);
        void* addr1 = Unsafe.AsPointer(ref testObject.UserDataKey);

        TestContext.Out.WriteLine($"""
            Address relocation:
            GC gen: {gen0}, before: 0x{(nint)addr0:x2}
            GC gen: {gen1}, after:  0x{(nint)addr1:x2}
            """);

        Data* actual = (Data*)fontFace.GetUserData(ref testObject.UserDataKey);

        using (Assert.EnterMultipleScope())
        {
            Assert.That((nint)addr0  , Is.EqualTo((nint)addr1));
            Assert.That(actual->Value, Is.EqualTo(data.Value));
        }

        gcHandle.Free();
    }
    //-------------------------------------------------------------------------
    private struct Data
    {
        public int Value;
    }
    //-------------------------------------------------------------------------
    private class TestObject
    {
        private UserDataKey _userDataKey;

        public ref UserDataKey UserDataKey => ref _userDataKey;
    }
}
