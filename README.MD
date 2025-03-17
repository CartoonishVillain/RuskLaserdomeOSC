### Rusk Laserdome OSC

This is a module that reimplments the laserdome avatar functionality of LD/Dead and LD/Team for those who want to continue using the interactive elements on their avatars.

This module currently works for Laserdome 1 but not Laserdome 2. If Cirrex uses the same logging schema in Laserdome 2 later it will connect fine, otherwise I'll patch it later. But for  now it's not possible from my end.

This module depends on VRCOSC, found here: https://vrcosc.com/

We also have a test world here, if you don't want to pool folks together to test in a live arena: https://vrchat.com/home/world/wrld_3b191e33-5262-444c-9259-d6d3a6beb5fc/info

Feel free to compile it as a VRCOSC module yourself, or you can install it by:
Todo: Make proper installation steps.


### Usage
When installed, ensure the module is enabled, in the settings check the "Toggle Log Reader" button.

Notice: It's recommended to activate this module early in VRChat's life time. It continously scans log entries you haven't seen before, it could take some time to catch up, and may even be resource intensive (unverified) if it has a lot of logs to parse through!

Once the log reader is on, and log messages are caught up with, you're done! Any avatar with the standard LD/Dead and LD/Team parameters should work more or less how they did prior to vrchat breaking the station system.
