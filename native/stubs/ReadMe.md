# Stubs

## Why?

The name on `DllImport` is `cairo`, therefore the runtime evaluates the dll to
* `cairo.dll` on windows
* `libcairo.so` on linux

But on linux the actual so is `libcairo.so.2` (see `/usr/lib/x86_64-linux-gnu`, there it is a symlink to `libcairo.so.2.11400.6` or similar).
Therefore the so can't be loaded. Possible workaurounds are:
* another symlink ln -s libcairo.so.2 libcairo.so
* a stub libcairo.so that loads libcairo.so.2

The first approach is possible, but requires user-action (and sudo) therefore this is error-prone and not practical.
The second approach requires no user action, hence this one is preferable.


## How?

The idea is to generate a stub shared library that is named like the Windows DLL 
and contains a reference to the real library name for Linux. The DllImport in .NET code 
remains unchanged and uses the Windows DLL name (without .dll suffix). 
The .NET core native loader will then load the stub shared object. This action 
invokes the Linux dynamic loader (ld.so) which then resolves the dependency of the 
stub on the real library and automatically maps all symbols from the real library 
into the stub.

To generate a stub library do the following:

```bash
touch empty.c
gcc -shared -o libLinuxName.so empty.c    
gcc -Wl,--no-as-needed -shared -o libWindowsName.so -fPIC -L. -l:libLinuxName.so
rm -f libLinuxName.so
rm -f empty.c
```

[Source](https://github.com/dotnet/coreclr/issues/930#issuecomment-328542896)
