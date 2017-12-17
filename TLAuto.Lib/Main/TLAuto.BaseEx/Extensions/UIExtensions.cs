// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endregion

namespace TLAuto.BaseEx.Extensions
{
    public static class UIExtensions
    {
        /// <summary>
        /// ，
        /// </summary>
        /// <typeparam name="TParentItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TParentItem FindVisualParent<TParentItem>(this DependencyObject obj) where TParentItem: DependencyObject
        {
            if (obj != null)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                return parent as TParentItem ?? FindVisualParent<TParentItem>(parent);
            }
            return null;
        }

        public static TParentItem FindVisualParentWithoutElementName<TParentItem>(this DependencyObject obj, string withoutElementName) where TParentItem: DependencyObject
        {
            if (obj != null)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                var frameworkElement = parent as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (frameworkElement.Name == withoutElementName)
                    {
                        return null;
                    }
                }
                return parent as TParentItem ?? FindVisualParent<TParentItem>(parent);
            }
            return null;
        }

        /// <summary>
        /// Find Visual Parent UI
        /// </summary>
        /// <param name="obj">Source</param>
        /// <param name="parentType">UI Type</param>
        /// <returns></returns>
        public static DependencyObject FindVisualParent(this DependencyObject obj, Type parentType)
        {
            if (obj != null)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                if (parent != null)
                {
                    return parent.GetType() == parentType ? parent : FindVisualParent(parent, parentType);
                }
            }
            return null;
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static FrameworkElement FindVisualParent(this DependencyObject obj, string elementName)
        {
            if (obj != null)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                if (parent is FrameworkElement)
                {
                    var source = (FrameworkElement)parent;
                    return source.Name == elementName ? source : source.FindVisualParent(elementName);
                }
            }
            return null;
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <typeparam name="TChildItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TChildItem FindVisualChild<TChildItem>(this DependencyObject obj) where TChildItem: DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if ((child != null) && child is TChildItem)
                {
                    return (TChildItem)child;
                }
                var childOfChild = FindVisualChild<TChildItem>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        public static bool IsFindVisualChild(this DependencyObject obj, object source)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child == source)
                {
                    return true;
                }
                if (child.IsFindVisualChild(source))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<T> GetVisualChildCollection<T>(this DependencyObject obj) where T: DependencyObject
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(obj, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T: DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else
                {
                    if (child != null)
                    {
                        GetVisualChildCollection(child, visualCollection);
                    }
                }
            }
        }

        /// <summary>
        /// UI，
        /// </summary>
        /// <typeparam name="TParentItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TParentItem FindLogicalParent<TParentItem>(this DependencyObject obj) where TParentItem: DependencyObject
        {
            if (obj != null)
            {
                var parent = LogicalTreeHelper.GetParent(obj);
                return parent as TParentItem ?? FindLogicalParent<TParentItem>(parent);
            }
            return null;
        }

        /// <summary>
        /// Find Logical Parent UI
        /// </summary>
        /// <param name="obj">Source</param>
        /// <param name="elementName">UI Name</param>
        /// <returns></returns>
        public static FrameworkElement FindLogicalParent(this DependencyObject obj, string elementName)
        {
            if (obj != null)
            {
                var parent = LogicalTreeHelper.GetParent(obj);
                var source = parent as FrameworkElement;
                if (source != null)
                {
                    return source.Name == elementName ? source : source.FindLogicalParent(elementName);
                }
            }
            return null;
        }

        /// <summary>
        /// Find Logical Parent UI
        /// </summary>
        /// <param name="obj">Source</param>
        /// <param name="parentType">UI Type</param>
        /// <returns></returns>
        public static DependencyObject FindLogicalParent(this DependencyObject obj, Type parentType)
        {
            if (obj != null)
            {
                var parent = LogicalTreeHelper.GetParent(obj);
                if (parent != null)
                {
                    return parent.GetType() == parentType ? parent : FindLogicalParent(parent, parentType);
                }
            }
            return null;
        }

        /// <summary>
        /// UI，
        /// </summary>
        /// <typeparam name="TChildItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TChildItem FindLogicalChild<TChildItem>(this DependencyObject obj) where TChildItem: DependencyObject
        {
            var childItems = LogicalTreeHelper.GetChildren(obj);
            foreach (DependencyObject childItem in childItems)
            {
                if (childItem is TChildItem)
                {
                    return (TChildItem)childItem;
                }
                var childOfChild = FindLogicalChild<TChildItem>(childItem);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// UI，
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static FrameworkElement FindLogicalChild(this DependencyObject obj, string elementName)
        {
            if (obj != null)
            {
                var childItems = LogicalTreeHelper.GetChildren(obj);
                foreach (var childItem in childItems)
                {
                    if (childItem is FrameworkElement)
                    {
                        var source = (FrameworkElement)childItem;
                        return source.Name == elementName ? source : source.FindLogicalChild(elementName);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static VisualStateGroup GetVisualStateGroup(this FrameworkElement source, string groupName)
        {
            var visualStateGroups = VisualStateManager.GetVisualStateGroups(source);
            if (visualStateGroups != null)
            {
                foreach (VisualStateGroup visualStateGroup in visualStateGroups)
                {
                    if (visualStateGroup.Name == groupName)
                    {
                        return visualStateGroup;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="adorner"></param>
        /// <returns></returns>
        public static bool AddAdorner(this UIElement element, Adorner adorner)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null)
            {
                adornerLayer.Add(adorner);
                return true;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="adorner"></param>
        /// <returns></returns>
        public static bool AddCurrentWindowAdorner(this UIElement element, Adorner adorner)
        {
            var window = Window.GetWindow(element);
            if ((window != null) && window.Content is UIElement)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(window.Content as UIElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Add(adorner);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T GetAdorner<T>(this UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(element);
                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        if (adorner is T)
                        {
                            return (T)(object)adorner;
                        }
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T GetCurrentWindowAdorner<T>(this UIElement element)
        {
            var window = Window.GetWindow(element);
            if ((window != null) && window.Content is UIElement)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(window.Content as UIElement);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(element);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                        {
                            if (adorner is T)
                            {
                                return (T)(object)adorner;
                            }
                        }
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool RemoveAdorner<T>(this UIElement element, out T obj)
        {
            obj = default(T);
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(element);
                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        if (adorner is T)
                        {
                            obj = (T)(object)adorner;
                            adornerLayer.Remove(adorner);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool RemoveAdorner<T>(this UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(element);
                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        if (adorner is T)
                        {
                            adornerLayer.Remove(adorner);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool RemoveCurrentWindowAdorner<T>(this UIElement element)
        {
            var window = Window.GetWindow(element);
            if ((window != null) && window.Content is UIElement)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(window.Content as UIElement);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(element);
                    if (adorners != null)
                    {
                        foreach (var adorner in adorners)
                        {
                            if (adorner is T)
                            {
                                adornerLayer.Remove(adorner);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapSource CreateElementScreenshot(this Visual visual, int width, int height)
        {
            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            bmp.Render(visual);
            return bmp;
        }

        /// <summary>
        /// Image
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image CreateElementScreen(this Visual visual, int width, int height)
        {
            return new Image {Source = CreateElementScreenshot(visual, width, height)};
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TProperty"></typeparam>
        ///// <param name="propertyChangedBase"></param>
        ///// <param name="expression"></param>
        //public static void RaisePropertyChanged<T, TProperty>(this T propertyChangedBase, Expression<Func<T, TProperty>> expression) where T : PropertyChangedBase
        //{
        //    var memberExpression = expression.Body as MemberExpression;
        //    if (memberExpression != null)
        //    {
        //        string propertyName = memberExpression.Member.Name;
        //        propertyChangedBase.NotifyPropertyChanged(propertyName);
        //    }
        //    else
        //        throw new ArgumentNullException("memberExpression is null.");
        //}
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentNullException("memberExpression is null.");
        }

        public static string GetXamlUrl(Type type)
        {
            var sb = new StringBuilder();
            var stackTrace = new StackTrace(true);
            var mb = stackTrace.GetFrame(1).GetMethod();
            if (mb.DeclaringType != null)
            {
                var moduleScopeName = mb.DeclaringType.Module.ScopeName.Replace(".dll", "").Replace(".exe", "");
                sb.Append(moduleScopeName);
                sb.Append(";component/");
                if (type.Namespace != null)
                {
                    var classScopeNames = type.Namespace.Replace(moduleScopeName + ".", "").Split('.');
                    foreach (var classScopeName in classScopeNames)
                    {
                        sb.Append(classScopeName);
                        sb.Append("/");
                    }
                }
                sb.Append(type.Name);
                sb.Append(".xaml");
            }
            return sb.ToString();
        }
    }
}