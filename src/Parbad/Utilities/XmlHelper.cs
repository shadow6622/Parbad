﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Parbad.Utilities
{
    internal static class XmlHelper
    {
        public static string GetNodeValueFromXml(string xml, string nodeName, string nameSpace = "")
        {
            if (xml.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(xml));
            }

            var document = new XmlDocument();

            document.LoadXml(xml);

            if (string.IsNullOrEmpty(nameSpace))
            {
                return document.SelectSingleNode($"//{nodeName}")?.InnerXml ?? string.Empty;
            }

            var xmlNameSpaceManager = new XmlNamespaceManager(document.NameTable);

            xmlNameSpaceManager.AddNamespace("TheNameSpace", nameSpace);

            return document.SelectSingleNode($"//TheNameSpace:{nodeName}", xmlNameSpaceManager)?.InnerXml ?? string.Empty;
        }

        public static byte[] Serialize(object obj)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            var xmlSerializer = new XmlSerializer(obj.GetType());

            byte[] contents;

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    xmlSerializer.Serialize(xmlWriter, obj);
                }

                contents = memoryStream.ToArray();
            }

            return contents;
        }

        public static bool TryDeserialize<T>(Stream stream, out T obj)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                using (var xmlReader = XmlReader.Create(stream))
                {
                    obj = (T)xmlSerializer.Deserialize(xmlReader);
                    return true;
                }
            }
            catch
            {
                obj = default(T);
                return false;
            }
        }
    }
}