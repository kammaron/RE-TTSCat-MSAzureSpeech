// Created by kammaron@bilibili kammaron@github
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Re_TTSCat.Data;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Text.RegularExpressions;

namespace Re_TTSCat
{
    public static partial class MicrosoftTTS
    {
        private static string ConverNumToPercent(int num)
        {
            string ret = "";
            if (num >= 0) { ret += "+"; }

            num *= 10;
            ret += num.ToString();
            ret += "%";
            return ret;
        }

        private static int IdentifyVoiceByCommand(string str)
        {
            if (Regex.IsMatch(str, @":[!！].*-\d{1,2}$"))
            {
                string[] strArr = str.Split('-');
                string numOfVoice = strArr[strArr.Length - 1];
                int ret = 0;
                bool parseSucceed = int.TryParse(numOfVoice, out ret);
                if (parseSucceed && ret >=0 && ret < Vars.MSVoiceMap.Length)
                {
                    return ret;
                }
            }
            return -1;
        }

        private static string RemoveCommandFromText(string str)
        {
            str = str.Replace("!", "").Replace("！", "");
            int lastHyphen = str.LastIndexOf('-');
            if (lastHyphen >= 0)
            {
                return str.Substring(0, lastHyphen);
            }
            else
            {
                return str;
            }
        }

        public static async Task<string> Download(string content)
        {
            var errorCount = 0;
        Retry:
            try
            {
                var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "USER.mp3");
                var fileOutput = AudioConfig.FromWavFileOutput(fileName);
                Bridge.ALog($"(E5) 正在下载 TTS, 文件名: {fileName}, 方法: {Vars.CurrentConf.ReqType}");
                string text = System.Web.HttpUtility.UrlDecode(content, Encoding.GetEncoding("UTF-8"));
                text = text.Replace(" ", "").Trim();
                int voiceNum = IdentifyVoiceByCommand(text);
                if (voiceNum >= 0)
                {
                    text = RemoveCommandFromText(text);
                }

                var config = SpeechConfig.FromSubscription(Vars.CurrentConf.MSSpeechKey, Vars.CurrentConf.MSSpeechRegion);
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz128KBitRateMonoMp3);

                var speed = ConverNumToPercent(Vars.CurrentConf.ReadSpeed);
                var pitch = ConverNumToPercent(Vars.CurrentConf.SpeechPitch);
                var style = Vars.CurrentConf.MSVoiceStyle;
                var ssml = Vars.MSSSMLExample;
                var msStyleNodeBeg = "<mstts:express-as style=\"$STYLE$\">";
                var msStyleNodeEnd = "</mstts:express-as>";
                if (style == ""
                    || voiceNum != Vars.CurrentConf.MSVoice)
                {
                    ssml = ssml.Replace(msStyleNodeBeg, "");
                    ssml = ssml.Replace(msStyleNodeEnd, "");
                }
                else
                {
                    ssml = ssml.Replace("$STYLE$", style);
                }
                ssml = ssml.Replace("$SPEED$", speed);
                ssml = ssml.Replace("$PITCH$", pitch);
                if (voiceNum >= 0)
                {
                    ssml = ssml.Replace("$VOICE$", Vars.MSVoiceMap[voiceNum]);
                }
                else
                {
                    ssml = ssml.Replace("$VOICE$", Vars.MSVoiceMap[Vars.CurrentConf.MSVoice]);
                }
                ssml = ssml.Replace("$CONTENT$", text);

                using (var speechSynthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var result = await speechSynthesizer.SpeakSsmlAsync(ssml);
                }

                using (var reader = new AudioFileReader(fileName)) { }

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
