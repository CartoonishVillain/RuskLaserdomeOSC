### Rusk Laserdome OSC

This is a module that reimplments the laserdome avatar functionality of LD/Dead and LD/Team for those who want to continue using the interactive elements on their avatars.

This module currently works for Laserdome 1 and Laserdome 2.

This module depends on VRCOSC, found here: https://vrcosc.com/

You'll also need to enable logging in Vrchat, under the debug settings. 

We have a test world here, if you don't want to pool folks together to test in a live arena: https://vrchat.com/home/world/wrld_3b191e33-5262-444c-9259-d6d3a6beb5fc/info

### Installation Options
1. Download this repository and build the module yourself. If you're successful, it'll show up as an unknown local VRCOSC module. Probably the "Safest" way of installation (I have no malicious intent, but for those who feel "better safe than sorry")
2. Download the latest release DLL, and place it in the VRCOSC folder (default windows directory: %appdata%/VRCOSC/packages/local)
3. Install through the VRCOSC app: On the first tab on the left of the app, you can install Rusk Laserdome OSC directly by hitting the plus button.


### Usage
This module, by default, assumes a default path for the VRChat logs. In the future I may add a non-standard path reading feature.

When installed, ensure the module is enabled, in the settings check the "Toggle Log Reader" button.

Notice: It's recommended to activate this module early in VRChat's life time. It continously scans log entries you haven't seen before, it could take some time to catch up, and may even be resource intensive (unverified) if it has a lot of logs to parse through!

Once the log reader is on, and log messages are caught up with, you're done! Any avatar with the standard LD/Dead and LD/Team parameters should work more or less how they did prior to vrchat breaking the station system.
