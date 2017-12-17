// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#endregion

namespace TLAuto.Base.Extensions
{
    /// <summary>
    /// XML扩展帮助类
    /// </summary>
    public static class XmlExtensions
    {
        #region 序列化与反序列化
        /// <summary>
        /// 将XML字符串转为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TObject ToObjectFromXml<TObject>(this string str) where TObject: class
        {
            var temp = Encoding.UTF8.GetBytes(str);
            using (var mstream = new MemoryStream(temp))
            {
                var serializer = new XmlSerializer(typeof(TObject));
                return serializer.Deserialize(mstream) as TObject;
            }
        }

        /// <summary>
        /// 将XML文件内容字符串转为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TObject ToObjectFromXmlFile<TObject>(this string filePath) where TObject: class
        {
            using (var mstream = new StreamReader(filePath, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(TObject));
                return serializer.Deserialize(mstream) as TObject;
            }
        }

        public static object ToObjectFromXmlFile(this string filePath, Type t)
        {
            using (var mstream = new StreamReader(filePath, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(t);
                return serializer.Deserialize(mstream);
            }
        }

        /// <summary>
        /// 将XML字符串转为对象(struct)
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TObject ToObjectAsStructFromXml<TObject>(this string str) where TObject: struct
        {
            var temp = Encoding.UTF8.GetBytes(str);
            using (var mstream = new MemoryStream(temp))
            {
                var serializer = new XmlSerializer(typeof(TObject));
                return (TObject)serializer.Deserialize(mstream);
            }
        }

        /// <summary>
        /// 将对象转为XML字符串
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml<TObject>(this TObject obj)
        {
            using (var mstream = new MemoryStream())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(typeof(TObject));
                serializer.Serialize(mstream, obj, ns);
                return Encoding.UTF8.GetString(mstream.ToArray());
            }
        }

        /// <summary>
        /// 将对象转为XML字符串
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml<TObject>(this TObject obj, Encoding encoding)
        {
            var settings = new XmlWriterSettings {Encoding = encoding, Indent = true};
            var mstream = new MemoryStream();
            using (var writer = XmlWriter.Create(mstream, settings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(typeof(TObject));
                serializer.Serialize(writer, obj, ns);
                return Encoding.UTF8.GetString(mstream.ToArray());
            }
        }

        /// <summary>
        /// 将对象转为XML字符串并存入XML文件中
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void ToXmlFile<TObject>(this TObject obj, string filePath)
        {
            using (var fileStream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(typeof(TObject));
                serializer.Serialize(fileStream, obj, ns);
            }
        }

        public static void ToXmlFile(this object obj, Type type, string filePath)
        {
            using (var fileStream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(type);
                serializer.Serialize(fileStream, obj, ns);
            }
        }
        #endregion

        #region XML文件节点分析法集合
        /// <summary>
        /// 获取一个XmlNode节点，如果该节点未空则抛出异常。
        /// </summary>
        /// <param name="xnNode">被查询的节点</param>
        /// <param name="sXPath">查询XPath</param>
        /// <returns>查询结果</returns>
        public static XmlNode GetXmlNode(XmlNode xnNode, string sXPath)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlNode() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                var sError = string.Format("Execute XPath failed, no XML node was found! /r/nXPath:({0})/r/nXML:({0})", sXPath);
                throw new Exception(sError);
            }

            return xn;
        }

        /// <summary>
        /// 获取一个XML节点的Value，如果该节点不存在则返回默认值
        /// </summary>
        /// <param name="xnNode">被查询的节点</param>
        /// <param name="sXPath">查询的XPath</param>
        /// <param name="sDefault">默认值</param>
        /// <returns>查询的结果</returns>
        public static string GetXmlValue(XmlNode xnNode, string sXPath, string sDefault)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlValue() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                return sDefault;
            }

            return xn.Value;
        }

        /// <summary>
        /// 获取一个XML节点的InnerText，如果该节点不存在则返回默认值
        /// </summary>
        /// <param name="xnNode">被查询的节点</param>
        /// <param name="sXPath">查询的XPath</param>
        /// <param name="sDefault">默认值</param>
        /// <returns>查询的结果</returns>
        public static string GetXmlText(XmlNode xnNode, string sXPath, string sDefault)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlText() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                return sDefault;
            }

            return xn.InnerText;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string path, string node, string attribute)
        {
            var value = "";
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);

                var xn = doc.SelectSingleNode(node);
                value = attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value;
                //if (attribute.Equals(""))
                //{
                //    value = xn.InnerText;
                //}
            }
            catch (Exception) { }
            return value;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
                var count = doc.SelectNodes(node).Count;
                var xn = doc.SelectNodes(node).Item(count - 1);

                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        var xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    var xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                    {
                        xe.InnerText = value;
                    }
                    else
                    {
                        xe.SetAttribute(attribute, value);
                    }
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        public static void Insert(string path, string node, string element, string attribute, string value, string inner)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
                var count = doc.SelectNodes(node).Count;
                var xn = doc.SelectNodes(node).Item(count - 1);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        var xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                        xe.InnerText = inner;
                    }
                }
                else
                {
                    var xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                    {
                        xe.InnerText = value;
                    }
                    else
                    {
                        xe.SetAttribute(attribute, value);
                        xe.InnerText = inner;
                    }
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
                var xn = doc.SelectSingleNode(node);
                var xe = (XmlElement)xn;
                if (attribute.Equals(""))
                {
                    xe.InnerText = value;
                }
                else
                {
                    xe.SetAttribute(attribute, value);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Delete(path, "/Node", "")
         * XmlHelper.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);
                var xn = doc.SelectSingleNode(node);
                var xe = (XmlElement)xn;
                if (attribute.Equals(""))
                {
                    xn.ParentNode.RemoveChild(xn);
                }
                else
                {
                    xe.RemoveAttribute(attribute);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool LoadXml(this string fullPath, out XmlDocument doc)
        {
            doc = null;
            FileStream fs = null;
            try
            {
                string xmlText;
                fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    fs = null;
                    xmlText = sr.ReadToEnd();
                }

                if (xmlText.IsNullOrEmpty())
                {
                    return false;
                }
                doc = new XmlDocument();
                doc.LoadXml(xmlText);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("LoadXml FullPath error:" + fullPath, ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
        }
        #endregion
    }
}