using System;
using System.Drawing;
using System.Windows.Forms;

namespace DachaMS
{
    public static class AppBranding
    {
        public const string AppName = "DachaMS";
        public const string AppSubtitle = "система управления дачным массивом";
        public const string AppEmoji = "🏡";

        public static readonly Color SidebarColor = Color.FromArgb(45, 45, 48);

        /// <summary>Фон 1 — белый: окно логина, панели участков, владельцев и т.д.</summary>
        public static readonly Color ContentBackgroundColor = Color.White;

        /// <summary>Фон 2 — светло-серый: шапка и область вокруг панелей на формах.</summary>
        public static readonly Color SurfaceBackgroundColor = Color.FromArgb(240, 242, 245);

        public static readonly Color MutedTextColor = Color.FromArgb(100, 100, 100);
        public static readonly Color ErrorColor = Color.FromArgb(192, 57, 43);
        public static readonly Color SuccessColor = Color.FromArgb(39, 174, 96);

        private static Icon appIcon;

        public static Icon AppIcon
        {
            get
            {
                if (appIcon == null)
                    appIcon = LoadAppIcon();
                return appIcon;
            }
        }

        public static void ApplyFormIcon(Form form)
        {
            if (AppIcon == null)
                return;

            form.Icon = (Icon)AppIcon.Clone();
        }

        public static void ApplySurfaceBackground(Control control)
        {
            control.BackColor = SurfaceBackgroundColor;
        }

        public static void ApplyContentBackground(Control control)
        {
            control.BackColor = ContentBackgroundColor;
        }

        private static Icon LoadAppIcon()
        {
            Icon extracted = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (extracted != null)
                return extracted;

            return SystemIcons.Application;
        }
    }
}
