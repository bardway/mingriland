// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows.Media;
#endregion

namespace TLAuto.BaseEx.Extensions
{
    public static class TransformExtension
    {
        public static Transform AddTransform(this Transform t, Transform item)
        {
            if (t == null)
            {
                t = item;
                return t;
            }

            var g = t as TransformGroup;
            if (g != null)
            {
                g.Children.Add(item);
                return g;
            }

            g = new TransformGroup();
            g.Children.Add(t);
            g.Children.Add(item);
            return g;
        }

        public static T GetTransform<T>(this Transform t) where T: Transform
        {
            var ret = t as T;
            if (ret != null)
            {
                return ret;
            }

            var g = t as TransformGroup;
            if (g == null)
            {
                return null;
            }

            foreach (var item in g.Children)
            {
                ret = item as T;
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public static int GetTransformIndex<T>(this TransformGroup t) where T: Transform
        {
            foreach (var item in t.Children)
            {
                var ret = item as T;
                if (ret != null)
                {
                    return t.Children.IndexOf(item);
                }
            }

            return -1;
        }

        public static Transform ResetTransform<T>(this Transform t, Transform item) where T: Transform
        {
            t = RemoveTransform<T>(t);
            t = AddTransform(t, item);
            return t;
        }

        public static Transform RemoveTransform<T>(this Transform t) where T: Transform
        {
            var ret = t as T;
            if (ret != null)
            {
                return null;
            }

            var g = t as TransformGroup;
            if (g == null)
            {
                return t;
            }

            var removeList = new List<Transform>();
            foreach (var item in g.Children)
            {
                ret = item as T;
                if (ret != null)
                {
                    removeList.Add(item);
                }
            }
            removeList.ForEach(i => g.Children.Remove(i));
            return g;
        }

        public static string TransformToString(this Transform t)
        {
            var ret = "";
            TransformToString(t, ref ret);
            return ret;
        }

        private static void TransformToString(Transform t, ref string result)
        {
            var translateT = t as TranslateTransform;
            if (translateT != null)
            {
                result += string.Format("TranslateTransform, X:{0}, Y:{1}\r\n", translateT.X, translateT.Y);
                return;
            }

            var scaleT = t as ScaleTransform;
            if (scaleT != null)
            {
                result += string.Format("ScaleTransform, CenterX:{0}, CenterY:{1}, ScaleX:{2}, ScaleY:{3}\r\n", scaleT.CenterX, scaleT.CenterY, scaleT.ScaleX, scaleT.ScaleY);
                return;
            }

            var g = t as TransformGroup;
            if (g != null)
            {
                result += string.Format("TransformGroup, Count:{0}\r\n", g.Children.Count);
                foreach (var item in g.Children)
                {
                    TransformToString(item, ref result);
                }
            }
        }
    }
}