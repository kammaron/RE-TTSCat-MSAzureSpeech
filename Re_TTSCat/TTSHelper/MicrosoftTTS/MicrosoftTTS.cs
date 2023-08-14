// Created by kammaron@bilibili kammaron@github
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using Re_TTSCat.Data;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

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
                text = text.Replace(" ", "");

                var config = SpeechConfig.FromSubscription(Vars.CurrentConf.MSSpeechKey, Vars.CurrentConf.MSSpeechRegion);
                config.SpeechSynthesisVoiceName = Vars.MSVoiceMap[Vars.CurrentConf.MSVoice];// "zh-CN-XiaoshuangNeural";
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz128KBitRateMonoMp3);
                var speed = ConverNumToPercent(Vars.CurrentConf.ReadSpeed);
                var pitch = ConverNumToPercent(Vars.CurrentConf.SpeechPitch);

                using (var speechSynthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    var ssml = Vars.MSSSMLExample;
                    ssml = ssml.Replace("$SPEED$", speed);
                    ssml = ssml.Replace("$PITCH$", pitch);
                    ssml = ssml.Replace("$VOICE$", Vars.MSVoiceMap[Vars.CurrentConf.MSVoice]);
                    ssml = ssml.Replace("$CONTENT$", text);
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
