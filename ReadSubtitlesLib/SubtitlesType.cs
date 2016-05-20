/* ==============================
* Author: jicemoon
* QQ: 375114086
* E-mail: jicemoon@outlook.com
* Time：2015/12/9 17:39:41
* FileName：SubtitlesType
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
    public class SubtitlesType
    {
        /// <summary>
        /// 和当前字幕同类型
        /// </summary>
        public const uint Type_Current = 0;
        /// <summary>
        /// SRT字幕
        /// </summary>
        public const uint Type_SRT = 1;
        /// <summary>
        /// ASS字幕
        /// </summary>
        public const uint Type_ASS = 2;
    }
}
