using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PWDCore
{
    public class MD5
    {
        static int[] RandNum = new int[100] { 
            12,34,56,78,23,45,67,89,13,24, 
            35,46,57,68,79,80,14,25,36,47, 
            58,69,70,98,87,76,65,54,43,32,
            11,22,33,44,55,66,77,88,99,0,
            17,28,39,40,51,62,73,84,95,7,
            74,83,48,96,36,26,96,25,85,1,
            97,43,98,37,96,13,63,74,84,51,
            85,25,96,24,87,24,98,23,64,73,
            18,02,48,93,93,17,39,45,9,2,
            95,24,12,16,19,24,84,45,29,20,
        };

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public static string Bit32(string SourceString)
        {
            byte[] result = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(SourceString));
            return BitConverter.ToString(result).Replace("-", "");
        }

        public static string Bit16(string SourceString)
        {
            byte[] result = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(SourceString));
            return BitConverter.ToString(result, 4, 8).Replace("-", "");
        }

        public static string PasswordFormat(string SourcePassword)
        {
            string Temp = SourcePassword;

            for (int i = 0; i < 20; i++)
            {
                Temp = Bit32(Temp);
            }

            return Temp;
        }

        public static string Manual(string SourceString, string PWDsPool, int Length, int MD5Times, string CPUID, string HardID, string USBID)
        {
            //乱序
            PWDsPool = FlyTheWords(PWDsPool);

            StringBuilder CreatStr = new StringBuilder();

            Console.WriteLine("Feed-1.1:"+SourceString + ":" + CPUID + ":" + HardID + ":" + USBID);
            //把序列号参数进行加密运算
            string MD5Temp = Bit16(SourceString + CPUID);
            MD5Temp = Bit16(MD5Temp + HardID);
            MD5Temp = Bit16(MD5Temp + USBID);

            for (int i = 0; i < MD5Times; i++)
            {
                MD5Temp = Bit16(MD5Temp);
            }

            int LastGet = -1;//记录上一次提取的，比对，避免重复
            int PlusTwoEveryTime = 1;//每次加2，用以打乱生成

            for (int i = 0; i < Length + 16; )
            {
                MD5Temp = Bit16(MD5Temp);
                PlusTwoEveryTime += 2;

                int FinalNum = HasNumber(MD5Temp, PlusTwoEveryTime);

                if (FinalNum != -1 && LastGet != Convert.ToInt16(MD5Temp[FinalNum]))
                {
                    CreatStr.Append(PWDsPool[Convert.ToInt16(MD5Temp[FinalNum].ToString())]);

                    LastGet = Convert.ToInt16(MD5Temp[FinalNum]);
                    i++;
                }
            }

            StringBuilder ReadyStr = new StringBuilder();

            if (HasNumber(CreatStr.ToString(), 0) != -1) ReadyStr.Append(PWDsPool[HasNumber(PWDsPool, 0)]);
            if (HasLetter(CreatStr.ToString()) != -1) ReadyStr.Append(PWDsPool[HasLetter(PWDsPool)]);
            if (HasCharacter(CreatStr.ToString()) != -1) ReadyStr.Append(PWDsPool[HasCharacter(PWDsPool)]);

            ReadyStr.Append(CreatStr.ToString());
            return ReadyStr.ToString().Substring(0, Length);
        }

        /// <summary>
        /// 乱序算法
        /// </summary>
        /// <param name="PWDs">原字符串</param>
        /// <returns>打乱后的库</returns>
        static string FlyTheWords(string PWDs)
        {
            StringBuilder FinalStr = new StringBuilder();
            for (int i = 0; i < (160 / PWDs.Length + 1); i++)
            {
                FinalStr.Append(PWDs);
            }

            string Temp = FinalStr.ToString().Substring(50, 100);
            Console.WriteLine(FinalStr.Length + ":" + Temp.Length);
            StringBuilder SndTemp = new StringBuilder();
            for (int i = 0; i < RandNum.Length; i++)
            {
                SndTemp.Append(Temp[RandNum[i]]);
            }

            return SndTemp.ToString();
        }

        /// <summary>
        /// 判定字符串中是否包含数字
        /// </summary>
        /// <param name="Source">字符串</param>
        /// <param name="FirstIndex">搜索的起始位置</param>
        /// <returns>数字的位置</returns>
        static int HasNumber(string Source, int FirstIndex)
        {
            int Has = -1;

            if (FirstIndex > Source.Length)
            {
                FirstIndex = FirstIndex - (FirstIndex / Source.Length) * Source.Length;
            }

            for (int i = FirstIndex; i < Source.Length; i++)
            {
                if (Char.IsNumber(Source[i]))
                {
                    Has = i;
                    break;
                }
            }

            return Has;
        }

        static int HasLetter(string Source)
        {
            int Has = -1;
            for (int i = 0; i < Source.Length; i++)
            {
                if (Char.IsLetter(Source[i]))
                {
                    Has = i;
                    break;
                }
            }
            return Has;
        }

        static int HasCharacter(string Source)
        {
            int Has = -1;
            for (int i = 0; i < Source.Length; i++)
            {
                if (Char.IsNumber(Source[i]) || Char.IsLetter(Source[i]))
                {
                    continue;
                }
                Has = i;
                break;
            }
            return Has;
        }
    }
}
