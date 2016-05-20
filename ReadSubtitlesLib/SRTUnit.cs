/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/9 15:14:05
* FileName：SRTUnit
* Version：V0.0.0.0
* ===============================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadSubtitlesLib
{
    public class SRTUnit:SubtitlesUnit
    {
        #region 字段

        #endregion

        #region 属性
        public string TimeStr
        {
            get
            {
                return BeginStr + " --> " + EndStr;
            }
            set
            {
                timeStr2Uint(value);
            }
        }
        #endregion

        #region 构造函数
        public SRTUnit(uint begin = 0, uint end = 0, List<string> contents = null):base(begin,end,contents)
        {
        }
        public SRTUnit(SubtitlesUnit su):base(su)
        {
        }

        public SRTUnit(string timeStr, List<string> contents=null)
        {
            this.contents = contents;
            timeStr2Uint(timeStr.Trim());
        }
        #endregion

        #region 公有方法
        public override List<string> toLines(int index)
        {
            List<string> rtn = base.toLines(index);
            rtn.Add((index + 1).ToString());
            rtn.Add(TimeStr);
            for(int j = 0; j < Contents.Count; j++)
            {
                rtn.Add(Contents[j]);
            }
            rtn.Add("");
            return rtn;
        }
        public override SubtitlesUnit clone()
        {
            return new SRTUnit(this);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将形如"00:00:00,000 --> 00:00:00,000"格式的字符串转化为开始时间和结束时间
        /// </summary>
        /// <param name="str">形如"00:00:00,000 --> 00:00:00,000"格式的字符串</param>
        private void timeStr2Uint(string str)
        {

            if(ReadSRT.TimeReg.IsMatch(str))
            {
                string[] temp = str.Split(new string[] { " --> " }, StringSplitOptions.RemoveEmptyEntries);

                beginStr = temp[0];
                endStr = temp[1];
                beginTime = time2Uint(beginStr);
                endTime = time2Uint(endStr);
            }
        }
        #endregion

        #region 事件监听

        #endregion
    }
}
