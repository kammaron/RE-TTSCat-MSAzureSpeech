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
        public static async Task<string> Download(string content)
        {
            var errorCount = 0;
        Retry:
            try
            {
                var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "USER.mp3");
                var fileOutput = AudioConfig.FromWavFileOutput(fileName);
                Bridge.ALog($"(E5) 正在下载 TTS, 文件名: {fileName}, 方法: {Vars.CurrentConf.ReqType}");

                var config = SpeechConfig.FromSubscription("c32e4db1b5234902a80e16cb70a94c43", "eastasia");

                // Set the voice name, refer to https://aka.ms/speech/voices/neural for full list.
                config.SpeechSynthesisVoiceName = Vars.MSVoiceMap[Vars.CurrentConf.MSVoice];// "zh-CN-XiaoshuangNeural";
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz128KBitRateMonoMp3);
                string text = System.Web.HttpUtility.UrlDecode(content, Encoding.GetEncoding("UTF-8"));
                text = text.Replace(" ", "");

                using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
                {
                    using (var result = await synthesizer.SpeakTextAsync(text))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                                Console.WriteLine($"CANCELED: Did you update the subscription info?");
                            }
                        }
                    }
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
