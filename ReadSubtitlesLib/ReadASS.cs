/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/7 16:26:41
* FileName：ReadASS
* Version：V0.0.0.0
* ===============================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ReadSubtitlesLib
{
    public class ReadASS:ReadSubtitles
    {
        #region 字段
        protected ScriptInfo si;
        protected List<Styles> styleList;
        #endregion

        #region 属性
        public List<Styles> StyleList
        {
            get
            {
                if(styleList == null){
                    styleList = new List<Styles>();
                }
                return  styleList;
            }
        }
        public ScriptInfo ScriptInfomation
        {
            get
            {
                return si;
            }
            set
            {
                si = value;
            }
        }
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
            if(styleList != null && styleList.Count > 0)
                styleList.Clear();
            else
                styleList = new List<Styles>();
            Console.WriteLine("lines length ==> " + textLines.Length);
            uint currentType = 0;
            string[] format = null;
            for(int i = 0; i < textLines.Length; i++)
            {
                textLines[i] = textLines[i].Trim();
                if(textLines[i].IndexOf("[Script Info]") > -1)
                {
                    currentType = 1;
                }
                else if(textLines[i].IndexOf("[V4+ Styles]") > -1 || textLines[i].IndexOf("[V4 Styles]") > -1)
                {
                    currentType = 2;
                }
                else if(textLines[i].IndexOf("[Events]") > -1)
                {
                    currentType = 3;
                }
                switch(currentType)
                {
                    case 1:
                        //Script Info
                        break;
                    case 2:
                        //Styles
                        if(textLines[i].IndexOf("Format") == 0)
                        {
                            format = textLines[i].Replace("Format:", "").Trim().Split(new char[] { ',' });
                        }
                        else if(textLines[i].IndexOf("Style") == 0)
                        {
                            string[] dialogue = textLines[i].Replace("Style:", "").Trim().Split(new char[] { ',' }, StringSplitOptions.None);
                            Styles st = new Styles();
                            for(int j = 0; j < format.Length - 1; j++)
                            {
                                setPropertyValue(st, format[j].Trim(), dialogue[j]);
                            }
                            styleList.Add(st);
                        }
                        break;
                    case 3:
                        //Dialogue
                        if(textLines[i].IndexOf("Format") == 0)
                        {
                            format = textLines[i].Replace("Format:", "").Trim().Split(new char[] { ','});
                        }
                        else if(textLines[i].IndexOf("Dialogue") == 0)
                        {
                            string[] dialogue = textLines[i].Replace("Dialogue:", "").Trim().Split(new char[] { ',' }, StringSplitOptions.None);
                            ASSUnit au = new ASSUnit();
                            for(int j = 0; j < format.Length - 1; j++)
                            {
                                if(format[j].Trim() == "Start")
                                {
                                    au.BeginStr = dialogue[j];
                                }
                                else if(format[j].Trim() == "End")
                                {
                                    au.EndStr = dialogue[j];
                                }
                                else
                                {
                                    setPropertyValue(au, format[j].Trim(), dialogue[j]);
                                }
                            }
                            string content = String.Join(",", dialogue.Skip(format.Length - 1));
                            au.Contents = content.Split(new string[] { "\\N" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            //Console.WriteLine(string.Join("\n\t", au.Contents));
                            subtitlesUnits.Add(au);
                        }
                        break;
                }
            }
            //Console.WriteLine("subtitlesUnits length ==> " + subtitlesUnits.Count);
        }

        #endregion

        #region 私有方法
        protected override List<string> allToStringLine()
        {
            List<string> baseRtn = base.allToStringLine();
            if(baseRtn == null)
                return null;
            List<string> allLine = new List<string>();
            //string temp = "";
            //字幕基本信息
            allLine.Add("[Script Info]");

            //字幕样式设置
            allLine.Add("");
            allLine.Add("[V4+ Styles]");
            allLine.Add("Format: " + string.Join(",", Styles.Format));
            for(int i = 0; i < styleList.Count; i++)
            {
                allLine.Add(styleList[i].ToString());
            }
            //字幕正式内容
            allLine.Add("");
            allLine.Add("[Events]");
            allLine.Add("Format: " + string.Join(",", ASSUnit.Format));
            allLine.AddRange(baseRtn);
            return allLine;
        }
        #endregion

        #region 事件监听

        #endregion
    }

}
