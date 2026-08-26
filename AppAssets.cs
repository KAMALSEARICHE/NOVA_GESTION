using System;
using System.Drawing;
using System.IO;

namespace NovaGestion.UI
{
    /// <summary>
    /// يحمّل شعار وأيقونة NovaGestion الحقيقيين (فولدر Resources) مرة وحدة
    /// ويوفّرهم لكل الفورمات. إذا الملفات ماكانوش موجودين (مثلاً بعد نسخ
    /// المشروع بلا فولدر Resources)، يرجع null بلا ما يوقف البرنامج.
    /// </summary>
    public static class AppAssets
    {
        private static Icon? _appIcon;
        private static Image? _logoFull;
        private static bool _iconTried, _logoTried;

        private static string ResourcesDir => Path.Combine(AppContext.BaseDirectory, "Resources");

        /// <summary>الأيقونة الدائرية الذهبية (تُستعمل فـ Form.Icon وكـ Logo مصغّر).</summary>
        public static Icon? AppIcon
        {
            get
            {
                if (!_iconTried)
                {
                    _iconTried = true;
                    try
                    {
                        string path = Path.Combine(ResourcesDir, "app.ico");
                        if (File.Exists(path)) _appIcon = new Icon(path);
                    }
                    catch { _appIcon = null; }
                }
                return _appIcon;
            }
        }

        /// <summary>الشعار الأفقي الكامل "NOVA GESTION" (يُستعمل فـ شاشة الدخول).</summary>
        public static Image? LogoFull
        {
            get
            {
                if (!_logoTried)
                {
                    _logoTried = true;
                    try
                    {
                        string path = Path.Combine(ResourcesDir, "logo_nova.png");
                        if (File.Exists(path)) _logoFull = Image.FromFile(path);
                    }
                    catch { _logoFull = null; }
                }
                return _logoFull;
            }
        }

        /// <summary>ينسخ الأيقونة لأي فورم (يُستدعى فـ بداية BuildUI/BuildShell).</summary>
        public static void ApplyIcon(System.Windows.Forms.Form form)
        {
            if (AppIcon != null) form.Icon = AppIcon;
        }
    }
}
