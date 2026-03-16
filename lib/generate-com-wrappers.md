# How to Generate .NET Wrappers for COM Libraries

On a Windows machine, launch the *Developper Prompt for Visual Studio* and navigate to this directory, then execute the following commands:

## For ADODB

```shell
tlbimp "C:\Program Files\Common Files\System\ado\msado15.dll" /namespace:ADODB /out:Interop.ADODB.dll
```

## For ADOX

```shell
tlbimp "C:\Program Files\Common Files\System\ado\msadox.dll" /namespace:ADOX /out:Interop.ADOX.dll
```

**Notes:**

The above commands will generate .NET assemblies that can be linked to the project and allow it to be compiled on any platform, but can only be properly invoked on Windows.
