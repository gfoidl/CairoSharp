// (c) gfoidl, all rights reserved

using Cairo;

namespace CairoSharp.Tests.Utilities;

[TestFixture]
public class CairoAPITests
{
    [Test]
    public void CheckSupportedVersion_version_is_supported___nop()
    {
        Assert.DoesNotThrow(() =>
        {
            CairoAPI.CheckSupportedVersion(1, 18, 0);
        });
    }

    [Test]
    public void CheckSupportedVersion_version_not_supported___throws_NotSupported()
    {
        NotSupportedException actual = Assert.Throws<NotSupportedException>(() =>
        {
            CairoAPI.CheckSupportedVersion(1, 19, 0);
        });

        Assert.That(actual.Message, Does.StartWith("The feature is only available from Cairo version 1.19.0"));
    }
}
