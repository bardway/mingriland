// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Themes
{
    public static class ThemeManager
    {
        private static ResourceDictionary _currentTheme;

        public static void ApplyTheme(this Application app, ThemeTypes theme)
        {
            var dictionary = GetThemeResourceDictionary(theme);
            if (dictionary != null)
            {
                if (_currentTheme != null)
                {
                    app.Resources.MergedDictionaries.Remove(_currentTheme);
                }
                _currentTheme = dictionary;
                app.Resources.MergedDictionaries.Add(dictionary);
            }
        }

        public static void ApplyTheme(this ContentControl control, ThemeTypes theme)
        {
            var dictionary = GetThemeResourceDictionary(theme);
            if (dictionary != null)
            {
                control.Resources.MergedDictionaries.Clear();
                control.Resources.MergedDictionaries.Add(dictionary);
            }
        }

        private static ResourceDictionary GetThemeResourceDictionary(ThemeTypes theme)
        {
            var packUri = theme.GetEnumAttribute<DescriptionAttribute>().Description;
            return Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
        }
    }
}