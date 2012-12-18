AppleWirelessKeyboard for Wired Keyboards
=========================================

This is a simple modification of the original AppleWirelessKeyboard helper 
utility by [uxsoft](http://uxsoft.cz/projects/applewirelesskeyboard/) which
also includes some basic support for the wired variant of the Apple Keyboard.

This utility isn't really useful or needed if you're running BootCamp on a real
Mac. Just stick with the Apple BootCamp stuff.

Usage
-----

This is basically just the exact same AppleWirelessKeyboard tool with a few
minor bug fixes and two new config options. The new config options are:

* F keys are Mac special function keys by default [on/off]
* Wired Keyboard. If enabled, fixes Eject key handling on a wired keyboard so that it toggles the above option on/off when pressed [on/off]

Unfortunately, as I don't own an Apple wireless keyboard I did not add any kind
of hardware auto-detection to toggle the second "Wired Keyboard" config option
on or off as needed. For now you will need to set this option manually.

When the "Wired Keyboard" option is turned off, everything will (should) work
just as it did with the original version of this utility. This means things
won't be quite perfect for you if you are using a wired keyboard and not a 
wireless one.

When the "Wired Keyboard" option is turned on, the Eject key will be fixed to
work the same way as it does with a wireless keyboard when using the original 
version of this utility. That is, it is used to toggle if the function keys (F1 
through F12) work as on Windows or OS X.

This tool does not make the 'Fn' key work with a wired keyboard. It just
gives you a simple work around. If you are using a wired keyboard none of the
'Fn' key combinations will work regardless of if you're using this version or the
original version of this utility.

Why?
----

The original AppleWirelessKeyboard utility is not able to read the state of the 
'Fn' key at all when using a wired keyboard. This _appears_ to be a driver issue, 
but I'm by no means an expert on low-level driver stuff in Windows so maybe it 
is possible to fix this. It doesn't seem to matter if you're using the default 
Windows provided driver, or the Apple driver from Lion's BootCamp driver 
collection... the 'Fn' key remains unreadable via this code.

Using the Apple driver with the wired keyboard, at least for me, only seems to 
get you half-way there. Next/Previous/Play Fn key combos work, but the volume 
control key combos don't. Plus you don't get the nice OSD icons when you press 
them. And again, even with the Apple driver installed, this utility is still not
able to read the state of the 'Fn' key.

Basically, I could find no solution at all which worked the way I wanted it to
with my wired keyboard.

After playing with the original code for AppleWirelessKeyboard from 
[CodePlex](http://applewirelesskbrd.codeplex.com/) I noticed that the Eject key
was still recognized somewhat. Just in a slightly different way then how the
original code was set up to recognize it. So, with some minor code tweaks, I
was able to fix the code so that the Eject key would work with wired keyboards 
to allow F key toggling like with a wireless keyboard.

Issues / Missing Functionality
------------------------------

* The code using iTunesLib has been commented out. I don't use iTunes nor do I have it installed, therefore I don't have a copy of this library so I couldn't build the code until I removed it. I guess it's possible that the media key mappings for F7-F9 could maybe work with iTunes? No idea.
* No automatic detection of which keyboard you are using (wired or wireless). I'd like to do this, but don't have a wireless keyboard for reference. You will need to set your config manually.
* 'Fn' key combos still do not work with a wired keyboard. Use the Eject key to toggle F key functionality.
* The "Restart" option is gone. This doesn't have a built-in equivalent in WPF.

Bug Fixes
---------

* Fixes virtual key codes used for Home, End, Page Up and Page Down keys (the Fn + Arrow combos available on wireless keyboards).
* Config settings are now saved. When you re-open the tool your last settings will still be there.
* The system tray icon now disappears when the tool is closed and the tool now closes properly (the "WPF way").
