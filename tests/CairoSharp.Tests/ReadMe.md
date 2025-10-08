# Tests

In the [cairo source](https://gitlab.com/saiwp/cairo/-/tree/master/test?ref_type=heads) there are lots of tests.
Ideally these tests are run in the .NET wrapper too. But that's a lot -- and here really a lot -- of work, which I won't do now.

If someone encouters a :bug:, please file an issue. Then a test for reproducing this will be added, so that at least that bug won't happen again.

On the other hand, the most time this is a quite thin wrapper around cairo.
There are only a handful members where some logic is performed. Thus it's a bit different than normal libraries, which should of course have lots of tests.
