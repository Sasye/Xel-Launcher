using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using XelLauncher.Helpers;
using XelLauncher.Forms;

namespace XelLauncher
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arge)
        {
            Microsoft.Web.WebView2.Core.CoreWebView2Environment.SetLoaderDllFolderPath("");
            // 捕获 UI 线程未处理异常
            Application.ThreadException += (s, e) =>
                Helpers.LogHelper.LogError(e.Exception, "UI ThreadException");

            // 捕获非 UI 线程未处理异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    Helpers.LogHelper.LogError(ex, "UnhandledException");
            };

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#if !NET10_0
            //ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);
#endif
            var command = string.Join(" ", arge);
            AntdUI.Localization.DefaultLanguage = "zh-CN";
            var cfg = ConfigHelper.Load();
            // 优先使用用户保存的语言，否则跟随系统语言
            string lang = !string.IsNullOrEmpty(cfg.Language)
                ? cfg.Language
                : AntdUI.Localization.CurrentLanguage;
            if (lang.StartsWith("en")) AntdUI.Localization.Provider = new Localizer();
            AntdUI.Localization.SetLanguage(lang);
            AntdUI.Config.Theme().Dark("#000", "#fff").Light("#fff", "#000").FormBorderColor();
            // 根据用户保存的主题模式决定深色/浅色，"system" 则跟随系统注册表
            AntdUI.Config.IsDark = cfg.ThemeMode switch
            {
                "dark"  => true,
                "light" => false,
                _       => IsSystemDarkMode()   // "system" 或其他旧值
            };
            AntdUI.Config.TextRenderingHighQuality = true;
            AntdUI.Config.ShowInWindow = true;
            AntdUI.Config.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            AntdUI.Config.SetEmptyImageSvg(Properties.Resources.icon_empty, Properties.Resources.icon_empty_dark);
            AntdUI.SvgDb.Emoji = AntdUI.FluentFlat.Emoji;
            if (!string.IsNullOrEmpty(cfg.PrimaryColor))
                AntdUI.Style.SetPrimary(System.Drawing.ColorTranslator.FromHtml(cfg.PrimaryColor));
            if (command == "m") Application.Run(new Main());
            else if (command == "tab") Application.Run(new TabHeaderForm());
            else Application.Run(new Overview(command == "t"));
        }

        /// <summary>
        /// 读取注册表判断系统是否处于深色模式（AppsUseLightTheme=0 即深色）
        /// </summary>
        static bool IsSystemDarkMode()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
                var val = key?.GetValue("AppsUseLightTheme");
                return val is int i && i == 0;
            }
            catch { return false; }
        }
    }
}