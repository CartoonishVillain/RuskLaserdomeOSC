using System.IO;
using System.Text;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.VRChat;

namespace RuskLaserdomeOSC
{
    [ModuleTitle("RuskLaserdomeOSC")]
    [ModuleDescription("Rusk Co's Laserdome Compatibility Module")]
    [ModuleType(ModuleType.Integrations)]
    public class RuskLaserOSC : Module
    {

        bool logReading = false;
        bool playerDead = false;
        DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat");
        int fileCheckCounter = 200;
        FileInfo? currentLog = null;
        long lastCountedLine = 0;
        int team = 0;

        protected override void OnPreLoad()
        {
            base.OnPreLoad();
            CreateToggle(RuskLaserOSCSetting.ToggleLogReader, "Toggle Log Reader", "Log Reading could possibly somewhat intensive, if a lot of logs need to be scanned initially. But having this off will disable functionality outright.", logReading);
        }

        private enum RuskLaserOSCSetting
        {
            ToggleLogReader
        }

        protected override Task<bool> OnModuleStart()
        {
            directoryInfo.Refresh();
            currentLog = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
            lastCountedLine = 0;
          //  Log(currentLog.FullName + "- Log file set to this file.");
            logReading = false;
            playerDead = false;
            return Task.FromResult(true);
        }

        protected override void OnAvatarChange(AvatarConfig? avatarConfig)
        {
            base.OnAvatarChange(avatarConfig);
            SendParameter("LD/Dead", playerDead);
            SendParameter("LD/Team", team);
        }


        //The file reading here heavily takes notes from VRCX's repository.
        //Because they are under the MIT license, here is that below.
        // Copyright(c) 2019-2022 pypy, Natsumi and individual contributors.
        // All rights reserved.
        //
        // This work is licensed under the terms of the MIT license.
        // For a copy, see <https://opensource.org/licenses/MIT>.


        // as well as a link to their repo and file that I referenced:
        //https://github.com/vrcx-team/VRCX/blob/634f465927bfaef51bc04e67cf1659170953fac9/LogWatcher.cs

        [ModuleUpdate(ModuleUpdateMode.Custom, false, 50)]
        private void onUpdate()
        {
            if (GetSettingValue<bool>(RuskLaserOSCSetting.ToggleLogReader))
            {
                //This ensures that we have the latest log file.
                if (fileCheckCounter > 0 || currentLog == null)
                {
                    fileCheckCounter -= 1;
                }
                else
                {
                    fileCheckCounter = 200;
                    directoryInfo.Refresh();
                    currentLog = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
               //     Log(currentLog.FullName + "- Log file set to this file.");
                }

                //If we do have a log file, read it.
                if (currentLog != null)
                {
                    // local var to track player death, ideally to avoid a case where a player somehow switches avatars mid parse and maybe causes undefined behavior.
                    var lastDead = playerDead;
                    var lastTeam = team;

                    using (var stream = File.Open(currentLog.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    { 
                        //Only read lines we haven't read already by setting the position to already be where we left off last.
                        stream.Position = lastCountedLine;
                        using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                        {
                            while (true)
                            {
                                var line = streamReader.ReadLine();
                                if(line == null)
                                {
                                    //At the end of the read, mark down the last line we read, so we can pick up there next time.
                                  //  Log("File Opened Successfully!" + "\nprevious line count: " + lastCountedLine +"\nNew line count: " + stream.Position);
                                    lastCountedLine = stream.Position;
                                    break;
                                }

                                if (line.Length == 0)
                                {
                                    continue;
                                }

                                // Alive and Dead log checks
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
                                if (line.Contains("[AvatarInteraction] Team changed to 0") || line.Contains("[AvatarInteraction] Team changed to {0}"))
                                {
                                    lastTeam = 0;
                                    Log("Team Signal - 0 - null team?");
                                }
                                if (line.Contains("[AvatarInteraction] Team changed to 1") || line.Contains("[AvatarInteraction] Team changed to {1}"))
                                {
                                    lastTeam = 1;
                                    Log("Team Signal - 1 - Free For All");
                                }
                                if (line.Contains("[AvatarInteraction] Team changed to 2") || line.Contains("[AvatarInteraction] Team changed to {2}"))
                                {
                                    lastTeam = 2;
                                    Log("Team Signal - 2 - Red team");
                                }
                                if (line.Contains("[AvatarInteraction] Team changed to 3") || line.Contains("[AvatarInteraction] Team changed to {3}"))
                                {
                                    lastTeam = 3;
                                    Log("Team Signal - 3 - Pink team");
                                }
                                if (line.Contains("[AvatarInteraction] Team changed to 4") || line.Contains("[AvatarInteraction] Team changed to {4}"))
                                {
                                    lastTeam = 4;
                                    Log("Team Signal - 4 - Blue team");
                                }
                                if (line.Contains("[AvatarInteraction] Team changed to 5") || line.Contains("[AvatarInteraction] Team changed to {5}"))
                                {
                                    lastTeam = 5;
                                    Log("Team Signal - 5 - Green team");
                                }
                            }
                        }
                        stream.Close();
                    }

                    //Reconcile the final results of the Death and Team checks, and push them to the Avatar Parameters
                    if(playerDead != lastDead)
                    {
                        playerDead = lastDead;
                        SendParameter("LD/Dead", playerDead);
                    }
                    if(team != lastTeam)
                    {
                        team = lastTeam;
                        SendParameter("LD/Team", team);
                    }
                }
            }
            else
            {
                Log("Log File Not Found!");
            }
        }
    }
}
