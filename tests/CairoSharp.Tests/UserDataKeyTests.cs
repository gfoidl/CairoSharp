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

        fontFace1.SetUserData(&userDataKey, &data1, null);
        fontFace2.SetUserData(&userDataKey, &data2, null);

        Data* data1Actual = (Data*)fontFace1.GetUserData(&userDataKey);
        Data* data2Actual = (Data*)fontFace2.GetUserData(&userDataKey);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(data1Actual->Value, Is.EqualTo(data1.Value));
            Assert.That(data2Actual->Value, Is.EqualTo(data2.Value));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void One_object_two_keys___OK()
    {
        using FontFace fontFace = new ToyFontFace("Arial");

        Data data1 = new() { Value = 1 };
        Data data2 = new() { Value = 2 };

        UserDataKey userDataKey1 = new();
        UserDataKey userDataKey2 = new();

        fontFace.SetUserData(&userDataKey1, &data1, null);
        fontFace.SetUserData(&userDataKey2, &data2, null);

        Data* data1Actual = (Data*)fontFace.GetUserData(&userDataKey1);
        Data* data2Actual = (Data*)fontFace.GetUserData(&userDataKey2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(data1Actual->Value, Is.EqualTo(data1.Value));
            Assert.That(data2Actual->Value, Is.EqualTo(data2.Value));
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void One_object_one_key_data_updated___OK()
    {
        using FontFace fontFace = new ToyFontFace("Arial");

        Data data1 = new() { Value = 1 };
        Data data2 = new() { Value = 2 };

        UserDataKey userDataKey = new();

        fontFace.SetUserData(&userDataKey, &data1, null);
        Data* data1Actual = (Data*)fontFace.GetUserData(&userDataKey);
        Assert.That(data1Actual->Value, Is.EqualTo(data1.Value));

        fontFace.SetUserData(&userDataKey, &data2, null);
        Data* data2Actual = (Data*)fontFace.GetUserData(&userDataKey);
        Assert.That(data2Actual->Value, Is.EqualTo(data2.Value));
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

        fontFace1.SetUserData(&userDataKey1, &data1, null);
        fontFace2.SetUserData(&userDataKey2, &data2, null);

        Data* data1Actual = (Data*)fontFace1.GetUserData(&userDataKey1);
        Data* data2Actual = (Data*)fontFace2.GetUserData(&userDataKey2);

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

        fontFace.SetUserData((UserDataKey*)Unsafe.AsPointer(ref testObject.UserDataKey), &data, null);

        GC.Collect();   // testObject Gen0 -> Gen1
        GC.Collect();   // testObject Gen1 -> Gen2

        int gen1    = GC.GetGeneration(testObject);
        void* addr1 = Unsafe.AsPointer(ref testObject.UserDataKey);

        TestContext.Out.WriteLine($"""
            Address relocation:
            GC gen: {gen0}, before: 0x{(nint)addr0:x2}
            GC gen: {gen1}, after:  0x{(nint)addr1:x2}
            """);

        Data* actual = (Data*)fontFace.GetUserData((UserDataKey*)Unsafe.AsPointer(ref testObject.UserDataKey));

        Assume.That(gen0, Is.Zero);
        Assume.That(gen1, Is.EqualTo(2));
        Assume.That(addr0 != addr1);

        Assert.Throws<NullReferenceException>(() => _ = actual->Value);

        // Note: it's hard to reproduce in tests here, but the same applies to
        // static classes too, as they're relocated by the GC also.
        // See https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmARgFgAoIgBgAIiyA6AJXwwEsBbGZgYQjcADpwA2MKAGVJAN05gYAZwDc1Ooxbs8XXswCSOyRGEyo8xaurV8SgIYAzGNQDe1eh8YoAVPTsATfygGAF56AFU8eydmAEElAAUITiMoAApYB3oAFWUMAHlgACsYMAwWcKVJABE7DDsAaRgATwBKNSp3TwBxfgEIMQkytPauj17+wdKMEY7PejGvXwCgsnowyOi+eKSUjEkMmCzcpQLi6YqqqFr6pra5z0WmAE40gBIAIi/F+djA2CUSnosDEEDAdU4EDwIB+nmAR2gMBA9FoCBcaTwe1aK2CIAQJAAvrCPI59lBkSi0RisTiyHjCcT6GAABZ2PAAcxg/mRLhxDAAhGFaUSqPNPF8PqMqCKAPQyuAKxVK5Uq1Vq9UazWq9QAZk0SEYJByeUKJTKlCoblFnmEUE4sjqMH1ESuN0aLXoSgA+jhXXV3c0Ok89UwDZkXTV/XcI9cox6QgA+YFHT0+v23FodWXyrW5vP5gsK6gAbSkGFwZQAMnZmhAcDNq7X6w0Uv5mDIAI44GA6Th2MStAC6us95ZwZRjbrurkWcpl9AAPEocNxuHYoM0E7O5fQ8BAMPRfVyVPROOy97BmNv5wuZcvV+vN8HTzoIngj/4s0A
        // for a demonstration.
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

        fontFace.SetUserData((UserDataKey*)Unsafe.AsPointer(ref testObject.UserDataKey), &data, null);

        GC.Collect();   // testObject Gen0 -> Gen1
        GC.Collect();   // testObject Gen1 -> Gen2

        int gen1    = GC.GetGeneration(testObject);
        void* addr1 = Unsafe.AsPointer(ref testObject.UserDataKey);

        TestContext.Out.WriteLine($"""
            Address relocation:
            GC gen: {gen0}, before: 0x{(nint)addr0:x2}
            GC gen: {gen1}, after:  0x{(nint)addr1:x2}
            """);

        Data* actual = (Data*)fontFace.GetUserData((UserDataKey*)Unsafe.AsPointer(ref testObject.UserDataKey));

        Assume.That(gen0, Is.Zero);
        Assume.That(gen1, Is.EqualTo(2));

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
