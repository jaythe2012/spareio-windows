using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CoreLib
{
    public class XmlHelper
    {
        private static string _xmlFilePath;
        private static System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
        private static Object _thisLock = new Object();

        /// <summary>
        /// On initializing.. adding/updating init time value
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="initTime"></param>
        public static void Initialize(string fileName, string initTime)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    Debug.Assert(false, "fileName is not defined");
                    fileName = Assembly.GetEntryAssembly().GetName().Name + ".log";
                }
                _xmlFilePath = Path.Combine(PathConstants.GetConfigPath(), fileName + ".xml");

                if (!File.Exists(_xmlFilePath))
                {
                    using (XmlWriter writer = XmlWriter.Create(_xmlFilePath))
                    {
                        writer.WriteStartElement("settings");
                        writer.WriteElementString("initTime", initTime);
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while initializing settings file " + ex.Message);

            }
        }

        public static void LoadInMemory()
        {
            xmlDoc.Load(_xmlFilePath);
        }

        public static void UpdateSetting(string key, string value)
        {
            try
            {
                //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                //xmlDoc.Load(_xmlFilePath);
                lock(_thisLock)
                { 
                    XmlNode node = xmlDoc.SelectSingleNode("/settings/"+key);
                    if (node != null)
                    {
                        node.InnerText = value;
                        //xmlDoc.Save(_xmlFilePath);
                    }
                    else
                    {
                        XmlElement nodetobeInserted = xmlDoc.CreateElement(key);
                        nodetobeInserted.InnerText = value;
                        if(xmlDoc.DocumentElement != null)
                            xmlDoc.DocumentElement.AppendChild(nodetobeInserted);
                        //xmlDoc.Save(_xmlFilePath);
                    }
                    xmlDoc.Save(_xmlFilePath);

                }
            }
            catch (Exception ex)
            {
                //LogWriter.Error("Error while updating settings "+ex.Message + "For key "+key);
            }
        }

        public static void UpdateSettingInBulk(Dictionary<string,string> keyValDictionary)
        {
            try
            {
                lock (_thisLock)
                {
                    foreach (var keyVal in keyValDictionary)
                    {


                        XmlNode node = xmlDoc.SelectSingleNode("/settings/" + keyVal.Key);
                        if (node != null)
                        {
                            node.InnerText = keyVal.Value;
                            //xmlDoc.Save(_xmlFilePath);
                        }
                        else
                        {
                            XmlElement nodetobeInserted = xmlDoc.CreateElement(keyVal.Key);
                            nodetobeInserted.InnerText = keyVal.Value;
                            if (xmlDoc.DocumentElement != null)
                                xmlDoc.DocumentElement.AppendChild(nodetobeInserted);
                            //xmlDoc.Save(_xmlFilePath);
                        }
                    }
                    xmlDoc.Save(_xmlFilePath);

                }
            }
            catch (Exception ex)
            {
                //LogWriter.Error("Error while updating settings in bulk " + ex.Message + "For key " + keyValDictionary.ToString());
            }
        }

        public static string ReadSetting(string key)
        {
            string result = String.Empty;
            
            try
            {
                //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                //xmlDoc.Load(_xmlFilePath);
                lock (_thisLock)
                {
                    XmlNode node = xmlDoc.SelectSingleNode("/settings/" + key);
                    if (node != null)
                    {
                        result = node.InnerText;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogWriter.Error("Error while reading settings " + ex.Message  + "For key " + key);
                return result;
            }
        }












    }
}
