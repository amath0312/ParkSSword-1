﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace PWDCore
{
    public static class Functions
    {
        #region Record Setting Check

        /// <summary>
        /// CPU编码
        /// </summary>
        public static string CPUCodeStr = null;

        /// <summary>
        /// 硬盘编码
        /// </summary>
        public static string HardCodeStr = null;

        /// <summary>
        /// USB编码
        /// </summary>
        public static string USBCodeStr = null;

        /// <summary>
        /// 预定义CPU编码
        /// </summary>
        public static string PreDefine_CPUCodeStr = null;

        /// <summary>
        /// 预定义硬盘编码
        /// </summary>
        public static string PreDefine_HardCodeStr = null;

        #endregion

        #region 远端绑定数据

        /// <summary>
        /// 判定是否登录成功
        /// </summary>
        public static bool Login = false;

        /// <summary>
        /// Email账号
        /// </summary>
        public static string EmailID { get; set; }

        #endregion

        /// <summary>
        /// 用户配置文件列表
        /// </summary>
        public const string UserPWDsPath = @"\UserPWDs.xml";

        /// <summary>
        /// 设定（默认）
        /// </summary>
        public static class Setting_Default
        {
            public static int Length = 8;
            public static int MD5Times = 8;
            public static bool LockCPU = true;
            public static bool LockHard = false;
            public static bool LockUSB = false;

            public static string TimeStamp = "";//时间戳
            public static string LoginEmail = "";//登录Email

            public static string CloudyTime = "";//云同步时间
            public static int UpdateSinceLstCloudy = 0;//记录自从上次云同步以来更新的数据条目
        }

        /// <summary>
        /// 设定（手动）
        /// </summary>
        public static class Setting_Manual
        {
            public static int Length = 8;
            public static int MD5Times = 8;
            public static bool LockCPU = true;
            public static bool LockHard = false;
            public static bool LockUSB = false;
        }

        /// <summary>
        /// 手动密码结构
        /// </summary>
        public struct ManualItems
        {
            /// <summary>
            /// 域名
            /// </summary>
            public string Domin;

            /// <summary>
            /// 密码池
            /// </summary>
            public string PWDPool;

            /// <summary>
            /// 密码长度
            /// </summary>
            public int Length;

            /// <summary>
            /// MD5计算次数
            /// </summary>
            public int MD5Times;

            /// <summary>
            /// 绑定CPU
            /// </summary>
            public bool LockCPU;

            /// <summary>
            /// 绑定Hard Disk
            /// </summary>
            public bool LockHard;

            /// <summary>
            /// 绑定USB
            /// </summary>
            public bool LockUSB;
        }

        /// <summary>
        /// 用以保存手动密码详细信息
        /// </summary>
        public static List<ManualItems> ManualItemsLst = new List<ManualItems>();

        /// <summary>
        /// 加载配置信息
        /// </summary>
        public static void LoadSettingXML()
        {
            //创建新的xml文件
            if (!File.Exists(Application.StartupPath + UserPWDsPath))
            {
                SaveCodeBaseXML();
                return;
            }

            //读取xml文件
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Application.StartupPath + UserPWDsPath);

                XmlNodeList ItemList;
                XmlNodeList Lst;

                //加载配置文件
                ItemList = doc.SelectNodes("ManualPWDs/Setting");


                if (ItemList.Count == 1)
                {
                    //读取配置
                    Lst = ItemList[0].ChildNodes;

                    //获取时间戳
                    Setting_Default.TimeStamp = Lst[0].InnerText;

                    try
                    {
                        //配置文件
                        Functions.Setting_Manual.Length = Convert.ToInt16(Lst[1].InnerText);
                        Functions.Setting_Manual.MD5Times = Convert.ToInt16(Lst[2].InnerText);
                        Functions.Setting_Manual.LockCPU = Convert.ToBoolean(Lst[3].InnerText);
                        Functions.Setting_Manual.LockHard = Convert.ToBoolean(Lst[4].InnerText);
                        Functions.Setting_Manual.LockUSB = Convert.ToBoolean(Lst[5].InnerText);

                        Functions.Setting_Default.Length = Convert.ToInt16(Lst[6].InnerText);
                        Functions.Setting_Default.MD5Times = Convert.ToInt16(Lst[7].InnerText);
                        Functions.Setting_Default.LockCPU = Convert.ToBoolean(Lst[8].InnerText);
                        Functions.Setting_Default.LockHard = Convert.ToBoolean(Lst[9].InnerText);
                        Functions.Setting_Default.LockUSB = Convert.ToBoolean(Lst[10].InnerText);

                        Functions.Setting_Default.LoginEmail = Lst[11].InnerText;
                    }
                    catch
                    { 
                    }
                }
                else
                {
                    Functions.Setting_Manual.Length = 8;
                    Functions.Setting_Manual.MD5Times = 8;
                    Functions.Setting_Manual.LockCPU = true;
                    Functions.Setting_Manual.LockHard = false;
                    Functions.Setting_Manual.LockUSB = false;

                    Functions.Setting_Default.Length = 8;
                    Functions.Setting_Default.MD5Times = 8;
                    Functions.Setting_Default.LockCPU = true;
                    Functions.Setting_Default.LockHard = false;
                    Functions.Setting_Default.LockUSB = false;

                    Functions.Setting_Default.LoginEmail = "";
                }

                //加载用户清单
                ItemList = doc.SelectNodes("ManualPWDs/Unit");
                ManualItemsLst.Clear();
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (ItemList[i].HasChildNodes)
                    {
                        Lst = ItemList[i].ChildNodes;

                        //以正常模式写入Functions.ManualItemsLst列表
                        Functions.ManualItems FM = new Functions.ManualItems();
                        FM.Domin = Lst[0].InnerText;
                        FM.PWDPool = Lst[1].InnerText;
                        FM.Length = Convert.ToInt16(Lst[2].InnerText);
                        FM.MD5Times = Convert.ToInt16(Lst[3].InnerText);
                        FM.LockCPU = Convert.ToBoolean(Lst[4].InnerText);

                        FM.LockHard = Convert.ToBoolean(Lst[5].InnerText);
                        FM.LockUSB = Convert.ToBoolean(Lst[6].InnerText);

                        Functions.ManualItemsLst.Add(FM);
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + Environment.NewLine + "配置文件读取失败", "出错", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        ///  保存配置文件
        /// </summary>
        public static void SaveCodeBaseXML(bool UpdateTimeStamp = true)
        {
            try
            {
                using (XmlTextWriter lXmlWriter = new XmlTextWriter(Application.StartupPath + UserPWDsPath, Encoding.Default))
                {
                    lXmlWriter.Formatting = Formatting.Indented;
                    lXmlWriter.WriteStartDocument();//开始
                    lXmlWriter.WriteStartElement("ManualPWDs");//总根节点

                    //配置清单
                    lXmlWriter.WriteStartElement("Setting");
                    if (UpdateTimeStamp)
                    {
                        lXmlWriter.WriteElementString("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmm"));//时间戳
                    }
                    else
                    {
                        lXmlWriter.WriteElementString("TimeStamp", Setting_Default.TimeStamp);//时间戳
                    }

                    lXmlWriter.WriteElementString("Length_Manual", Functions.Setting_Manual.Length.ToString());//Length
                    lXmlWriter.WriteElementString("MD5Times_Manual", Functions.Setting_Manual.MD5Times.ToString());//MD5Times
                    lXmlWriter.WriteElementString("LockCPU_Manual", Functions.Setting_Manual.LockCPU.ToString());//LockCPU
                    lXmlWriter.WriteElementString("LockHard_Manual", Functions.Setting_Manual.LockHard.ToString());//LockHard
                    lXmlWriter.WriteElementString("LockUSB_Manual", Functions.Setting_Manual.LockUSB.ToString());//LockUSB

                    lXmlWriter.WriteElementString("Length_Default", Functions.Setting_Default.Length.ToString());//Length
                    lXmlWriter.WriteElementString("MD5Times_Default", Functions.Setting_Default.MD5Times.ToString());//MD5Times
                    lXmlWriter.WriteElementString("LockCPU_Default", Functions.Setting_Default.LockCPU.ToString());//LockCPU
                    lXmlWriter.WriteElementString("LockHard_Default", Functions.Setting_Default.LockHard.ToString());//LockHard
                    lXmlWriter.WriteElementString("LockUSB_Default", Functions.Setting_Default.LockUSB.ToString());//LockUSB

                    lXmlWriter.WriteElementString("LoginEmail", Functions.Setting_Default.LoginEmail);//登录Email

                    lXmlWriter.WriteEndElement();//节点关闭


                    if (ManualItemsLst.Count > 0)
                    {
                        //保存手动密码清单
                        for (int i = 0; i < ManualItemsLst.Count; i++)
                        {
                            lXmlWriter.WriteStartElement("Unit");//子节点
                            lXmlWriter.WriteElementString("Domin", ManualItemsLst[i].Domin);
                            lXmlWriter.WriteElementString("PWDPool", ManualItemsLst[i].PWDPool);
                            lXmlWriter.WriteElementString("Length", ManualItemsLst[i].Length.ToString());
                            lXmlWriter.WriteElementString("MD5Times", ManualItemsLst[i].MD5Times.ToString());
                            lXmlWriter.WriteElementString("LockCPU", ManualItemsLst[i].LockCPU.ToString());
                            lXmlWriter.WriteElementString("LockHard", ManualItemsLst[i].LockHard.ToString());
                            lXmlWriter.WriteElementString("LockUSB", ManualItemsLst[i].LockUSB.ToString());
                            lXmlWriter.WriteEndElement();//子节点关闭
                        }
                    }

                    lXmlWriter.WriteEndElement();//总根节点关闭
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + Environment.NewLine + "配置文件保存失败", "出错", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //CPU密钥计算状态
        public static bool CPUStrDone = false;

        /// <summary>
        /// 取得CPU信息
        /// </summary>
        public static void GetCPUKeysAndLoadXML()
        {
            //加载XML文档
            LoadSettingXML();

            //根据预定义设定密钥
            if (PreDefine_CPUCodeStr != null)
            {
                CPUCodeStr = PreDefine_CPUCodeStr;
            }
            else
            {
                //根据CPU生成密钥
                ManagementClass cimobject = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = cimobject.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    CPUCodeStr = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            CPUStrDone = true;
        }

        private static ManagementClass mc;
        private static ManagementObject disk;

        /// <summary>
        /// 取得Hard信息
        /// </summary>
        public static void GetHardKeys()
        {
            try
            {
                //预定义硬盘ID序列
                if (PreDefine_HardCodeStr != null)
                {
                    HardCodeStr = PreDefine_HardCodeStr;
                }
                else
                {
                    //计算硬盘ID序列
                    mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
                    disk.Get();
                    HardCodeStr = disk.GetPropertyValue("VolumeSerialNumber").ToString();
                }
            }
            catch
            {
                HardCodeStr = null;
            }
        }

        /// <summary>
        /// 取得USB信息
        /// </summary>
        public static void GetUSBKeys()
        {
            USBSelect US = new USBSelect();
        }

        /// <summary>
        /// 读取USB信息
        /// </summary>
        public static string ReadUSBInfo(string USBDriver)
        {
            //过滤最后的"\"符号并加上引号
            USBDriver = USBDriver.Substring(0, USBDriver.Length - 1) + @"""";

            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + USBDriver);
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        /// <summary>
        /// 加密核心算法
        /// </summary>
        /// <param name="SourceText">原文</param>
        /// <returns>密文</returns>
        public static string CreatCore(string SourceText)
        {
            try
            {
                string TempStr = SourceText;

                for (int i = 0; i < 16; i++)
                {
                    TempStr = MD5.Bit16(TempStr);
                }

                int Find = IsManualPWD(SourceText);
                if (Find != -1)
                {
                    //检测选中的设备是否包含有效值
                    if (Convert.ToBoolean(ManualItemsLst[Find].LockCPU) && CPUCodeStr == null)
                    {
                        throw new Exception("无法读取CPU序列号");
                    }

                    if (Convert.ToBoolean(ManualItemsLst[Find].LockHard) && HardCodeStr == null)
                    {
                        throw new Exception("无法读取硬盘序列号");
                    }

                    //获取USB密钥数据
                    if (Convert.ToBoolean(ManualItemsLst[Find].LockUSB) && USBCodeStr == null)
                    {
                        Functions.GetUSBKeys();
                        if (USBCodeStr == null)
                        {
                            throw new Exception("读取USB序列号失败");
                        }
                    }

                    //启用手动加密序列
                    string CPUID = Convert.ToBoolean(ManualItemsLst[Find].LockCPU) ? CPUCodeStr : null;
                    string HardID = Convert.ToBoolean(ManualItemsLst[Find].LockHard) ? HardCodeStr : null;
                    string USBID = Convert.ToBoolean(ManualItemsLst[Find].LockUSB) ? USBCodeStr : null;

                    TempStr = MD5.Manual(TempStr, ManualItemsLst[Find].PWDPool, ManualItemsLst[Find].Length, ManualItemsLst[Find].MD5Times, CPUID, HardID, USBID);
                }
                else
                {
                    if (Functions.Setting_Default.LockCPU && CPUCodeStr == null)
                    {
                        throw new Exception("无法读取CPU序列号");
                    }

                    if (Functions.Setting_Default.LockHard && HardCodeStr == null)
                    {
                        throw new Exception("无法读取硬盘序列号");
                    }

                    //获取USB密钥数据
                    if (Functions.Setting_Default.LockUSB && USBCodeStr == null)
                    {
                        Functions.GetUSBKeys();
                        if (USBCodeStr == null)
                        {
                            throw new Exception("读取USB序列号失败");
                        }
                    }

                    //默认模式计算密码
                    //混合参数箱子
                    StringBuilder MixStr = new StringBuilder(TempStr);

                    if (Functions.Setting_Default.LockCPU) MixStr.Append(CPUCodeStr);
                    if (Functions.Setting_Default.LockHard) MixStr.Append(HardCodeStr);
                    if (Functions.Setting_Default.LockUSB) MixStr.Append(USBCodeStr);

                    Console.WriteLine(Functions.Setting_Default.LockCPU + "->" + CPUCodeStr);
                    Console.WriteLine(Functions.Setting_Default.LockHard + "->" + HardCodeStr);
                    Console.WriteLine(Functions.Setting_Default.LockUSB + "->" + USBCodeStr);

                    Console.WriteLine("MixStr:" + MixStr);

                    string Temp01 = MD5.Bit32(MixStr.ToString()).Substring(0, Functions.Setting_Default.Length).ToUpper();
                    string Temp02 = MD5.Bit32(Temp01 + MixStr.ToString()).Substring(0, Functions.Setting_Default.Length).ToLower();

                    TempStr = Temp01 + ":" + Temp02;
                    Console.WriteLine("TempStr->" + TempStr);
                }

                return TempStr;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        /// <summary>
        /// 检测是否存在于手动密码列表
        /// </summary>
        /// <param name="Source">待检测字符串</param>
        /// <returns>存在于列表中的序列（-1为不存在）</returns>
        static int IsManualPWD(string Source)
        {
            bool Has = false;
            int Index = -1;

            for (int i = 0; i < ManualItemsLst.Count; i++)
            {
                if (ManualItemsLst[i].Domin.ToLower() == Source.ToLower())
                {
                    Has = true;
                    Index = i;
                    break;
                }
            }
            return Has ? Index : -1;
        }

        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="delayTime">需要延时多少秒</param>
        /// <returns></returns>
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="Url">链接</param>
        /// <param name="CharSet">编码</param>
        /// <returns></returns>
        public static string GetSource(string Url)
        {
            using (WebClient myWebClient = new WebClient())
            {
                using (Stream myStream = myWebClient.OpenRead(Url))
                {
                    using (StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 查询帐号状态
        /// </summary>
        public struct StatuStuct
        {
            public bool SignUp;
            public bool Weixin;

            public bool RefreshDone;
            public bool Error;
            public string ErrorStr;

            public void Refresh()
            {
                new Thread(new ThreadStart(Thread)).Start();
            }

            private void Thread()
            {
                try
                {
                    AccountStatu.RefreshDone = false;
                    AccountStatu.Error = false;

                    string Src = Functions.GetSource("http://parkssword.sinaapp.com/CheckStatu.php?Email=" + Functions.EmailID + "&CPUID=" + Functions.CPUCodeStr);

                    if (Src.Trim() == "-1") throw new Exception("帐号异地登录，请重新登录。或联系ParkSSword客服（QQ：2715937765）");

                    AccountStatu.SignUp = Src.Substring(0, 1) == "1" ? true : false;
                    AccountStatu.Weixin = Src.Substring(1, 1) == "1" ? true : false;
                }
                catch (Exception E)
                {
                    AccountStatu.Error = true;
                    AccountStatu.ErrorStr = E.Message;
                }
                finally
                {
                    AccountStatu.RefreshDone = true;
                }
            }
        }

        /// <summary>
        /// 帐号状态
        /// </summary>
        public static StatuStuct AccountStatu;

        /// <summary>
        /// 判断是否为Email格式
        /// </summary>
        /// <param name="Src"></param>
        /// <returns></returns>
        public static bool IsEmail(string Src)
        {
            Regex r = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return r.IsMatch(Src);
        }

        /// <summary>
        /// 提取参数ID并赋值
        /// </summary>
        /// <param name="SourceStr">源参数字符串</param>
        /// <param name="TargetStr">目标参数</param>
        /// <param name="PickStr">关键字</param>
        public static bool PickArgsStr(string SourceStr, ref string TargetStr, string PickStr)
        {
            bool Success = false;
            string TempStr = "";
            try
            {
                if (SourceStr.Substring(0, PickStr.Length).ToUpper() == PickStr.ToUpper())
                {
                    TempStr = SourceStr.Substring(PickStr.Length, SourceStr.Length - PickStr.Length);
                }
            }
            catch
            {
                TempStr = null;
            }
            if (TempStr.Trim() != "")
            {
                TargetStr = TempStr;
                Success = true;
            }

            return Success;
        }
    }
}
