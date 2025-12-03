// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.SVG;

namespace CairoSharp.Extensions.Tests.Fonts.FreeTypeTests;

[TestFixture]
public class Behavorial
{
    [Test]
    public void FontOptions_demo___OK()
    {
        byte[] actual = DrawHelper.DrawFontOptions();

        AssertHelper.AssertSvg(AssertHelper.s_expectedSvgDataFontOptions, AssertHelper.s_expectedSvgStringFontOptions, actual);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void DrawText_demo___OK([Values] bool useDefaultFont)
    {
        byte[] actual = DrawHelper.DrawText(useDefaultFont);

        AssertHelper.AssertSvg(AssertHelper.s_expectedSvgDataText, AssertHelper.s_expectedSvgStringText, actual);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void FontOptions_demo_multiple_times___OK([Values] bool gcCollect)
    {
        // 20 was chosen as debugging showed that at 14 iterations
        // FreeTypeExtensions.DestroyFunc is called.

        for (int i = 0; i < 20; ++i)
        {
            byte[] actual = DrawHelper.DrawFontOptions();

            AssertHelper.AssertSvg(AssertHelper.s_expectedSvgDataFontOptions, AssertHelper.s_expectedSvgStringFontOptions, actual);

            if (gcCollect)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void DrawText_demo_multiple_times___OK([Values] bool useDefaultFont, [Values] bool gcCollect)
    {
        // 20 was chosen as debugging showed that at 14 iterations
        // FreeTypeExtensions.DestroyFunc is called.

        for (int i = 0; i < 20; ++i)
        {
            byte[] actual = DrawHelper.DrawText(useDefaultFont);

            AssertHelper.AssertSvg(AssertHelper.s_expectedSvgDataText, AssertHelper.s_expectedSvgStringText, actual);

            if (gcCollect)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
    //-------------------------------------------------------------------------
    [Test, CancelAfter(10_000)]
    public async Task DrawText_demo_parallel___OK([Values] bool useDefaultFont, [Values] bool gcCollect, CancellationToken cancellationToken)
    {
        using CountdownEvent cde       = new(Environment.ProcessorCount);
        using ManualResetEventSlim mre = new();

        Task managerTask = Task.Run(() =>
        {
            cde.Wait();
            mre.Set();
        }, cancellationToken);

        int id = 0;
        ParallelOptions parallelOptions = new() { CancellationToken = cancellationToken };

        Parallel.For(0, Environment.ProcessorCount, parallelOptions, (_, state) =>
        {
            int loopId = Interlocked.Increment(ref id);
            TestContext.Out.WriteLine($"T-ID (entry): {Environment.CurrentManagedThreadId,2}, Loop-ID: {loopId,2}");

            cde.Signal();
            mre.Wait();

            for (int i = 0; i < 100; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (state.ShouldExitCurrentIteration)
                {
                    break;
                }

                byte[] actualText = DrawHelper.DrawText(useDefaultFont);

                AssertHelper.AssertSvg(AssertHelper.s_expectedSvgDataText, AssertHelper.s_expectedSvgStringText, actualText);

                if (gcCollect)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }

            TestContext.Out.WriteLine($"T-ID (exit): {Environment.CurrentManagedThreadId,2}, Loop-ID: {loopId,2}");
        });

        await managerTask;
    }
    //-------------------------------------------------------------------------
    [Test, CancelAfter(1_500)]
    [Repeat(10)]    // test is flaky
    public async Task Font_create_in_one_thread_and_dispose_in_another_thread(CancellationToken cancellationToken)
    {
        int count                      = Environment.ProcessorCount;
        using CountdownEvent cde       = new(count);
        using ManualResetEventSlim mre = new();

        Task loopTask = Parallel.ForAsync(0, count, async (_, ct) =>
        {
            cde.Signal();
            mre.Wait(ct);

            TestContext.Out.WriteLine($"T-ID: {Environment.CurrentManagedThreadId,2}, create font");
            FreeTypeFont sanRemoFont = Helper.LoadFreeTypeFontFromFile("SanRemo.ttf");

            await Task.Run(sanRemoFont.Dispose, ct);
        });

        cde.Wait(cancellationToken);
        mre.Set();

        await loopTask;
    }
}
