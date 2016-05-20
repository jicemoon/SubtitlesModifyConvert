using System.Windows;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ReadSubtitlesLib;

namespace SortSRT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            //ReadASS ra = new ReadASS();
            //ra.readSubtitles(@"F:\download\BaiduYunDownload\Everest.2015.720p.BluRay.x264-SPARKS.ass");
            //ReadSubtitles rs = ra.convertTo(SubtitlesType.Type_SRT);
            //rs.writeSubtitles(@"F:\download\BaiduYunDownload\Everest.2015.720p.BluRay.x264-SPARKS.srt");
            //convertFiles(@"D:\jicemoon\Desktop\[]", @"D:\jicemoon\Desktop\[]\[]", SubtitlesType.Type_SRT,true,true);
            convertFiles(@"G:\[电视剧]\[疑犯追踪][S04][720p][BluRay][DEMAND]\[ass]", @"G:\[电视剧]\[疑犯追踪][S04][720p][BluRay][DEMAND]", SubtitlesType.Type_SRT, true, false);
        }
        private void convertFiles(string inFolder, string outFolder, uint targetType = SubtitlesType.Type_SRT, bool lineFormat = false, bool all = false)
        {
            string[] files = Directory.GetFiles(inFolder, "*.ass");
            string end = targetType == SubtitlesType.Type_ASS ? ".ass" : ".srt";
            Console.WriteLine("文件个数: " + files.Length);
            for(int i = 0; i < files.Length; i++)
            {
                ReadSubtitles rs = ReadSubtitles.ConvertTo(files[i], targetType);
                if(lineFormat)
                {
                    rs.deleteLineFormat(all);
                }
                rs.resortSrt(true);
                rs.writeSubtitles(outFolder + @"\" + Path.GetFileNameWithoutExtension(files[i]) + end);
                //break;
            }
        }


        private void operation_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        /// <summary>
        /// 遍历文件夹内的所有文件
        /// </summary>
        /// <param name="fold">需要遍历的文件夹</param>
        /// <param name="files">将遍历到的文件列表输出到此List对象中</param>
        private void traversalFold(DirectoryInfo fold, List<FileInfo> files, string patter)
        {
            files.AddRange(fold.GetFiles(patter));
            DirectoryInfo[] tempfold = fold.GetDirectories();

            foreach(DirectoryInfo next in tempfold)
            {
                traversalFold(next, files, patter);
            }
        }
    }
}
