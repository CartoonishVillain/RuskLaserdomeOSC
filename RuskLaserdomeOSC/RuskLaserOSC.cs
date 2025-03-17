using System.IO;
using System.Text;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.VRChat;

namespace RuskOSCModule
{
    [ModuleTitle("RuskLaserOSC")]
    [ModuleDescription("Rusk Co's Laserdome Compatibility Module")]
    [ModuleType(ModuleType.Integrations)]
    public class RuskLaserOSC : Module
    {

        bool logReading = false;
        bool playerDead = false;
        DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat");
        int fileCheckCounter = 20;
        FileInfo? currentLog = null;
        long lastCountedLine = 0;

        protected override void OnPreLoad()
        {
            currentLog = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
            base.OnPreLoad();
            CreateToggle(RuskLaserOSCSetting.ToggleLogReader, "Toggle Log Reader", "Log Reading can be somewhat intensive. This will also disable functionality outright.", logReading);
        }

        private enum RuskLaserOSCSetting
        {
            ToggleLogReader
        }

        protected override Task<bool> OnModuleStart()
        {
            logReading = false;
            playerDead = false;
            return Task.FromResult(true);
        }

        protected override void OnAvatarChange(AvatarConfig? avatarConfig)
        {
            base.OnAvatarChange(avatarConfig);
            SendParameter("LD/Dead", playerDead);
        }

        [ModuleUpdate(ModuleUpdateMode.Custom, false, 50)]
        private void onUpdate()
        {
            bool isActive = GetSettingValue<bool>(RuskLaserOSCSetting.ToggleLogReader);
       
            if (isActive)
            {
                if (fileCheckCounter > 0 || currentLog == null)
                {
                    fileCheckCounter -= 1;
                }
                else
                {
                    fileCheckCounter = 20;
                    currentLog = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                }

                if (currentLog != null)
                {
                    var lastDead = playerDead;

                    using (var stream = File.Open(currentLog.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.Position = lastCountedLine;
                        using(var streamReader = new StreamReader(stream, Encoding.UTF8))
                        {
                            while (true)
                            {
                                var line = streamReader.ReadLine();
                                if(line == null)
                                {
                                    lastCountedLine = stream.Position;
                                    break;
                                }

                                if (line.Length == 0)
                                {
                                    continue;
                                }

                                if (line.Contains("[AvatarInteraction] Alive"))
                                {
                                    lastDead = false;
                                    Log("Read Alive Signal!");
                                }
                                if (line.Contains("[AvatarInteraction] Dead"))
                                {
                                    lastDead = true;
                                    Log("Read Dead Signal!");
                                }
                            }
                        }
                        stream.Close();
                    }
                    playerDead = lastDead;
                    SendParameter("LD/Dead", playerDead);
                }
            }
            else
            {
                Log("Log File Not Found!");
            }
        }
    }
}
