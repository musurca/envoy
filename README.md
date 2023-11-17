# Envoy
![screenshot](https://i.imgur.com/qvhJfDo.jpg)
**[DOWNLOAD LATEST RELEASE (v1.11)](https://github.com/musurca/envoy/releases/download/1.11/Envoy_v1.11.zip)**

**Envoy** is a utility for [Wargame Design Studio](http://www.wargameds.com)’s series of historical wargames that simulates command and control with order delay. Often, decisive battles turned not on weapons and men, but on time and information: who knew what, and when? Were the correct orders issued—and received in time to make a difference?

Using **Envoy**, you can explore these questions by composing orders for subordinates, then passing them down the chain of command with realistic (and customizable) delay. You will be notified on the turn when the order is delivered.

As a solo player, you can use this tool as a prompt for moving your units along constraints defined by the orders. You can also use it to umpire Kriegspiel-style wargames for multiple players, using the underlying WDS game to adjudicate combat.

## How do I use it?

A full [manual](https://github.com/musurca/envoy/blob/master/manual/Envoy_Manual_v11.pdf) is included.

## Compatibility

**Envoy** is only compatible with the WDS Musket & Pike, Napoleonic, and Civil War Battles series. The Panzer & Modern Campaigns series are not currently supported. The Early American Wars series is not currently supported, but will likely work automatically after those games receive their post-4.02 update from WDS.

PBEM games are supported, as long as they are unencrypted. (In the WDS game, *Settings -> PBEM Encryption* should be unchecked.)

## Version History

v1.11 (11/17/2023)
-added: support for coalition armies in Thirty Years War
-added: support for coalition armies in Great Northern War
-added: support for coalition armies in The Renaissance
-added: preset for 17th century warfare
-fixed: support for coalition armies in Seven Years War

v1.1 (11/7/2023)
- added: history of dispatches sent & received
- added: custom sender & receiver
- added: support for hotseat battles
- added: digitally-signed executable via WDS
- changed: user can set name of dispatch file
- changed: dispatch file can be relinked to battle file if path is no longer valid
- changed: main window resizable
- fixed: chain of command for sender/recipient in different branches of hierarchy
- fixed: allied armies now appear correctly in the OOB
- fixed: parsing fixed for Windows regions using non-Western float notation

v1.04 (7/20/2023)
- added: OOB color-coding to indicate presence of units
- added: chance for additional delay to dispatches
- added: can change player-controlled army if not detected correctly
- fixed: various crashes related to OOB parsing in Civil War Battles
- fixed: parts of OOBs not being displayed

v1.03 (6/28/2023)
- fixed: bug with parsing some OOBs

v1.02 (6/26/2023)
- fixed: bug with Civil War Battles scenarios

v1.01 (6/26/2023)
- fixed: bug with off-map message recipients in a message chain
- fixed: bug with enemy ZOC evaluation with off-map units

v1.0 (6/25/2023)
- initial release
