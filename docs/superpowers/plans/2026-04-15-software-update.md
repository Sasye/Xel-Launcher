# 软件更新功能 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在设置页面的"软件更新"面板中实现完整的更新检查与下载功能，支持安装版（Setup.exe）和便携版（Portable.zip）两种下载方式，并在下载失败时分流跳转网盘备用链接。

**Architecture:** `UpdateHelper.cs` 封装所有 GitHub API 调用和下载逻辑；`Setting.Designer.cs` 新增 panelUpdate 内的 UI 控件；`Setting.cs` 绑定更新 UI 逻辑；`Program.cs` 在窗口 Load 后异步静默检查更新。

**Tech Stack:** .NET 9 WinForms, AntdUI, System.Net.Http.HttpClient, System.Text.Json, System.Version

---

## 文件映射

| 文件 | 操作 | 职责 |
|------|------|------|
| `Helpers/UpdateHelper.cs` | 新增 | GitHub API 查询、版本比较、流式下载 |
| `Models/UpdateInfo.cs` | 新增 | 更新信息数据模型 |
| `Forms/Setting.Designer.cs` | 修改 | panelUpdate 内的 UI 控件声明与布局 |
| `Forms/Setting.cs` | 修改 | 更新 UI 逻辑绑定 |
| `Program.cs` | 修改 | 启动时异步静默检查更新 |

---

## Task 1: 创建 UpdateInfo 数据模型

**Files:**
- Create: `Models/UpdateInfo.cs`

- [ ] **Step 1: 创建 UpdateInfo.cs**

```csharp
// Models/UpdateInfo.cs
namespace XelLauncher.Models
{
    public class UpdateInfo
    {
        /// <summary>最新版本号，如 "0.1.6"（已去掉 v 前缀）</summary>
        public string LatestVersion { get; set; }
        /// <summary>GitHub Release body（更新日志）</summary>
        public string Changelog { get; set; }
        /// <summary>安装版 .exe 的直链，可为 null（Asset 不存在时）</summary>
        public string SetupDownloadUrl { get; set; }
        /// <summary>便携版 .zip 的直链，可为 null（Asset 不存在时）</summary>
        public string PortableDownloadUrl { get; set; }
        /// <summary>GitHub Release 页面链接（分流备用）</summary>
        public string ReleasePageUrl { get; set; }
    }
}
```

- [ ] **Step 2: 确认文件编译无误**

在 Visual Studio 中或命令行运行：
```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 3: Commit**

```bash
git add Models/UpdateInfo.cs
git commit -m "feat: add UpdateInfo model"
```

---

## Task 2: 创建 UpdateHelper（API 查询 + 版本比较）

**Files:**
- Create: `Helpers/UpdateHelper.cs`

- [ ] **Step 1: 创建 UpdateHelper.cs（CheckAsync 方法）**

```csharp
// Helpers/UpdateHelper.cs
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using XelLauncher.Models;

namespace XelLauncher.Helpers
{
    public static class UpdateHelper
    {
        private const string ApiUrl =
            "https://api.github.com/repos/lTinchl/Xel-Launcher/releases/latest";

        // 备用网盘链接，GitHub 下载失败时跳转
        public const string FallbackUrl = "TODO";

        private static readonly HttpClient _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        static UpdateHelper()
        {
            _client.DefaultRequestHeaders.Add(
                "User-Agent",
                "XelLauncher/" + System.Windows.Forms.Application.ProductVersion);
            _client.DefaultRequestHeaders.Add(
                "Accept",
                "application/vnd.github+json");
        }

        /// <summary>
        /// 从 GitHub 查询最新 Release，返回 UpdateInfo；
        /// 网络错误或解析失败时返回 null。
        /// </summary>
        public static async Task<UpdateInfo> CheckAsync()
        {
            try
            {
                var json = await _client.GetStringAsync(ApiUrl);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // tag_name 可能是 "v0.1.6" 或 "0.1.6"
                var tagName = root.GetProperty("tag_name").GetString() ?? "";
                var version = tagName.TrimStart('v');
                var changelog = root.GetProperty("body").GetString() ?? "";
                var releaseUrl = root.GetProperty("html_url").GetString() ?? "";

                string setupUrl = null;
                string portableUrl = null;

                if (root.TryGetProperty("assets", out var assets))
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        var name = asset.GetProperty("name").GetString() ?? "";
                        var url  = asset.GetProperty("browser_download_url").GetString() ?? "";
                        if (name.EndsWith("-Setup.exe",    StringComparison.OrdinalIgnoreCase))
                            setupUrl = url;
                        else if (name.EndsWith("-Portable.zip", StringComparison.OrdinalIgnoreCase))
                            portableUrl = url;
                    }
                }

                return new UpdateInfo
                {
                    LatestVersion     = version,
                    Changelog         = changelog,
                    SetupDownloadUrl  = setupUrl,
                    PortableDownloadUrl = portableUrl,
                    ReleasePageUrl    = releaseUrl
                };
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "UpdateHelper.CheckAsync");
                return null;
            }
        }

        /// <summary>
        /// 比较当前版本与最新版本，返回 true 表示有新版本可用。
        /// </summary>
        public static bool IsNewer(string currentVersion, string latestVersion)
        {
            try
            {
                var current = new Version(currentVersion.TrimStart('v'));
                var latest  = new Version(latestVersion.TrimStart('v'));
                return latest > current;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

- [ ] **Step 2: 确认编译无误**

```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 3: Commit**

```bash
git add Helpers/UpdateHelper.cs
git commit -m "feat: add UpdateHelper with CheckAsync and IsNewer"
```

---

## Task 3: UpdateHelper 新增 DownloadAsync 方法

**Files:**
- Modify: `Helpers/UpdateHelper.cs`

- [ ] **Step 1: 在 UpdateHelper 类末尾追加 DownloadAsync 方法**

在 `UpdateHelper.cs` 的最后一个 `}` 前（即类体末尾），添加以下方法：

```csharp
        /// <summary>
        /// 流式下载文件到指定路径，通过 progress 回调汇报进度（0-100）和已下载/总大小字节数。
        /// 抛出异常时由调用方处理。
        /// </summary>
        /// <param name="url">下载 URL</param>
        /// <param name="destPath">目标文件完整路径</param>
        /// <param name="progress">进度回调：(percent 0-100, downloadedBytes, totalBytes)</param>
        /// <param name="ct">取消令牌</param>
        public static async Task DownloadAsync(
            string url,
            string destPath,
            Action<int, long, long> progress,
            System.Threading.CancellationToken ct = default)
        {
            using var response = await _client.GetAsync(
                url,
                System.Net.Http.HttpCompletionOption.ResponseHeadersRead,
                ct);
            response.EnsureSuccessStatusCode();

            var total = response.Content.Headers.ContentLength ?? -1L;
            var dir = System.IO.Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(dir))
                System.IO.Directory.CreateDirectory(dir);

            using var src  = await response.Content.ReadAsStreamAsync(ct);
            using var dest = System.IO.File.Create(destPath);

            var buffer = new byte[81920];
            long downloaded = 0;
            int read;
            while ((read = await src.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                await dest.WriteAsync(buffer, 0, read, ct);
                downloaded += read;
                int pct = total > 0 ? (int)(downloaded * 100 / total) : 0;
                progress?.Invoke(pct, downloaded, total);
            }
        }
```

- [ ] **Step 2: 确认编译无误**

```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 3: Commit**

```bash
git add Helpers/UpdateHelper.cs
git commit -m "feat: add DownloadAsync with progress callback to UpdateHelper"
```

---

## Task 4: 更新 panelUpdate 的 UI 控件（Designer）

**Files:**
- Modify: `Forms/Setting.Designer.cs`

当前 `panelUpdate` 是空面板（只有 `txtLog` 被错误地加入其中，且 `panelLog` 和 `panelUpdate` 共用同一个 `txtLog` 控件——这是 Designer 的 bug，需要同步修正）。

- [ ] **Step 1: 在 Setting.Designer.cs 的字段声明区追加新控件字段**

在文件末尾的字段声明部分（`private System.Windows.Forms.Panel panelUpdate;` 之后）追加：

```csharp
        private AntdUI.Label lblCurrentVersion;
        private AntdUI.Label lblLatestVersion;
        private AntdUI.Label lblCurrentVersionTitle;
        private AntdUI.Label lblLatestVersionTitle;
        private AntdUI.Button btnCheckUpdate;
        private System.Windows.Forms.RichTextBox txtChangelog;
        private AntdUI.Button btnDownloadSetup;
        private AntdUI.Button btnDownloadPortable;
        private AntdUI.Button btnFallback;
        private AntdUI.Progress progressDownload;
        private AntdUI.Label lblDownloadStatus;
        private System.Windows.Forms.TableLayoutPanel tableUpdate;
        private System.Windows.Forms.Panel panelUpdateButtons;
```

- [ ] **Step 2: 修正 panelUpdate 和 panelLog 共用 txtLog 的 bug，并填充 panelUpdate 的控件**

在 `InitializeComponent()` 方法中，找到以下代码段：
```csharp
            //
            // panelUpdate
            //
            panelUpdate.Controls.Add(txtLog);
            panelUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            panelUpdate.Name = "panelUpdate";
            panelUpdate.Visible = false;
```

将其替换为：
```csharp
            //
            // panelUpdate
            //
            panelUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            panelUpdate.Name = "panelUpdate";
            panelUpdate.Visible = false;
            panelUpdate.Padding = new System.Windows.Forms.Padding(8);

            // tableUpdate：版本信息 + 检查按钮
            tableUpdate = new System.Windows.Forms.TableLayoutPanel();
            tableUpdate.ColumnCount = 3;
            tableUpdate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableUpdate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableUpdate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            tableUpdate.RowCount = 2;
            tableUpdate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            tableUpdate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            tableUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            tableUpdate.Height = 60;
            tableUpdate.Name = "tableUpdate";

            lblCurrentVersionTitle = new AntdUI.Label();
            lblCurrentVersionTitle.Text = "当前版本";
            lblCurrentVersionTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            lblCurrentVersionTitle.Name = "lblCurrentVersionTitle";

            lblLatestVersionTitle = new AntdUI.Label();
            lblLatestVersionTitle.Text = "最新版本";
            lblLatestVersionTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            lblLatestVersionTitle.Name = "lblLatestVersionTitle";

            lblCurrentVersion = new AntdUI.Label();
            lblCurrentVersion.Text = "v" + System.Windows.Forms.Application.ProductVersion;
            lblCurrentVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            lblCurrentVersion.Name = "lblCurrentVersion";

            lblLatestVersion = new AntdUI.Label();
            lblLatestVersion.Text = "—";
            lblLatestVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            lblLatestVersion.Name = "lblLatestVersion";

            btnCheckUpdate = new AntdUI.Button();
            btnCheckUpdate.Text = "检查更新";
            btnCheckUpdate.Name = "btnCheckUpdate";
            btnCheckUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            btnCheckUpdate.Margin = new System.Windows.Forms.Padding(4, 2, 0, 2);

            tableUpdate.Controls.Add(lblCurrentVersionTitle, 0, 0);
            tableUpdate.Controls.Add(lblLatestVersionTitle, 1, 0);
            tableUpdate.Controls.Add(lblCurrentVersion, 0, 1);
            tableUpdate.Controls.Add(lblLatestVersion, 1, 1);
            tableUpdate.SetRowSpan(btnCheckUpdate, 2);
            tableUpdate.Controls.Add(btnCheckUpdate, 2, 0);

            // txtChangelog：更新日志
            txtChangelog = new System.Windows.Forms.RichTextBox();
            txtChangelog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtChangelog.Name = "txtChangelog";
            txtChangelog.ReadOnly = true;
            txtChangelog.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            txtChangelog.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtChangelog.Font = new System.Drawing.Font("Consolas", 9F);
            txtChangelog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            txtChangelog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtChangelog.Text = "点击「检查更新」查看最新版本信息";

            // panelUpdateButtons：下载按钮 + 进度
            panelUpdateButtons = new System.Windows.Forms.Panel();
            panelUpdateButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelUpdateButtons.Height = 80;
            panelUpdateButtons.Name = "panelUpdateButtons";
            panelUpdateButtons.Visible = false;

            btnDownloadSetup = new AntdUI.Button();
            btnDownloadSetup.Text = "⬇ 下载安装版";
            btnDownloadSetup.Name = "btnDownloadSetup";
            btnDownloadSetup.Width = 130;
            btnDownloadSetup.Height = 34;
            btnDownloadSetup.Location = new System.Drawing.Point(0, 4);

            btnDownloadPortable = new AntdUI.Button();
            btnDownloadPortable.Text = "⬇ 下载便携版";
            btnDownloadPortable.Name = "btnDownloadPortable";
            btnDownloadPortable.Width = 130;
            btnDownloadPortable.Height = 34;
            btnDownloadPortable.Location = new System.Drawing.Point(138, 4);

            btnFallback = new AntdUI.Button();
            btnFallback.Text = "打开网盘下载页";
            btnFallback.Name = "btnFallback";
            btnFallback.Width = 130;
            btnFallback.Height = 34;
            btnFallback.Location = new System.Drawing.Point(0, 4);
            btnFallback.Visible = false;
            btnFallback.Type = AntdUI.TTypeMini.Warn;

            progressDownload = new AntdUI.Progress();
            progressDownload.Name = "progressDownload";
            progressDownload.Dock = System.Windows.Forms.DockStyle.Bottom;
            progressDownload.Height = 8;
            progressDownload.Value = 0F;
            progressDownload.Visible = false;

            lblDownloadStatus = new AntdUI.Label();
            lblDownloadStatus.Name = "lblDownloadStatus";
            lblDownloadStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblDownloadStatus.Height = 20;
            lblDownloadStatus.Text = "";

            panelUpdateButtons.Controls.Add(btnDownloadSetup);
            panelUpdateButtons.Controls.Add(btnDownloadPortable);
            panelUpdateButtons.Controls.Add(btnFallback);
            panelUpdateButtons.Controls.Add(progressDownload);
            panelUpdateButtons.Controls.Add(lblDownloadStatus);

            panelUpdate.Controls.Add(txtChangelog);
            panelUpdate.Controls.Add(tableUpdate);
            panelUpdate.Controls.Add(panelUpdateButtons);
```

- [ ] **Step 3: 确认编译无误**

```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 4: Commit**

```bash
git add Forms/Setting.Designer.cs
git commit -m "feat: add update panel UI controls to Setting.Designer"
```

---

## Task 5: Setting.cs 绑定更新 UI 逻辑

**Files:**
- Modify: `Forms/Setting.cs`

- [ ] **Step 1: 在 Setting.cs 顶部追加 using 语句**

在现有 using 之后追加：
```csharp
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XelLauncher.Models;
```

- [ ] **Step 2: 在 Setting 类中添加字段和 BindUpdatePanel 方法**

在 `Setting` 类的字段声明处（`AntdUI.BaseForm form;` 之后）追加：
```csharp
        private UpdateInfo _updateInfo;
        private CancellationTokenSource _downloadCts;
```

然后在构造函数末尾（最后一个 `switch10.CheckedChanged` 事件绑定之后，`}` 之前）添加：
```csharp
            BindUpdatePanel();
```

- [ ] **Step 3: 在 Setting.cs 末尾（类体内，最后一个 `}` 前）添加更新面板方法**

```csharp
        private void BindUpdatePanel()
        {
            btnCheckUpdate.Click += async (s, e) => await CheckUpdateAsync();
            btnDownloadSetup.Click    += async (s, e) => await DownloadAsync(isSetup: true);
            btnDownloadPortable.Click += async (s, e) => await DownloadAsync(isSetup: false);
            btnFallback.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = UpdateHelper.FallbackUrl,
                    UseShellExecute = true
                });
            };
        }

        private async Task CheckUpdateAsync()
        {
            btnCheckUpdate.Text = "检查中...";
            btnCheckUpdate.Enabled = false;
            try
            {
                var info = await UpdateHelper.CheckAsync();
                if (info == null)
                {
                    txtChangelog.Text = "检查失败，请检查网络连接。";
                    lblLatestVersion.Text = "—";
                    return;
                }

                _updateInfo = info;
                lblLatestVersion.Text = "v" + info.LatestVersion;

                var currentVer = System.Windows.Forms.Application.ProductVersion;
                if (UpdateHelper.IsNewer(currentVer, info.LatestVersion))
                {
                    txtChangelog.Text = info.Changelog;
                    panelUpdateButtons.Visible = true;
                    // 重置下载区状态
                    btnDownloadSetup.Visible    = true;
                    btnDownloadPortable.Visible = true;
                    btnFallback.Visible         = false;
                    progressDownload.Visible    = false;
                    progressDownload.Value      = 0F;
                    lblDownloadStatus.Text      = "";
                }
                else
                {
                    txtChangelog.Text = "已是最新版本 🎉";
                    panelUpdateButtons.Visible = false;
                }
            }
            finally
            {
                btnCheckUpdate.Text    = "检查更新";
                btnCheckUpdate.Enabled = true;
            }
        }

        private async Task DownloadAsync(bool isSetup)
        {
            if (_updateInfo == null) return;

            string url = isSetup ? _updateInfo.SetupDownloadUrl : _updateInfo.PortableDownloadUrl;
            if (string.IsNullOrEmpty(url))
            {
                // Asset 不存在，直接跳转网盘
                ShowFallback();
                return;
            }

            // 便携版：先弹 SaveFileDialog
            string destPath;
            if (!isSetup)
            {
                var sfd = new System.Windows.Forms.SaveFileDialog
                {
                    Title            = "保存便携版",
                    FileName         = $"XelLauncher.v{_updateInfo.LatestVersion}-Portable.zip",
                    Filter           = "ZIP 压缩包|*.zip",
                    DefaultExt       = "zip",
                    RestoreDirectory = true
                };
                if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                destPath = sfd.FileName;
            }
            else
            {
                var tmpDir = Path.Combine(Path.GetTempPath(), "XelLauncher_Update");
                destPath = Path.Combine(tmpDir,
                    $"XelLauncher-{_updateInfo.LatestVersion}-Setup.exe");
            }

            // 禁用下载按钮，显示进度
            btnDownloadSetup.Enabled    = false;
            btnDownloadPortable.Enabled = false;
            progressDownload.Visible    = true;
            progressDownload.Value      = 0F;
            lblDownloadStatus.Text      = "准备下载...";

            _downloadCts = new CancellationTokenSource();

            try
            {
                await UpdateHelper.DownloadAsync(url, destPath,
                    (pct, downloaded, total) =>
                    {
                        if (!IsHandleCreated) return;
                        Invoke(() =>
                        {
                            progressDownload.Value  = pct;
                            var dlMB    = downloaded / 1048576.0;
                            var totalMB = total > 0 ? total / 1048576.0 : 0;
                            lblDownloadStatus.Text  = total > 0
                                ? $"{dlMB:F1} MB / {totalMB:F1} MB  {pct}%"
                                : $"{dlMB:F1} MB";
                        });
                    },
                    _downloadCts.Token);

                // 下载成功
                if (isSetup)
                {
                    // 写 bat 脚本，等待 2 秒后启动安装程序
                    var batPath = Path.Combine(Path.GetTempPath(), "XelLauncher_Update", "update.bat");
                    var exePath = System.Windows.Forms.Application.ExecutablePath;
                    File.WriteAllText(batPath,
                        $"@echo off\r\n" +
                        $"TIMEOUT /T 2 /NOBREAK >nul\r\n" +
                        $"start \"\" \"{destPath}\"\r\n" +
                        $"exit\r\n");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName        = "cmd.exe",
                        Arguments       = $"/c \"{batPath}\"",
                        WindowStyle     = System.Diagnostics.ProcessWindowStyle.Hidden,
                        UseShellExecute = true
                    });
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    // 便携版：下载完成后打开文件所在文件夹并选中文件
                    lblDownloadStatus.Text = "下载完成！";
                    System.Diagnostics.Process.Start("explorer.exe",
                        $"/select,\"{destPath}\"");
                }
            }
            catch (OperationCanceledException)
            {
                lblDownloadStatus.Text = "已取消";
                CleanupTempFile(destPath);
            }
            catch (Exception)
            {
                lblDownloadStatus.Text = "下载失败";
                CleanupTempFile(destPath);
                ShowFallback();
            }
            finally
            {
                btnDownloadSetup.Enabled    = true;
                btnDownloadPortable.Enabled = true;
            }
        }

        private void ShowFallback()
        {
            btnDownloadSetup.Visible    = false;
            btnDownloadPortable.Visible = false;
            btnFallback.Visible         = true;
        }

        private static void CleanupTempFile(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        }
```

- [ ] **Step 4: 确认编译无误**

```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 5: Commit**

```bash
git add Forms/Setting.cs
git commit -m "feat: bind update panel logic in Setting.cs"
```

---

## Task 6: Program.cs 启动时静默检查更新

**Files:**
- Modify: `Program.cs`

在 `Overview` 窗口的 `Load` 事件之后异步触发更新检查，有新版本时通过 `AntdUI.Notification` 弹出提示。

- [ ] **Step 1: 修改 Program.cs 的 Overview 运行代码**

找到以下代码：
```csharp
            else Application.Run(new Overview(command == "t"));
```

替换为：
```csharp
            else
            {
                var overview = new Overview(command == "t");
                overview.Load += async (s, e) =>
                {
                    await Task.Delay(3000); // 等待主界面完全渲染
                    await CheckUpdateSilentAsync(overview);
                };
                Application.Run(overview);
            }
```

- [ ] **Step 2: 在 Program 类末尾添加 CheckUpdateSilentAsync 静态方法**

在 `IsSystemDarkMode` 方法之后（类体内）追加：

```csharp
        /// <summary>
        /// 静默检查更新：有新版本且未通知过同一版本时弹出通知。
        /// </summary>
        static async Task CheckUpdateSilentAsync(AntdUI.Window owner)
        {
            try
            {
                var info = await Helpers.UpdateHelper.CheckAsync();
                if (info == null) return;

                var cfg = Helpers.ConfigHelper.Load();
                var currentVer = System.Windows.Forms.Application.ProductVersion;
                if (!Helpers.UpdateHelper.IsNewer(currentVer, info.LatestVersion)) return;
                if (cfg.LastNotifiedVersion == info.LatestVersion) return;

                // 更新已通知版本，避免重复弹出
                cfg.LastNotifiedVersion = info.LatestVersion;
                Helpers.ConfigHelper.Save(cfg);

                owner.Invoke(() =>
                {
                    AntdUI.Notification.open(new AntdUI.Notification.Config(owner,
                        "发现新版本 v" + info.LatestVersion,
                        AntdUI.TType.Info)
                    {
                        AutoClose = 6
                    });
                });
            }
            catch (Exception ex)
            {
                Helpers.LogHelper.LogError(ex, "CheckUpdateSilentAsync");
            }
        }
```

- [ ] **Step 3: 在 Program.cs 顶部补全 using**

确认顶部有：
```csharp
using System.Threading.Tasks;
```
（已存在，无需添加）

- [ ] **Step 4: 确认编译无误**

```
dotnet build XelLauncher.csproj -c Debug
```
Expected: Build succeeded, 0 Error(s)

- [ ] **Step 5: Commit**

```bash
git add Program.cs
git commit -m "feat: silent update check on app start with AntdUI.Notification"
```

---

## Task 7: 手动测试与收尾

**Files:**
- Modify: `Helpers/UpdateHelper.cs`（替换 FallbackUrl TODO）

- [ ] **Step 1: 将 FallbackUrl 的 TODO 替换为真实网盘链接**

在 `UpdateHelper.cs` 中找到：
```csharp
        public const string FallbackUrl = "TODO";
```
替换为你的真实网盘链接，例如：
```csharp
        public const string FallbackUrl = "https://your-cloud-drive-link-here";
```

- [ ] **Step 2: 运行软件，进入 设置 → 软件更新，点击「检查更新」**

验证以下行为：
- 当前版本正确显示（如 `v0.1.5`）
- 最新版本从 GitHub 正确读取并显示
- 若有新版本：更新日志显示、下载区出现
- 若已最新：显示"已是最新版本 🎉"

- [ ] **Step 3: 测试便携版下载**

点击「下载便携版」→ SaveFileDialog 弹出 → 选择路径 → 进度条正常更新 → 下载完成后资源管理器打开并选中文件

- [ ] **Step 4: 测试网络失败分流**

断开网络后点击「检查更新」→ 提示"检查失败，请检查网络连接。"

- [ ] **Step 5: Commit 最终修改**

```bash
git add Helpers/UpdateHelper.cs
git commit -m "chore: replace FallbackUrl placeholder with real link"
```

---

## 实现完成后

所有 Task 完成后，运行完整构建确认无报错：
```
dotnet build XelLauncher.csproj -c Release
```

然后使用 `superpowers:requesting-code-review` 进行代码审查。
