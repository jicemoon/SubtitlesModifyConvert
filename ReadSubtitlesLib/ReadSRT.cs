/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/9 15:22:59
* FileName：ReadSRT
* Version：V0.0.0.0
* ===============================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ReadSubtitlesLib
{
    public class ReadSRT:ReadSubtitles
    {
        #region 字段
        #endregion

        #region 属性
        public static Regex TimeReg = new Regex(@"\d\d:\d\d:\d\d,\d\d\d --> \d\d:\d\d:\d\d,\d\d\d");

        #endregion

        #region 构造函数

        #endregion

        #region 公有方法
        override public void readSubtitles(string path)
        {
            base.readSubtitles(path);
            if(textLines == null || textLines.Length <= 0)
                return;
            if(subtitlesUnits != null && subtitlesUnits.Count > 0)
                subtitlesUnits.Clear();
            else
                subtitlesUnits = new List<SubtitlesUnit>();
            Regex numReg = new Regex(@"^[1-9]\d*$");
            int lens = textLines.Length;
            SRTUnit timeSRT = null;

            for(int i = 0; i < lens; i++)
            {
                string tempStr = textLines[i];
                tempStr = tempStr.Trim();
                if(tempStr.Length == 0)
                {
                    continue;
                }
                else
                {
                    if(TimeReg.IsMatch(tempStr))
                    {
                        timeSRT = new SRTUnit(tempStr);
                        subtitlesUnits.Add(timeSRT);
                    }
                    else
                    {
                        if(timeSRT != null && !numReg.IsMatch(tempStr))
                            timeSRT.Contents.Add(tempStr);
                    }
                }
            }
        }
        #endregion

        #region 私有方法

        #endregion

        #region 事件监听

        #endregion
    }
}
