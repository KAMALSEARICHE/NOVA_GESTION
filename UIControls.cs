using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NovaGestion.UI
{
    /// <summary>
    /// الألوان والخطوط الموحّدة لكامل التطبيق NovaGestion — مطابقة لألوان العلم الجزائري
    /// (أخضر جزائري أساسي، أبيض للخلفيات، أحمر جزائري للتنبيهات والعناصر البارزة).
    /// </summary>
    public static class Theme
    {
        // الأخضر الجزائري (اللون الأساسي: Header, Sidebar, أزرار رئيسية)
        public static readonly Color Maroon = Color.FromArgb(0, 98, 51);
        public static readonly Color MaroonDark = Color.FromArgb(0, 71, 37);
        // الأحمر الجزائري (لون بارز: أزرار التمييز/الإجراءات المهمة)
        public static readonly Color MaroonAccent = Color.FromArgb(210, 16, 52);

        public static readonly Color PageBg = Color.FromArgb(242, 242, 244);
        public static readonly Color CardBg = Color.White;
        public static readonly Color BorderGray = Color.FromArgb(224, 224, 228);

        public static readonly Color TextDark = Color.FromArgb(33, 37, 41);
        public static readonly Color TextGray = Color.FromArgb(120, 124, 130);
        public static readonly Color TextMuted = Color.FromArgb(160, 163, 168);

        public static readonly Color BtnLightBg = Color.FromArgb(238, 238, 240);
        public static readonly Color Green = Color.FromArgb(0, 98, 51);
        public static readonly Color Orange = Color.FromArgb(217, 140, 20);
        public static readonly Color Red = Color.FromArgb(210, 16, 52);
        public static readonly Color Navy = Color.FromArgb(30, 41, 59);
        public static readonly Color SidebarBg = Color.FromArgb(0, 71, 37);
        public static readonly Color SidebarHover = Color.FromArgb(0, 98, 51);
        public static readonly Color SidebarActive = Color.FromArgb(210, 16, 52);

        public static Font FontTitle => new Font("Segoe UI", 16F, FontStyle.Bold);
        public static Font FontSectionTitle => new Font("Segoe UI", 11.5F, FontStyle.Bold);
        public static Font FontLabel => new Font("Segoe UI", 9F);
        public static Font FontValue => new Font("Segoe UI", 9.5F);
        public static Font FontBold => new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static Font FontButton => new Font("Segoe UI", 9.5F, FontStyle.Bold);
    }

    /// <summary>
    /// Panel أبيض بحواف دائرية يُستعمل كـ "Card" (بطاقة) في كل شاشات التطبيق.
    /// </summary>
    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 10;
        public Color BorderColor { get; set; } = Theme.BorderGray;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Theme.CardBg;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = RoundedRect(rect, CornerRadius);
            using SolidBrush brush = new SolidBrush(BackColor);
            using Pen pen = new Pen(BorderColor, 1);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(pen, path);
            base.OnPaint(e);
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            if (d <= 0 || bounds.Width <= d || bounds.Height <= d)
            {
                path.AddRectangle(bounds);
                return path;
            }
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// زر بحواف دائرية بدون حدود، يشبه أزرار التصميم الحديث (Flat + Rounded).
    /// </summary>
    public class RoundedButton : Button
    {
        public int CornerRadius { get; set; } = 8;
        private bool _hover;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            MouseEnter += (s, e) => { _hover = true; Invalidate(); };
            MouseLeave += (s, e) => { _hover = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = RoundedPanel.RoundedRect(ClientRectangle, CornerRadius);
            Color fill = _hover ? ControlPaint.Dark(BackColor, 0.05f) : BackColor;
            using (SolidBrush brush = new SolidBrush(fill))
            {
                e.Graphics.FillPath(brush, path);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    /// <summary>
    /// Panel بتدرّج لوني (Gradient) يُستعمل فالهيدرز باش يعطي عمق بصري عصري
    /// عوض اللون المسطّح.
    /// </summary>
    public class GradientPanel : Panel
    {
        public Color ColorStart { get; set; } = Theme.Maroon;
        public Color ColorEnd { get; set; } = Theme.MaroonDark;
        public float Angle { get; set; } = 100f;

        public GradientPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                using LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, Width, Height), ColorStart, ColorEnd, Angle);
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
            base.OnPaint(e);
        }
    }

    /// <summary>
    /// شعار (Logo) مربّع بحواف دائرية وتدرّج أخضر↔أحمر (ألوان العلم الجزائري)
    /// يحمل الحرف الأول من اسم التطبيق — يُستعمل غير كـ Fallback إذا ملفات
    /// اللوغو الحقيقية (Resources) ماكانوش موجودين.
    /// BackColor حقيقي إجباري (نفس سبب NumberBadge) باش الحواف تبان نظيفة.
    /// </summary>
    public class LogoBadge : Panel
    {
        public string Letter { get; set; } = "N";
        public int CornerRadius { get; set; } = 10;

        public LogoBadge()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(38, 38);
            BackColor = Theme.Maroon;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = RoundedPanel.RoundedRect(rect, CornerRadius);
            using LinearGradientBrush brush = new LinearGradientBrush(rect, Theme.Maroon, Theme.MaroonAccent, 45f);
            e.Graphics.FillPath(brush, path);
            TextRenderer.DrawText(e.Graphics, Letter, new Font("Segoe UI", (float)(Height * 0.45), FontStyle.Bold),
                ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            base.OnPaint(e);
        }
    }

    /// <summary>
    /// دائرة صغيرة ملوّنة تحمل رقم القسم (تستعمل في عناوين البطاقات مثل "1  Références").
    /// ملاحظة: نستعمل BackColor حقيقي (ماشي Transparent) ونمليه أول شي فـ OnPaint
    /// لأن الشفافية الوهمية فـ WinForms ما تخدمش مزيان فوق Controls مرسومة بالكود
    /// (RoundedPanel/GradientPanel) — كانت تسبب حواف مشوّهة حول الدائرة.
    /// </summary>
    public class NumberBadge : Panel
    {
        public string Number { get; set; } = "1";

        public NumberBadge()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(24, 24);
            BackColor = Theme.CardBg;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using SolidBrush brush = new SolidBrush(Theme.Maroon);
            e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            TextRenderer.DrawText(e.Graphics, Number, new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
