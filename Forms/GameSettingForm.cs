using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using XelLauncher.Helpers;
using XelLauncher.Models;

namespace XelLauncher.Forms
{
    public class GameSettingForm : UserControl
    {
        private readonly GameEntry _game;
        private readonly Overview _overview;
        private AntdUI.Input _inputPath;

        public GameSettingForm(GameEntry game, Overview overview)
        {
            _game = game;
            _overview = overview;
            var cfg = ConfigHelper.Load();
            var latest = cfg.Games.Find(g => g.Name == game.Name && g.IconName == game.IconName);
            string currentPath = latest?.RootPath ?? game.RootPath;

            Font = new Font("Microsoft YaHei UI", 9F);
            Size = new Size(360, 290);

            // ── 游戏图标 ──
            var picIcon = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(48, 48),
                Location = new Point(20, 20),
            };
            try
            {
                var ico = LoadIcon(game.IconName);
                if (ico != null)
                {
                    var src = ico.ToBitmap();
                    var dst = new Bitmap(48, 48);
                    using var g = Graphics.FromImage(dst);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(src, 0, 0, 48, 48);
                    picIcon.Image = dst;
                }
            }
            catch { }

            // ── 游戏名 ──
            string displayName = game.Name;
            var lblName = new AntdUI.Label
            {
                Text = displayName,
                Location = new Point(80, 20),
                Size = new Size(260, 28),
                Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold),
            };

            // ── 游戏版本 ──
            bool isEndfield = game.IconName == "Endfield" || game.IconName == "BiliEndfield" || game.IconName == "GlobalEndfield";
            var lblVersion = new AntdUI.Label
            {
                Text = isEndfield ? "版本：v1.1.9" : "版本：v71.0.0",
                Location = new Point(80, 48),
                Size = new Size(260, 24),
                Font = new Font("Microsoft YaHei UI", 9F),
            };

            // ── 分割线 ──
            var divider1 = new AntdUI.Divider
            {
                Location = new Point(20, 82),
                Size = new Size(320, 1),
                Thickness = 1F,
            };

            // ── 游戏安装路径 标题 ──
            var lblPathSection = new AntdUI.Label
            {
                Text = "游戏安装路径",
                Location = new Point(20, 94),
                Size = new Size(320, 24),
                Font = new Font("Microsoft YaHei UI", 9F),
            };

            // ── 安装路径输入框 ──
            _inputPath = new AntdUI.Input
            {
                Text = currentPath,
                Location = new Point(20, 124),
                Size = new Size(320, 36),
                ReadOnly = true,
                PlaceholderText = "未设置路径",
            };

            // ── 更改路径 ──
            var btnBrowse = new AntdUI.Button
            {
                Text = "更改路径",
                Location = new Point(20, 172),
                Size = new Size(320, 36),
                Ghost = true,
            };
            btnBrowse.Click += (s, e) => BrowsePath();

            // ── 打开文件目录 ──
            var btnOpenDir = new AntdUI.Button
            {
                Text = "打开文件目录",
                Location = new Point(20, 220),
                Size = new Size(320, 36),
                Ghost = true,
            };
            btnOpenDir.Click += (s, e) =>
            {
                string path = _inputPath.Text.Trim();
                if (Directory.Exists(path))
                    Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
            };

            if (game.IconName == "BiliArknights")
            {
                var btnBili = new AntdUI.Button
                {
                    Text = "Arknights BiliBili官网",
                    Location = new Point(20, 268),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btnBili.Click += (s, e) =>
                    Process.Start(new ProcessStartInfo("https://www.biligame.com/detail/?id=117664") { UseShellExecute = true });
                Controls.Add(btnBili);
                Size = new Size(360, 338);
            }
            else if (game.IconName == "Endfield")
            {
                var btn = new AntdUI.Button
                {
                    Text = "Endfield 官网",
                    Location = new Point(20, 268),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btn.Click += (s, e) =>
                    Process.Start(new ProcessStartInfo("https://endfield.hypergryph.com/") { UseShellExecute = true });
                Controls.Add(btn);

                var btnSync = new AntdUI.Button
                {
                    Text = "同步路径到 BillBili服 / 国际服",
                    Location = new Point(20, 316),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btnSync.Click += (s, e) =>
                {
                    string currentPath = _inputPath.Text.Trim();
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        AntdUI.Message.warn(_overview, "请先设置官服路径");
                        return;
                    }
                    var cfg = ConfigHelper.Load();
                    foreach (var icon in new[] { "BiliEndfield", "GlobalEndfield" })
                    {
                        var other = cfg.Games.Find(g => g.IconName == icon);
                        if (other != null) other.RootPath = currentPath;
                    }
                    ConfigHelper.Save(cfg);
                    AntdUI.Message.success(_overview, "路径已同步到 BillBili服 和 国际服");
                };
                Controls.Add(btnSync);
                Size = new Size(360, 386);
            }
            else if (game.IconName == "BiliEndfield")
            {
                var btn = new AntdUI.Button
                {
                    Text = "Endfield BiliBili官网",
                    Location = new Point(20, 268),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btn.Click += (s, e) =>
                    Process.Start(new ProcessStartInfo("https://www.biligame.com/detail/?id=105651") { UseShellExecute = true });
                Controls.Add(btn);
                Size = new Size(360, 338);
            }
            else if (game.IconName == "GlobalEndfield")
            {
                var btn = new AntdUI.Button
                {
                    Text = "Endfield 国际服官网",
                    Location = new Point(20, 268),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btn.Click += (s, e) =>
                    Process.Start(new ProcessStartInfo("https://endfield.hypergryph.com/en-US/") { UseShellExecute = true });
                Controls.Add(btn);
                Size = new Size(360, 338);
            }
            else
            {
                var btnguan = new AntdUI.Button
                {
                    Text = "Arknights 官网",
                    Location = new Point(20, 268),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btnguan.Click += (s, e) =>
                    Process.Start(new ProcessStartInfo("https://ak.hypergryph.com/") { UseShellExecute = true });
                Controls.Add(btnguan);

                var btnSync = new AntdUI.Button
                {
                    Text = "同步路径到 BillBili服",
                    Location = new Point(20, 316),
                    Size = new Size(320, 36),
                    Ghost = true,
                };
                btnSync.Click += (s, e) =>
                {
                    string currentPath = _inputPath.Text.Trim();
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        AntdUI.Message.warn(_overview, "请先设置官服路径");
                        return;
                    }
                    var cfg = ConfigHelper.Load();
                    var bili = cfg.Games.Find(g => g.IconName == "BiliArknights");
                    if (bili != null) bili.RootPath = currentPath;
                    ConfigHelper.Save(cfg);
                    AntdUI.Message.success(_overview, "路径已同步到 BillBili服");
                };
                Controls.Add(btnSync);
                Size = new Size(360, 386);
            }

            Controls.Add(picIcon);
            Controls.Add(lblName);
            Controls.Add(divider1);
            Controls.Add(lblVersion);
            Controls.Add(lblPathSection);
            Controls.Add(_inputPath);
            Controls.Add(btnBrowse);
            Controls.Add(btnOpenDir);
            

        }

        private void AutoSave(string path)
        {
            var cfg = ConfigHelper.Load();
            var entry = cfg.Games.Find(g => g.Name == _game.Name && g.IconName == _game.IconName);
            if (entry != null) { entry.RootPath = path; ConfigHelper.Save(cfg); }
        }

        private void BrowsePath()
        {
            while (true)
            {
                Helpers.DialogHelper.InjectIcon(Properties.Resources.icon);
                using var dlg = new FolderBrowserDialog();
                dlg.Description = $"选择「{_game.Name}」游戏根目录";
                dlg.UseDescriptionForTitle = true;
                if (!string.IsNullOrEmpty(_inputPath.Text))
                    dlg.InitialDirectory = _inputPath.Text;

                var form = FindForm();
                if (dlg.ShowDialog(form) != DialogResult.OK) return;

                string selected = dlg.SelectedPath;
                bool isEndfield = _game.IconName == "Endfield" || _game.IconName == "BiliEndfield" || _game.IconName == "GlobalEndfield";
                string exeName = isEndfield ? "Endfield.exe" : "Arknights.exe";
                if (File.Exists(Path.Combine(selected, exeName)))
                {
                    _inputPath.Text = selected;
                    AutoSave(selected);
                    return;
                }
                System.Media.SystemSounds.Exclamation.Play();
                var result = AntdUI.Modal.open(new AntdUI.Modal.Config(
                    FindForm() as AntdUI.BaseForm ?? null,
                    "路径无效",
                    $"所选文件夹中未找到 {exeName}，请重新选择正确的游戏根目录。",
                    AntdUI.TType.Warn)
                {
                    OkText = "重新选择",
                    CancelText = "取消"
                });
                if (result != DialogResult.OK) return;
            }
        }

        private static System.Drawing.Icon LoadIcon(string iconName)
        {
            try
            {
                string basePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                    "Resources");
                string file = iconName switch
                {
                    "Arknights"      => "Arknights.ico",
                    "BiliArknights"  => "BiliArknights.ico",
                    "Endfield"       => "Endfield.ico",
                    "BiliEndfield"   => "BiliEndfield.ico",
                    "GlobalEndfield" => "GlobalEndfield.ico",
                    _ => null
                };
                if (file == null) return null;
                string full = Path.Combine(basePath, file);
                return File.Exists(full) ? new System.Drawing.Icon(full, new Size(256, 256)) : null;
            }
            catch { return null; }
        }
    }
}
