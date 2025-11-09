## Gtk4Animation

[Gtk4AnimationWithBuilder](../Gtk4AnimationWithBuilder/ReadMe.md) is similar, just with UI builder.

---

When the check box for _creating images_ is checked, then it's possible to create a video from the sequence of stored images.

```bash
# To start at a specific number, see https://trac.ffmpeg.org/wiki/Slideshow

ffmpeg -f image2 -framerate 25 -i output\img%03d.png -vcodec libx264 -crf 22 animation.mp4
```
