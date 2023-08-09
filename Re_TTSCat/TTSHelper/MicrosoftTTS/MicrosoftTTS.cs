// ** CUSTOM ENGINE IS IN ALPHA, STABILITY NOT GUARANTEED**

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Newtonsoft.Json;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class MicrosoftTTS
    {
        public static async Task<string> Download(string content)
        {
            var errorCount = 0;
        Retry:
            try
            {
                var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "USER.mp3");

                return fileName;
            }
            catch (Exception ex)
            {
                Bridge.ALog($"(E7) TTS 下载失败: {ex.Message}");
                errorCount += 1;
                Vars.TotalFails++;
                if (errorCount <= Vars.CurrentConf.DownloadFailRetryCount)
                {
                    goto Retry;
                }
                return null;
            }
        }
    }
}
