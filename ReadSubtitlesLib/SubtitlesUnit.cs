/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/7 15:23:55
* FileName：SubtitlesUnit
* Version：V0.0.0.0
* ===============================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadSubtitlesLib
{
    public abstract class SubtitlesUnit:IComparable
    {
        #region 字段
        protected uint beginTime=0;
        protected string beginStr;
        protected uint endTime=0;
        protected string endStr;
        protected List<string> contents;
        #endregion

        #region 属性
        public string BeginStr
        {
            get
            {
                beginStr = time2Str(beginTime);
                return beginStr;
            }
            set
            {                
                beginStr = value;
                beginTime = time2Uint(value);
            }
        }
        public string EndStr
        {
            get
            {
                EndStr = time2Str(endTime);
                return endStr;
            }
            set
            {
                endStr = value;
                endTime = time2Uint(value);
            }
        }
        public uint BeginTime
        {
            get
            {
                return beginTime;
            }
            set
            {
                beginTime = value;
                //beginStr = time2Str(beginTime);
            }
        }
        public uint EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;
                //endStr = time2Str(endTime);
            }
        }
        public List<string> Contents
        {
            get
            {
                if(contents == null)
                    contents = new List<string>();
                return contents;
            }
            set
            {
                contents = value;
            }
        }
        #endregion

        #region 构造函数
        public SubtitlesUnit(uint begin=0, uint end=0, List<string> contents = null)
        {
            this.beginTime = begin;
            this.endTime = end;
            this.contents = contents;
        }
        public SubtitlesUnit(SubtitlesUnit su)
        {
            this.beginTime = su.beginTime;
            this.endTime = su.endTime;
            this.contents = new List<string>();
            this.contents.AddRange(su.contents);
        }
        #endregion

        #region 公有方法
        public int CompareTo(object su)
        {
            return (int)beginTime - (int)(su as SubtitlesUnit).beginTime;
        }
        /// <summary>
        /// 将本字幕单元转化为文本中对应的行形式
        /// </summary>
        /// <param name="index">该字幕单元在整个字幕中的索引</param>
        /// <returns></returns>
        public virtual List<string> toLines(int index)
        {
            return new List<string>();
        }
        public virtual SubtitlesUnit clone()
        {
            return null;
        }
        public virtual void deleteFormat(bool all=false)
        {
            string[] temp;
            //int index = -1;
            List<string> subline;
            Regex reg = new Regex(@"{.*(\\pos\(.*?\)).*}", RegexOptions.IgnoreCase);
            Regex reg1 = new Regex(@"{.*}", RegexOptions.IgnoreCase);
            for(int j = 0; j < contents.Count; j++)
            {
                string str = contents[j];
                temp = str.Split(new string[] { "}" }, StringSplitOptions.RemoveEmptyEntries);
                subline = new List<string>();
                for(int i = 0; i < temp.Length; i++)
                {
                    str = temp[i] + "}";
                    if(!all && reg.IsMatch(str))
                    {
                        //保留位置信息
                        subline.Add(reg.Replace(str, @"{$1}"));
                    }
                    else
                    {
                        //清除所有样式
                        if(reg1.IsMatch(str))
                            subline.Add(reg1.Replace(str, ""));
                        else
                            subline.Add(str.Substring(0, str.Length - 1));
                    }
                }
                contents[j] = string.Join("", subline);
                if(contents[j].Length <= 0)
                {
                    contents.RemoveAt(j);
                    j--;
                }                
            }
        }
        #endregion

        /// <summary>
        /// 将毫秒数转化为目标字幕时间格式的字符串
        /// </summary>
        /// <param name="time">需要转换的毫秒数</param>
        /// <returns>目标字幕时间格式的字符串</returns>
        #region 私有方法
        protected virtual string time2Str(uint time)
        {
            return time2String(time);
        }
        /// <summary>
        /// 将形如"00:00:00,000"格式的字符串转化为毫秒数
        /// </summary>
        /// <param name="time">形如"00:00:00,000"格式的字符串</param>
        /// <returns>转换后的毫秒数</returns>
        protected virtual uint time2Uint(string time)
        {
            return time2Milli(time);
        }
        /// <summary>
        /// 将毫秒数转化为形如"00:00:00,000"格式的字符串
        /// </summary>
        /// <param name="time">需要转换的毫秒数</param>
        /// <param name="hl">小时格式化长度</param>
        /// <param name="smSplit">秒和毫秒之间的分隔符</param>
        /// <param name="ml">分钟格式化长度</param>
        /// <param name="sl">秒数格式化长度</param>
        /// <param name="mil">毫秒数格式化长度</param>
        /// <returns></returns>
        protected string time2String(uint time, uint hl = 2, char smSplit = ',', uint ml = 2, uint sl = 2, uint mil = 3)
        {
            string rtn;
            uint hour, minute, second, milli;
            milli = time % 1000;
            second = time / 1000;
            minute = second / 60;
            second = second % 60;
            hour = minute / 60;
            minute = minute % 60;

            rtn = hour.ToString("d" + hl);
            rtn += ":" + minute.ToString("d" + ml);
            rtn += ":" + second.ToString("d" + sl);
            rtn += smSplit + milli.ToString("d" + mil);
            return rtn;
        }
        /// <summary>
        /// 将形如"00:00:00,000""0:00:00.000"格式的字符串转化为毫秒数
        /// </summary>
        /// <param name="time">形如"00:00:00,000"格式的字符串</param>
        /// <param name="smSplit">毫秒与秒之间的间隔符</param>
        /// <returns>毫秒数</returns>
        protected uint time2Milli(string time, char smSplit = ',')
        {
            time = time.Trim();
            uint rtn = 0;
            uint hour, minute, second, milli;
            List<string> temp = time.Split(new char[] { ':' }).ToList();
            if(temp.Count != 3)
                return 0;
            string[] tt = temp[2].Split(new char[] { smSplit });
            temp.RemoveAt(2);
            temp.AddRange(tt);
            temp[3] = (temp[3] + "00").Substring(0, 3);
            if(temp.Count == 4)
            {
                hour = Convert.ToUInt32(temp[0]);
                minute = Convert.ToUInt32(temp[1]);
                second = Convert.ToUInt32(temp[2]);
                milli = Convert.ToUInt32(temp[3]);
                rtn = (hour * 3600 + minute * 60 + second) * 1000 + milli;
            }
            return rtn;
        }
        #endregion

        #region 事件监听

        #endregion

    }
}
