using Re_TTSCat.Windows;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Re_TTSCat.Data
{
    public static class Vars
    {
        public static readonly string ApiBaidu = "https://fanyi.baidu.com/gettts?lan=zh&text=$TTSTEXT&spd=5&source=web";
        public static readonly string ApiBaiduCantonese = "https://fanyi.baidu.com/gettts?lan=cte&text=$TTSTEXT&source=web";
        public static readonly string ApiYoudao = "http://tts.youdao.com/fanyivoice?word=$TTSTEXT&le=zh&keyfrom=speaker-target";
        public static readonly string ApiBaiduAi = "https://tsn.baidu.com/text2audio?tex=$TTSTEXT&lan=zh&per=$PERSON&spd=$SPEED&pit=$PITCH&cuid=1234567JAVA&ctp=1&tok=$TOKEN";
        public static readonly string AppDllFileName = Assembly.GetExecutingAssembly().Location;
        public static readonly string AppDllFilePath = (new FileInfo(AppDllFileName)).DirectoryName;
        public static readonly string ConfDir = Path.Combine(AppDllFilePath, "RE-TTSCat");
        public static readonly string DownloadUpdateFilename = Path.Combine(ConfDir, "Re_TTSCat_update.zip");
        public static readonly string DefaultCacheDir = Path.Combine(ConfDir, "Cache");
        public static readonly string CacheDirTemp = Path.Combine(Path.GetTempPath(), "Re-TTSCat TTS Cache");
        public static readonly string ConfFileName = Path.Combine(ConfDir, "Config.json");
        public static readonly string AudioLibraryFileName = Path.Combine(ConfDir, "NAudio.dll");
        public static readonly Version CurrentVersion = new Version("3.10.2.726");
        public static readonly string ManagementWindowDefaultTitle = "Re: TTSCat - 插件管理";
        public static readonly string MSSSMLExample = "<speak xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xmlns:emo=\"http://www.w3.org/2009/10/emotionml\" version=\"1.0\" xml:lang=\"zh-CN\"><voice name=\"$VOICE$\"><s /><mstts:express-as style=\"$STYLE$\"><prosody rate=\"$SPEED$\" pitch=\"$PITCH$\">$CONTENT$</prosody></mstts:express-as><s /></voice></speak>";
        public static readonly string[] MSVoiceMap =
            {
                "zh-CN-XiaoxiaoNeural",
                "zh-CN-YunxiNeural",
                "zh-CN-YunjianNeural",
                "zh-CN-XiaoyiNeural",
                "zh-CN-YunyangNeural",
                "zh-CN-XiaochenNeural",
                "zh-CN-XiaohanNeural",
                "zh-CN-XiaomengNeural",
                "zh-CN-XiaomoNeural",
                "zh-CN-XiaoqiuNeural",
                "zh-CN-XiaoruiNeural",
                "zh-CN-XiaoshuangNeural",
                "zh-CN-XiaoxuanNeural",
                "zh-CN-XiaoyanNeural",
                "zh-CN-XiaoyouNeural",
                "zh-CN-XiaozhenNeural",
                "zh-CN-YunfengNeural",
                "zh-CN-YunhaoNeural",
                "zh-CN-YunxiaNeural",
                "zh-CN-YunyeNeural",
                "zh-CN-YunzeNeural",
                "zh-CN-henan-YundengNeural",
                "zh-CN-liaoning-XiaobeiNeural",
                "zh-CN-shaanxi-XiaoniNeural",
                "zh-CN-shandong-YunxiangNeural",
                "zh-CN-sichuan-YunxiNeural",
                "yue-CN-XiaoMinNeural",
                "yue-CN-YunSongNeural",
                "zh-HK-HiuMaanNeural",
                "zh-HK-WanLungNeural",
                "zh-HK-HiuGaaiNeural",
                "zh-TW-HsiaoChenNeural",
                "zh-TW-YunJheNeural",
                "zh-TW-HsiaoYuNeural",
                "wuu-CN-XiaotongNeural",
                "wuu-CN-YunzheNeural"
            };

        public static Conf CurrentConf = new Conf();
        public static Thread Player;
        public static uint RoomCount;
        public static uint TotalPlayed = 0;
        public static uint TotalFails = 0;
        public static bool SystemSpeechAvailable = false;
        public static string SpeechUnavailableString = "";
        public static bool CallPlayerStop = false;
        public static bool HangWhenCrash = false;
        public static bool UpdatePending = false;
        public static OptionsWindow ManagementWindow;
        public static string CacheDir => CurrentConf?.SaveCacheInTempDir == false ? DefaultCacheDir : CacheDirTemp;
        public static string ApiBaiduAiAccessToken = string.Empty;
        public static GiftDebouncer Debouncer = new GiftDebouncer();
    }
}
