/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/7 15:20:26
* FileName：ReadSubtitles
* Version：V0.0.0.0
* ===============================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace ReadSubtitlesLib
{
    public abstract class ReadSubtitles
    {
        #region 字段
        
        protected string fileInUrl = "";
        protected List<SubtitlesUnit> subtitlesUnits = null;
        protected String[] textLines = null;
        #endregion

        #region 属性
        public List<SubtitlesUnit> SubtitlesUnits
        {
            get
            {
                if(subtitlesUnits == null)
                {
                    subtitlesUnits = new List<SubtitlesUnit>();
                }
                return subtitlesUnits;
            }
        }
        #endregion

        #region 构造函数

        #endregion
       
        #region 静态方法
        /// <summary>
        /// 给指定对象的指定属性赋值
        /// </summary>
        /// <param name="obj">要赋值的指定对象</param>
        /// <param name="proper">要赋值的属性字符串</param>
        /// <param name="value">该属性的值</param>
        public static void setPropertyValue(object obj, string proper, object value)
        {
            try
            {
                PropertyInfo pi = obj.GetType().GetProperty(proper);
                if(pi != null)
                    pi.SetValue(obj, value);
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine("找不到对应的属性 --> " + proper + " : " + e.Message);
            }
        }
        /// <summary>
        /// 获取指定对象的指定属性值
        /// </summary>
        /// <param name="obj">指定对象</param>
        /// <param name="proper">指定属性的字符串形式</param>
        /// <returns></returns>
        public static Object getPropertyValue(object obj, string proper)
        {
            try
            {
                PropertyInfo pi = obj.GetType().GetProperty(proper);
                if(pi != null)
                    return pi.GetValue(obj);
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine("找不到对应的属性 --> " + proper + " : " + e.Message);
            }
            return null;
        }
        /// <summary>
        /// 从指定路径读取字幕文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ReadSubtitles ReadSubtitlesFromPath(string path)
        {
            if(!File.Exists(path))
                return null;
            ReadSubtitles rs=null;
            switch (Path.GetExtension(path).ToLower())
            {
                case ".srt":
                    rs = new ReadSRT();
                    break;
                case ".ass":
                    rs = new ReadASS();
                    break;
            }
            if(rs != null)
                rs.readSubtitles(path);
            return rs;
        }
        /// <summary>
        /// 将参数中的两个字幕,按照时间进行合并,如果开始和结束时间相同,合并为同一个字幕元素,参数中前面的字幕在上方
        /// </summary>
        /// <param name="path1">要合并的第一个字幕</param>
        /// <param name="path2">要合并的第二个字幕</param>
        /// <param name="toOneLine">合并字幕前是否将字幕放在同一行, 默认为true,表示合并</param>
        /// <param name="contect">合并字幕时的连接符</param>
        /// <returns></returns>
        public static ReadSubtitles MergeSubtitlesByTime(string path1, string path2, uint targetType = SubtitlesType.Type_Current, bool toOneLine = true, string contect = " ")
        {
            ReadSubtitles rs1 = ReadSubtitlesFromPath(path1);
            ReadSubtitles rs2 = ReadSubtitlesFromPath(path2);

            if(toOneLine)
            {
                rs1.toOneLine(contect);
                rs2.toOneLine(contect);
            }
            rs1.resortSrt();
            rs2.resortSrt();

            return rs1.mergeSubtitlesByTime(rs2, targetType);
        }

        /// <summary>
        /// 将参数路径中的字幕转化为参数类型的字幕
        /// </summary>
        /// <param name="inPath">要转化的字幕路径</param>
        /// <param name="type">要转化的字幕类型</param>
        /// <param name="outPath">输出路径</param>
        /// <returns></returns>
        public static ReadSubtitles ConvertTo(string inPath, uint type = SubtitlesType.Type_SRT, string outPath="")
        {
            ReadSubtitles rs = ReadSubtitles.ReadSubtitlesFromPath(inPath);
            return rs == null ? null : rs.convertTo(type, outPath);
        }
        #endregion
        #region 公有方法
        //需要重写
        /// <summary>
        /// 开始读取SRT文件
        /// </summary>
        /// <param name="path">要读取的SRT文件路径</param>
        public virtual void readSubtitles(string path)
        {
            fileInUrl = path;
            textLines = null;
            if(!File.Exists(fileInUrl))
                return;
            textLines = File.ReadAllLines(fileInUrl, Encoding.Default);
        }
        /// <summary>
        /// 删除空白
        /// </summary>
        public void deleteSpace()
        {
            if(subtitlesUnits != null && subtitlesUnits.Count > 0)
            {
                for(int i = 0; i < subtitlesUnits.Count; i++)
                {
                    if(subtitlesUnits[i].Contents.Count <= 0)
                    {
                        subtitlesUnits.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        for(int j = 0; j < subtitlesUnits[i].Contents.Count; j++)
                        {
                            subtitlesUnits[i].Contents[j] = subtitlesUnits[i].Contents[j].Trim();
                        }
                        if(subtitlesUnits[i].Contents.Count == 2)
                        {
                            if(subtitlesUnits[i].Contents[0] == subtitlesUnits[i].Contents[1])
                            {
                                subtitlesUnits[i].Contents.RemoveAt(1);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 将单个时间段中的字幕,整理到同一行
        /// </summary>
        /// <param name="connect">不同行中的连接符,默认为一个空格</param>
        public void toOneLine(string connect = " ")
        {
            if(subtitlesUnits == null || subtitlesUnits.Count <= 0)
                return;
            int lens = subtitlesUnits.Count;
            string tempStr = "";
            for(int i = 0; i < lens; i++)
            {
                tempStr = string.Join(connect, subtitlesUnits[i].Contents);
                subtitlesUnits[i].Contents.Clear();
                subtitlesUnits[i].Contents.Add(tempStr);
            }
        }
        /// <summary>
        /// 重新排序时间轴
        /// </summary>
        /// <param name="byBeginTime">默认按开始时间升序排列,如果设置为false,则按照结束时间升序排列</param>
        public void resortSrt(bool byBeginTime = true)
        {
            if(subtitlesUnits == null || subtitlesUnits.Count <= 0)
                return;
            if(byBeginTime)
                subtitlesUnits.Sort();
            else
            {
                int lens = subtitlesUnits.Count;
                SubtitlesUnit templs;
                for(int i = 0; i < lens - 2; i++)
                {
                    for(int j = lens - 1; j > i; j--)
                    {
                        if(subtitlesUnits[j].EndTime < subtitlesUnits[j - 1].EndTime)
                        {
                            templs = subtitlesUnits[j - 1];
                            subtitlesUnits[j - 1] = subtitlesUnits[j];
                            subtitlesUnits[j] = templs;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 重设时间轴,
        /// 如果含有相同时间(开始和结束时间相同, 必须相邻)的单元, 将时间按照字符个数分割, 
        /// 如果单元1的开始时间小于单元2的开始时间,但是单元1的结束时间大于单元2的开始时间, 单元1的结束时间将改成单元2的开始时间
        /// </summary>
        public void sortConvergenceTime()
        {
            int length = subtitlesUnits.Count;
            for(int i = 1; i < length; i++)
            {
                if(subtitlesUnits[i - 1].BeginTime == subtitlesUnits[i].BeginTime && subtitlesUnits[i - 1].EndTime == subtitlesUnits[i].EndTime)
                {
                    seprateTime(i - 1);
                }
                if(subtitlesUnits[i - 1].EndTime > subtitlesUnits[i].BeginTime)
                {
                    subtitlesUnits[i - 1].EndTime = subtitlesUnits[i].BeginTime;
                }
            }
        }
        /// <summary>
        /// 将字幕写入文件
        /// </summary>
        /// <param name="path">文件路径,如果为空值,将写入到源文件中</param>
        public void writeSubtitles(string path = "")
        {
            if(subtitlesUnits == null || subtitlesUnits.Count <= 0)
                return;
            if(path == "")
                path = fileInUrl;
            List<string> allLine = allToStringLine();
            if(!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            if(allLine != null)
                File.WriteAllLines(path, allLine, Encoding.UTF8);
        }
        /// <summary>
        /// 平移整个字幕的时间轴,
        /// </summary>
        /// <param name="mili">要添加或减少的毫秒数</param>
        public void translateTime(int mili)
        {
            int length = subtitlesUnits.Count;
            for(int i = 0; i < length; i++)
            {
                subtitlesUnits[i].BeginTime = (uint)Math.Max(0, subtitlesUnits[i].BeginTime + mili);
                subtitlesUnits[i].EndTime = (uint)Math.Max(0, subtitlesUnits[i].EndTime + mili);
            }
        }
        /// <summary>
        /// 将单元按照参数分割成多个单元, 只有单元为两行, 并且两行中分隔符的个数相同时, 才进行分割
        /// </summary>
        /// <param name="breakStr">分割参照字符</param>
        public void breakDialogMore(string breakStr = "- ")
        {
            int length = subtitlesUnits.Count;
            List<SubtitlesUnit> tempLines = new List<SubtitlesUnit>();

            for(int i = 0; i < length; i++)
            {
                if(subtitlesUnits[i].Contents.Count == 2)
                {
                    string[] ta0 = subtitlesUnits[i].Contents[0].Split(new string[] { breakStr }, StringSplitOptions.RemoveEmptyEntries);
                    string[] ta1 = subtitlesUnits[i].Contents[1].Split(new string[] { breakStr }, StringSplitOptions.RemoveEmptyEntries);

                    if(ta0.Length == ta1.Length && ta0.Length > 1)
                    {
                        double totalWord = 0;
                        for(int j = 0; j < ta1.Length; j++)
                        {
                            ta0[j] = ta0[j].Trim();
                            ta1[j] = ta1[j].Trim();
                            totalWord += ta1[j].Length;
                        }
                        double totalTime = Math.Max(0, subtitlesUnits[i].EndTime - subtitlesUnits[i].BeginTime);
                        for(int j = 0; j < ta1.Length; j++)
                        {
                            SubtitlesUnit tempSU = subtitlesUnits[i].clone();
                            if(j == 0)
                                tempSU.BeginTime = subtitlesUnits[i].BeginTime;
                            else
                                tempSU.BeginTime = tempLines[tempLines.Count - 1].EndTime;
                            tempSU.Contents.Add(ta0[j]);
                            tempSU.Contents.Add(ta1[j]);
                            tempSU.EndTime = tempSU.BeginTime + (uint)(totalTime * ((double)ta1[j].Length / totalWord));
                            tempLines.Add(tempSU);
                        }

                    }
                    else
                    {
                        tempLines.Add(subtitlesUnits[i]);
                    }

                }
                else
                {
                    tempLines.Add(subtitlesUnits[i]);
                }

            }
            subtitlesUnits = tempLines;
            deleteSpace();
        }
        /// <summary>
        /// 分割对话(单行字幕)
        /// </summary>
        /// <param name="breakStr">单元内包含此字符时, 会根据此字符将本单元分割成多个单元</param>
        /// <param name="wordNum">单元内最多的字符个数, 此数字小于等于0时, 无效</param>
        public void breakDialog(string breakStr = "- ", int wordNum = 15)
        {
            int length = subtitlesUnits.Count;
            List<SubtitlesUnit> tempLines = new List<SubtitlesUnit>();
            for(int i = 0; i < length; i++)
            {
                string tempCont = string.Join(" ", subtitlesUnits[i].Contents);
                string[] ta0 = tempCont.Split(new string[] { breakStr }, StringSplitOptions.RemoveEmptyEntries);
                List<string> tempArr = new List<string>();
                for(int j = 0; j < ta0.Length; j++)
                {
                    if(wordNum > 0 && ta0[j].Length >= wordNum)
                    {
                        breakDialogByNum(ta0[j], wordNum);
                    }
                    else
                    {
                        tempArr.Add(ta0[j]);
                    }
                }
                if(tempArr.Count > 1)
                {
                    double totalWord = 0;
                    for(int j = 0; j < tempArr.Count; j++)
                    {
                        tempArr[j] = tempArr[j].Trim();
                        totalWord += tempArr[j].Length;
                    }
                    double totalTime = Math.Max(0, subtitlesUnits[i].EndTime - subtitlesUnits[i].BeginTime);
                    for(int j = 0; j < tempArr.Count; j++)
                    {
                        SubtitlesUnit tempSU = subtitlesUnits[i].clone();
                        if(j == 0)
                            tempSU.BeginTime = subtitlesUnits[i].BeginTime;
                        else
                            tempSU.BeginTime = tempLines[tempLines.Count - 1].EndTime;
                        tempSU.Contents.Add(tempArr[j]);
                        tempSU.EndTime = tempSU.BeginTime + (uint)(totalTime * ((double)tempArr[j].Length / totalWord));
                        tempLines.Add(tempSU);
                    }
                }
                else
                {
                    tempLines.Add(subtitlesUnits[i]);
                }
            }
            subtitlesUnits = tempLines;
            deleteSpace();
        }
        /// <summary>
        /// 将当前字幕和参数中的字幕,按照时间进行合并,如果开始和结束时间相同,合并为同一个字幕元素,当前字幕在上方
        /// </summary>
        /// <param name="subtitles">要参与合并的字幕</param>
        /// <param name="outPath">合并后字幕要输出的位置, 如果此值不为空,且为正确的路径,回自动写入合并后的字幕</param>
        /// <returns>合并后的字幕</returns>
        public ReadSubtitles mergeSubtitlesByTime(ReadSubtitles subtitles, uint type=SubtitlesType.Type_Current, string outPath = "")
        {
            ReadSRT rtn= new ReadSRT();
            rtn.subtitlesUnits = new List<SubtitlesUnit>();
            int lens1 = subtitlesUnits.Count;
            int lens2 = subtitles.subtitlesUnits.Count;
            rtn.fileInUrl = outPath == "" ? fileInUrl : outPath;
            SRTUnit tempUnit;
            int i = 0, j = 0;
            while(i < lens1 || j < lens2)
            {
                tempUnit = new SRTUnit();
                if(i < lens1)
                {
                    if(j < lens2)
                    {
                        if(subtitlesUnits[i].BeginTime == subtitles.subtitlesUnits[j].BeginTime && subtitlesUnits[i].EndTime == subtitles.subtitlesUnits[j].EndTime)
                        {
                            tempUnit.BeginTime = subtitlesUnits[i].BeginTime;
                            tempUnit.EndTime = subtitlesUnits[i].EndTime;
                            tempUnit.Contents.AddRange(subtitlesUnits[i].Contents);
                            tempUnit.Contents.AddRange(subtitles.subtitlesUnits[j].Contents);
                            i++;
                            j++;
                        }
                        else if(subtitlesUnits[i].BeginTime < subtitles.subtitlesUnits[j].BeginTime)
                        {
                            tempUnit.BeginTime = subtitlesUnits[i].BeginTime;
                            tempUnit.EndTime = subtitlesUnits[i].EndTime;
                            tempUnit.Contents.AddRange(subtitlesUnits[i].Contents);
                            i++;
                        }
                        else
                        {
                            tempUnit.BeginTime = subtitles.subtitlesUnits[j].BeginTime;
                            tempUnit.EndTime = subtitles.subtitlesUnits[j].EndTime;
                            tempUnit.Contents.AddRange(subtitles.subtitlesUnits[j].Contents);
                            j++;
                        }
                    }
                    else
                    {
                        tempUnit.BeginTime = subtitlesUnits[i].BeginTime;
                        tempUnit.EndTime = subtitlesUnits[i].EndTime;
                        tempUnit.Contents.AddRange(subtitlesUnits[i].Contents);
                        i++;
                    }
                }
                else
                {
                    tempUnit.BeginTime = subtitles.subtitlesUnits[j].BeginTime;
                    tempUnit.EndTime = subtitles.subtitlesUnits[j].EndTime;
                    tempUnit.Contents.AddRange(subtitles.subtitlesUnits[j].Contents);
                    j++;
                }
                rtn.subtitlesUnits.Add(tempUnit);
            }
            ReadSubtitles rs = this.convertTo(type);
            if(outPath != null && outPath.Length > 0)
                rs.writeSubtitles();
            return rs;
        }
        /// <summary>
        /// 将当前字幕转化为参数类型的字幕
        /// </summary>
        /// <param name="type">要转化的字幕类型</param>
        /// <param name="outPath">输出路径</param>
        /// <returns></returns>
        public virtual ReadSubtitles convertTo(uint type = SubtitlesType.Type_SRT, string outPath="")
        {
            if(type == SubtitlesType.Type_Current)
            {
                if(this is ReadASS){
                    type = SubtitlesType.Type_ASS;
                }
                else{
                    type = SubtitlesType.Type_SRT;
                }
            }
            ReadSubtitles rs = null;
            switch(type)
            {
                case 1:
                    //srt
                    rs = new ReadSRT();
                    rs.subtitlesUnits = new List<SubtitlesUnit>();
                    for(int i = 0; i < subtitlesUnits.Count; i++)
                    {
                        rs.subtitlesUnits.Add(new SRTUnit(subtitlesUnits[i]));
                    }
                    break;
                case 2:
                    //ASS
                    rs = new ReadASS();
                    (rs as ReadASS).StyleList.Add(new Styles());
                    (rs as ReadASS).ScriptInfomation = new ScriptInfo();
                    //rs.subtitlesUnits = new List<SubtitlesUnit>();
                    for(int i = 0; i < subtitlesUnits.Count; i++)
                    {
                        rs.subtitlesUnits.Add(new ASSUnit(subtitlesUnits[i]));
                    }
                    break;
            }
            if(outPath != "")
            {
                rs.writeSubtitles(outPath);
            }
            return rs;
        }
        /// <summary>
        /// 删除字幕中多余的格式
        /// </summary>
        /// <param name="all">是否删除所有的字幕格式, 默认为false, 会保留字幕位置格式</param>
        public virtual void deleteLineFormat(bool all = false)
        {
            for(int i = 0; i < subtitlesUnits.Count; i++)
            {
                subtitlesUnits[i].deleteFormat(all);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将当前字幕转化为List&lt;string&gt;(每个元素都是文本文件中的一整行)
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> allToStringLine()
        {
            if(subtitlesUnits == null || subtitlesUnits.Count <= 0)
                return null;
            List<string> lineAll = new List<string>();
            for(int i = 0; i < subtitlesUnits.Count; i++)
            {
                lineAll.AddRange(subtitlesUnits[i].toLines(i));
            }
            return lineAll;
        }
        /// <summary>
        /// 为相同时间的单元,做时间分割(根据个单元字符数)
        /// </summary>
        /// <param name="start">开始检测的索引值</param>
        protected void seprateTime(int start)
        {
            uint totalTime = subtitlesUnits[start].EndTime - subtitlesUnits[start].BeginTime;
            double totalWord = 0;
            int end = start;
            for(int i = start; i < subtitlesUnits.Count - 1 && subtitlesUnits[i].BeginTime == subtitlesUnits[i + 1].BeginTime && subtitlesUnits[i].EndTime == subtitlesUnits[i + 1].EndTime && subtitlesUnits[i].Contents.Count == subtitlesUnits[i + 1].Contents.Count; i++)
            {
                totalWord += calcContentLens(subtitlesUnits[i]);
                end = i + 1;
            }
            totalWord += calcContentLens(subtitlesUnits[end]);
            for(int i = start; i <= end; i++)
            {
                if(i > start)
                {
                    subtitlesUnits[i].BeginTime = subtitlesUnits[i - 1].EndTime;
                }
                if(i < end)
                    subtitlesUnits[i].EndTime = subtitlesUnits[i].BeginTime + (uint)((double)totalTime * ((double)calcContentLens(subtitlesUnits[i]) / totalWord));
            }
        }
        /// <summary>
        /// 计算字符单元内内容的总字符数
        /// </summary>
        /// <param name="srtU">要计算的字符单元</param>
        /// <returns>字符单元内容的总字符数</returns>
        protected int calcContentLens(SubtitlesUnit srtU)
        {
            int length = srtU.Contents.Count, rtn = 0;
            for(int i = 0; i < length; i++)
            {
                rtn += srtU.Contents[i].Length;
            }
            return rtn;
        }

        /// <summary>
        /// 将单元字符串, 分成长度小于等于参数数字的几个小单元
        /// </summary>
        /// <param name="unit">要分割的单元字符产</param>
        /// <param name="wordNum">最多字符数</param>
        /// <returns></returns>
        protected List<string> breakDialogByNum(string unit, int wordNum)
        {
            List<string> tempList = new List<string>();
            unit = unit.Trim();
            int textLens = unit.Length;
            int breakNum = Convert.ToInt32(Math.Ceiling((double)textLens / wordNum));
            for(int i = 0; i < breakNum; i++)
            {
                int start = i * wordNum;
                if(start + wordNum <= textLens)
                    tempList.Add(unit.Substring(start, wordNum));
                else
                    tempList.Add(unit.Substring(start));
            }

            return tempList;
        }
        #endregion

        #region 事件监听

        #endregion
    }
}
