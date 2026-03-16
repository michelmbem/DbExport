# How to Generate a Distributable Binary Version of the Project

Execute one of the following commands to generate a distributable binary version of the project that will be optimized for a specific OS platform and/or architecture:

## Windows

### Intel 64-bit processor

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r win-x64 -o publish/win-x64
```

### ARM 64-bit processor

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r win-arm64 -o publish/win-arm64
```

### Packaging

If you have the [WiX toolset](https://github.com/wixtoolset/wix3/releases) installed on your developper machine, you can execute the following commands to generate a MSI package from the compiled binaries:

```pwsh
heat dir "publish\win-x64" -cg PublishedFiles -dr INSTALLFOLDER -var var.PublishDir -gg -srd -sfrag -sreg -out installer\Files.wxs
candle installer\Product.wxs installer\Files.wxs "-dPublishDir=publish\win-x64" "-dVersion=x.y.z"
light Product.wixobj Files.wixobj -ext WixUIExtension -o "dbexport-x.y.z-win-x64.msi"
```

**Notes:**

1. The *installer* directory already contains a *Product.wxs* file that describes how the product should be deployed on a Windows machine.
2. The `heat` command will produce a list off all the files to include in the deployment package and store them in a *Files.wxs* file in the *installer* directory.
3. The `candle` command will compile both *.wxs* files to corresponding *.wixobj* binary files.
4. The `light` command will invoke the WiX linker to combine all *.wixobj* files in a single MSI archive.
5. The *x.y.z* sequence represents the target version number. It should typically be something like *3.2.1*.
6. Depending on your processor architecture, you can replace *win-x64* by *win-arm64*.

## Linux

### Intel 64-bit processor

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r linux-x64 -o publish/linux-x64
```

### ARM 64-bit processor

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r linux-arm64 -o publish/linux-arm64
```

### Packaging

You can simply distribute the Linux version of your application as a compressed tarball:

```shell
cd publish/linux-x64
tar -czf ../../dbexport-x.y.z-linux-x64.tar.gz *
```

**Notes:**

1. As on Windows, the *x.y.z* sequence represents the target version number.
2. Depending on your processor architecture, you can replace *linux-x64* by *linux-arm64*.

## macOS

### Intel chip

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r osx-x64 -o publish/osx-x64
```

### Apple Silicon chip

```shell
dotnet publish src/DbExport.Gui/DbExport.Gui.csproj -c Release -r osx-arm64 -o publish/osx-arm64
```

Packaging

It would make sense to distribute the macOS version of this application as a DMG disc image containing a *.app* application bundle.
Here is how to proceed.

Launch a terminal window from the solution's folder and run the following commands:

```shell
# Generate the application bundle
APP="dbexport.app"

mkdir -p $APP/Contents/MacOS
mkdir -p $APP/Contents/Resources

cp -R publish/osx-arm64/* $APP/Contents/MacOS/
cp src/DbExport.Gui/Assets/Images/DbExport.icns $APP/Contents/Resources/

# Add the Info.plist file to the application bundle
cat > $APP/Contents/Info.plist <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <dict>
        <key>CFBundleName</key>
        <string>DbExport</string>

        <key>CFBundleExecutable</key>
        <string>dbexport</string>

        <key>CFBundleIdentifier</key>
        <string>org.addy.dbexport</string>

        <key>CFBundleVersion</key>
        <string>x.y.z</string>

        <key>CFBundlePackageType</key>
        <string>APPL</string>

        <key>CFBundleIconFile</key>
        <string>DbExport</string>
    </dict>
</plist>
EOF

# Create the DMG image
hdiutil create -fs HFS+ -srcfolder dbexport.app -volname DbExport -format UDZO -ov dbexport-x.y.z-osx-arm64.dmg
```

**Notes:**

1. As on Windows and Linux, the *x.y.z* sequence represents the target version number.
2. Depending on your processor architecture, you may need to replace *osx-arm64* by *osx-x64*.
3. The generated application bundle is not signed. Users will have to execute the `xattr -dr com.apple.quarantine /Applications/DbExport.app` command on their machine before launching it for the first time to prevent OSX from complaining about a corrupted app.
