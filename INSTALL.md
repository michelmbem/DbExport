# Installing DbExport

DbExport is distributed from the GitHub Releases page:

<https://github.com/michelmbem/DbExport/releases>

Download the package that matches your operating system and CPU architecture, then follow the platform-specific steps below.

## 1. Choose the Right Package

Release assets use names similar to these:

- `dbexport-x.y.z-win-x64.msi`
- `dbexport-x.y.z-win-arm64.msi`
- `dbexport-x.y.z-linux-x64.tar.gz`
- `dbexport-x.y.z-linux-arm64.tar.gz`
- `dbexport-x.y.z-osx-x64.dmg`
- `dbexport-x.y.z-osx-arm64.dmg`

Architecture guidance:

- `x64`: Most Intel and AMD 64-bit PCs
- `arm64`: Apple Silicon Macs and Windows/Linux ARM devices
- `osx-x64`: Intel-based Mac
- `osx-arm64`: Apple Silicon Mac

If you are unsure which package to pick:

- Windows: Open `Settings > System > About` and check `System type`
- macOS: Open `Apple menu > About This Mac`
- Linux: Run `uname -m`

## 2. Windows Installation

Package to download:

- `dbexport-x.y.z-win-x64.msi` for Intel/AMD 64-bit Windows
- `dbexport-x.y.z-win-arm64.msi` for Windows on ARM

Installation steps:

1. Download the `.msi` package from the Releases page.
2. Double-click the file to launch the installer.
3. If Windows prompts for permission, allow the installer to run.
4. Follow the setup wizard until installation completes.

Notes:

- The MSI installs DbExport system-wide, so administrator rights may be required.
- The installer creates Start Menu and Desktop shortcuts.
- The application is installed under `Program Files\DbExport`.

## 3. Linux Installation

Package to download:

- `dbexport-x.y.z-linux-x64.tar.gz` for Intel/AMD 64-bit Linux
- `dbexport-x.y.z-linux-arm64.tar.gz` for ARM 64-bit Linux

Installation steps:

1. Download the `.tar.gz` archive from the Releases page.
2. Extract it to a folder of your choice.
3. Open a terminal in the extracted folder.
4. If needed, make the binary executable:

```bash
chmod +x dbexport
```

5. Start the application:

```bash
./dbexport
```

Notes:

- Linux packages are distributed as compressed application folders, not native `.deb` or `.rpm` installers.
- You can move the extracted folder anywhere you want, for example under `/opt/DbExport` or inside your home directory.
- If you want a launcher entry in your desktop environment, create it manually according to your distribution's conventions.

## 4. macOS Installation

Package to download:

- `dbexport-x.y.z-osx-x64.dmg` for Intel Macs
- `dbexport-x.y.z-osx-arm64.dmg` for Apple Silicon Macs

Installation steps:

1. Download the `.dmg` file from the Releases page.
2. Open the disk image.
3. Drag `DbExport.app` into the `Applications` folder.
4. Eject the mounted disk image.

### First Launch on macOS

The macOS application bundle is currently not signed. Because of that, macOS may block it the first time you try to open it.

Run this command once in Terminal after copying the app to `Applications`:

```bash
xattr -dr com.apple.quarantine /Applications/DbExport.app
```

Then launch DbExport again from `Applications`.

Notes:

- This step removes the quarantine attribute added by macOS for downloaded apps.
- If you install the app in a different folder, replace `/Applications/DbExport.app` with the actual path.

## 5. After Installation

When DbExport starts, you can configure your source and target database connections and begin the migration workflow.

Platform notes:

- Microsoft Access connections are supported on Windows only.
- On Linux and macOS, use the platform package that matches your CPU architecture exactly.

## 6. Troubleshooting

- If the downloaded package does not start, confirm that you selected the correct OS and architecture from the Releases page.
- On macOS, re-run the `xattr` command if the app was moved after installation.
- On Linux, ensure the `dbexport` file has execute permission.
- On Windows, re-run the MSI installer with administrative privileges if installation is blocked.
