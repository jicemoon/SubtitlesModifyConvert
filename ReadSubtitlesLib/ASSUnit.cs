/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/7 17:08:35
* FileName：ASSUnit
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
    public class ASSUnit:SubtitlesUnit
    {
        #region 字段
        public string Layer = "0";
        public string Style = "*Default";
        public string Name = "";
        public string Actor = "NTP";
        public string MarginL = "0000";
        public string MarginR = "0000";
        public string MarginV = "0000";
        public string Effect = "";
        #endregion

        #region 属性
        public static string[] Format = new string[] { "Layer", "Start", "End", "Style", "Actor", "MarginL", "MarginR", "MarginV", "Effect", "Text" };
        #endregion

        #region 构造函数
        public ASSUnit(uint begin = 0, uint end = 0, List<string> contents = null):base(begin,end,contents)
        {
        }
        public ASSUnit(SubtitlesUnit su):base(su)
        {
        }
        #endregion

        #region 公有方法
        public override List<string> toLines(int index)
        {
            List<string> rtn = base.toLines(index);
            string temp = "Dialogue: ";
            for(int i = 0; i < Format.Length; i++)
            {
                if(i > 0)
                {
                    temp += ",";
                }
                if(Format[i].Trim() == "Start")
                {
                    temp += this.BeginStr;
                }
                else if(Format[i].Trim() == "End")
                {
                    temp += this.EndStr;
                }
                else if(Format[i].Trim() == "Text")
                {
                    temp += string.Join("\\N", Contents);
                }
                else
                {
                    temp += ReadSubtitles.getPropertyValue(this, Format[i]);
                }
            }
            rtn.Add(temp);
            return rtn;
        }
        public override SubtitlesUnit clone()
        {
            ASSUnit au = new ASSUnit(this);
            au.Layer = this.Layer;
            au.Style = this.Style;
            au.Name = this.Name;
            au.Actor = this.Actor;
            au.MarginL = this.MarginL;
            au.MarginR = this.MarginR;
            au.MarginV = this.MarginV;
            au.Effect = this.Effect;
            return au;
        }
        #endregion

        #region 私有方法
        protected override string time2Str(uint time)
        {
            return time2String(time, 1, '.');
        }
        protected override uint time2Uint(string time)
        {
            return time2Milli(time, '.');
        }
        #endregion

        #region 事件监听

        #endregion
    }

    public class ScriptInfo
    {

    }
    public class Styles
    {
        public static string[] Format = new string[] { "Name", "Fontname", "Fontsize", "PrimaryColour", "SecondaryColour", "OutlineColour", "BackColour", "Bold", "Italic", "Underline", "StrikeOut", "ScaleX", "ScaleY", "Spacing", "Angle", "BorderStyle", "Outline", "Shadow", "Alignment", "MarginL", "MarginR", "MarginV", "Encoding" };
        public string Name = "Default";
        public string Fontname = "方正黑体_GBK";
        public string Fontsize = "20";
        public string PrimaryColour = "&H00FFFFFF";
        public string SecondaryColour = "&HF0000000";
        public string OutlineColour = "&H00000000";
        public string BackColour = "&H32000000";
        public string Bold = "0";
        public string Italic = "0";
        public string Underline = "0";
        public string StrikeOut = "0";
        public string ScaleX = "100";
        public string ScaleY = "100";
        public string Spacing = "0";
        public string Angle = "0";
        public string BorderStyle = "1";
        public string Outline = "2";
        public string Shadow = "1";
        public string Alignment = "2";
        public string MarginL = "5";
        public string MarginR = "5";
        public string MarginV = "2";
        public string Encoding = "134";

        public override string ToString()
        {
            string rtn = "Style: ";
            for(int i = 0; i < Format.Length; i++)
            {
                if(i > 0)
                {
                    rtn += ",";
                }
                rtn += ReadSubtitles.getPropertyValue(this, Format[i]);
            }
                //rtn += Name;
                //rtn += "," + Fontname;
                //rtn += "," + Fontsize;
                //rtn += "," + PrimaryColour;
                //rtn += "," + SecondaryColour;
                //rtn += "," + OutlineColour;
                //rtn += "," + BackColour;
                //rtn += "," + Bold;
                //rtn += "," + Italic;
                //rtn += "," + Underline;
                //rtn += "," + StrikeOut;
                //rtn += "," + ScaleX;
                //rtn += "," + ScaleY;
                //rtn += "," + Spacing;
                //rtn += "," + Angle;
                //rtn += "," + BorderStyle;
                //rtn += "," + Outline;
                //rtn += "," + Shadow;
                //rtn += "," + Alignment;
                //rtn += "," + MarginL;
                //rtn += "," + MarginR;
                //rtn += "," + MarginV;
                //rtn += "," + Encoding;
                return rtn;
        }
        public Styles clone()
        {
            Styles rtn = new Styles();
            for(int i = 0; i < Format.Length; i++)
            {
                ReadSubtitles.setPropertyValue(rtn, Format[i],ReadSubtitles.getPropertyValue(this, Format[i]));
            }
            return rtn;
        }
    }
}
