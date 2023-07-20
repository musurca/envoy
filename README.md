# Envoy
![screenshot](https://i.imgur.com/8Pmolh3.jpg)
**[DOWNLOAD LATEST RELEASE (v1.04)](https://github.com/musurca/envoy/releases/download/1.04/Envoy.v1.04.zip)**

**Envoy** is a utility for [Wargame Design Studio](http://www.wargameds.com)’s series of historical wargames that
simulates command and control with order delay. Often, decisive battles turned not on
weapons and men, but on time and information: who knew what, and when? Were the correct
orders issued—and received in time to make a difference?

Using **Envoy**, you can explore these questions by composing orders for subordinates,
then passing them down the chain of command with realistic (and customizable) delay. You will
be notified on the turn when the order is delivered.

As a solo player, you can use this tool as a prompt for moving your units along
constraints defined by the orders. You can also use it to umpire Kriegspiel-style wargames for
multiple players, using the underlying WDS game to adjudicate combat.

## How do I use it?

A full [manual](https://github.com/musurca/envoy/blob/master/manual/Envoy_Manual_v10.pdf) is included.

## Compatibility

**Envoy** is currently compatible with the WDS Musket & Pike, Napoleonic, Early American Wars, and Civil War Battles series. The Panzer & Modern Campaigns series are not currently supported. 

PBEM games are supported, as long as they are unencrypted. (In the WDS game, *Settings -> PBEM Encryption* should be unchecked.)

## Version History

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
