# VR Stage Lighting

![alt text](https://i.imgur.com/XJtiP9f.png)

## A collection of HLSL shaders, UdonSharp scripts, 3D models, prefabs, and assets designed to emulate the real control, quality, and complexity of professional stage lighting into VRChat in as many ways as possible.
<a href="https://youtu.be/3yaEsIQVaHA">
<p align="center">
  <img src="Documentation/VRSL-GitHubTrailer-Short.gif">
</p>
  </a>

VR Stage Lighting is a year-long project that started out as a means to research and develop a performant/reliable way to send DMX512 (Digital Multiplex with 512 pieces of information) data to VRChat to control spot lights and other lighting effects. It has evolved into creating a package of assets that can bring quality lighting effects in all manner of ways performantly. Those ways include:

- DMX512/ArtNet via Video/Stream Panel.
- Audio Reaction via AudioLink.
- A special "VR Card-Based System" powered by UdonSharp and Animations.
- Raw Udon input via UdonSharp.
- DMX512/ArtNet via OSC (Coming Soon!).

The majority of the overhead is powered through a standardized set of custom shaders that are designed to emulate different types lighting as well as input methods. This is provide performance by avoiding things such as real-time unity lights and using cost saving measures such as GPU-instancing and batching. The different light types that are included:

- Mover Spotlights
- Static Par Lights
- Static Blinder Lights
- Generic Static Lights
- Disco Ball
- Lazers (Coming Soon!)



## SEE ALL THESE SYSTEMS LIVE IN VRCHAT @ CLUB ORION (https://vrchat.com/home/launch?worldId=wrld_b2d9f284-3a77-4a8a-a58e-f8427f87ba79)

## IMPORTANT
- These systems are designed for world building on VRChat for PC. While some of the shaders in theory could be used on avatars, they are primarily designed to be placed in a PC world.
- This system is still a work in progress and is planned for long-term support as more VRChat features are released. Planned features include OSC support, lazers, and Quest-Ready alternatives.
- For proper documentation, please refer to the PDF documents provided on the release page.
- ### These lights and shaders are open source while the VR Stage Lighting Grid Node is not. While the VR Stage Lighting Grid Node is not required for the Non-DMX versions of the stage lights, purchasing a copy of the VR Stage Lighting Grid Node will help in the development of both the node grid and the VR Stage Lighting kit as well as give you full access to the DMX capabilities of the shaders! You can purchase a copy here: https://gumroad.com/l/xYaPu, and your support will be greatly appreciated! <3



## Setup

### Requirements
- Unity 2019.4
- VRChat SDK3 for worlds (Udon)
- UdonSharp
- Usharp Video Player (https://github.com/MerlinVR/USharpVideo)
- PostProcessing Stack V2
- uDesktopDuplication 1.6.0 (https://github.com/hecomi/uDesktopDuplication/releases/tag/v1.6.0) (for working with in-Editor; must be 1.60, Unity 2018.4 causes 1.70 to break for some reason)
- AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
- Open BroadCast Software (or streaming software of your choice)
- VR Stage Lighting Grid Node (For DMX control, more info below. Purchase it here: https://gumroad.com/l/xYaPu)
- Atleast one extra screen that can support 16:9 resolutions.
- The latest release: 


### Installation
1. Install VRChat SDK3, UdonSharp, Usharp Video Player, PostProcessing Stack V2, uDesktopDuplication 1.6.0, and Audio Link. Finally, install the latest release of VR Stage Lighting.
2. Have a look at any of the available example scenes. The inspectors of the shaders and scripts will have ToolTips about what they do. Proper documentation for each system will also be included in the same folder.

<p align="center">
  <img width="497" height="480" src="Documentation/VRSL-LightDemonstration.gif">
</p>

### Getting started (DMX VIA Stream Panel through Editor)
1. Drag "VRSL-DMX-uDesktopDuplicationReader" prefab into the scene.
2. Add some DMX Compatible Light Fixture Prefabs (VRStageLighting-Exporter\Prefabs\DMX).
3. Assign Sectors to them through their Udon Scripts. (0 to 35)
4. Open your Artnet compatible software of choice. (Recommendation: SoundSwitch)
5. Open "VRStageLightingGridNode.exe" (Get it here: https://gumroad.com/l/xYaPu)
6. Ensure VRStageLightingGridNode is a visible Artnet device and ensure its getting Universes 1 and 2.
6. Open OBS.
7. Set "VRStageLightingGridNode.exe" as a window source. Place perfectly in the bottom right corner of your screen.
8. Make sure your OBS stream resolution is set to a 16:9 ratio.
9. Set the stream as a full screen window preview on your extra screen.
10. Press play in Unity. Ensure both the Game View and Scene View are both showing/are visible for the script to update properly.
11. In the "VRSL-DMX-uDesktopDuplicationReader", go to the script called "Texture" and set the display to your extra screen with the OBS Preview on it.
12. Ensure the screen is orientated properly and test out some DMX inputs.
13. Read the documentation for more information and channels and other things.


### Getting started (DMX VIA Stream Panel through USharp Video Player)
1. Drag "VRSL-DMX-USharpVideoReader" prefab into the scene.
2. Add some DMX Compatible Light Fixture Prefabs (VRStageLighting-Exporter\Prefabs\DMX).
3. Assign Sectors to them through their Udon Scripts. (0 to 35)
4. Hide the child object named "DMXReaderScreen" somewhere away where visitors can't see.
5. Place the "USharpVideo" child object where you want on your map
6. Upload and test in VRchat. This test video in the playlsit will load automatically: https://www.youtube.com/watch?v=YrIOvLbHPgo&ab_channel=AirConditioning


### Getting Started (Audio React via Audio Link)
1. Follow the "Getting Started" instructions provided with the Audio Link package.
2. Add the desired AudioLink compatible VRSL Fixtures when ready.


### Getting Started (VRSL Card-Based System)
1. Start a scene and place "VRSL-Animated" fixture prefabs where you want them (located in \VRSL-VRCardSystem\Prefabs).
2. Drop in the "VRSL-StageLighting-CardSystem-Prefab" into your world.
3. For each lighting group, in the script, add the lighting fixtures you want that group to control under the "Stage Light List" array in the inspector.
4. If you need more groups, just duplicate one, recolor it to your liking, and add your fixtures. Afterwards, go to the "Master Intensity Slider Handle" object and add your new light groups to the appropriate list.
5. Uplaod and test.  



## ABOUT

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



## Shaders
The shaders themselves are hand-written vertex-frag shaders designed to loosely emulate real-time stage lighting without having the performance cost. The only real-time light needed is the default directional light (with optimal settings applied, examples are included) for use of the depth texture. There are 3 types of shaders that are seperated into Mover and Static categories.

- Fixture (The stage light's housing)
- Volumetric (The volumetric beam that the light would produce in a foggy/smoky setting)
- Projection (The actual light being projected on to other objects. This does not use the standard Unity Projector Component despite it's name.)

More information on them can be found in the Shaders folder as well the tooltips on the shader properties in the inspector.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

## DMX512/ArtNet Via Video/Stream Panel

<p align="center">
  <img src="Documentation/NewNodeGridSample-SectorExample.png">
</p>

The main system this project was based on. This system can convert signals it reads from a VRChat stream/video panel into actions that the lights can perform, mainly a special black/white grid that coverts DMX signals into something the lighting system can read. This system is powered 95% by shaders, including the actual method of reading the pixels from the screen. The other 5% is to enable GPU instancing for the shaders and certain properties. There is also basic "RAW" Udon support for when DMX Support is disabled for these shaders.

Summary for the pipeline:

- The grid sits in the bottom right corner of a stream/video that has a 16:9 aspect ratio (I.E 1920x1080, 1280x720, etc.).
- The grid runs in the background and is discoverable Artnet device, which allows Artnet compatible software to send it DMX data via local Ethernet.
- When the grid gets the DMX data, it displays its value as a black/white range with 0/1 representing the values respectively.
- A camera picks ups the grid in the bottom right corner of the screen and converts it into a render texture.
- The render texture is converted into two custom render textures, one to add realtime smoothing to compensate for compression, and another to add phase compensation for the strobe channel.
- The custom render textures are then sent to the lighting fixtures, where the signals are to be read and converted into actions.
-The channels of the grid start from the bottom left corner (channel 1) counting up as you move right, starting a new row every 26 channels. Every 13 channels is considered a "Sector" and is the maximum number of channels any fixture here can use. 
- Every fixture is assigned a Sector and will only read the channels from that respective sector. The fixture may or may not use every channel depending on the type, but this will allow multiple fixtures to read from the same Sector.
- The current channel types are Pan (Regular + Fine), Tilt (Regular + Fine), Brightness/Intensity, Red, Green, Blue, Strobe, SpotLight Radius, GOBO (Cookie) and GOBO Spin Speed (Coming Soon!). More Channel types will be added as more fixture types are created.
- Some special channels are also created by default including a dedicated Disco Ball channel and a "Mover Speed Channel" (which controls the smoothing strength of all mover lights).


The appeal of this system is that this allows any software or hardware that supports Artnet to be able to control these lights in VRChat in real time through a stream or a recorded video. This repository comes with an example recorded video in an example scene of the lights in action as well as the grid system they're reacting to. The video is placed in an example scene where the same lights are set-up to re-react to the video in real time, mimicing the actions of the lights shown in the example video.



### Limitations 
- This system requires a video/stream panel, meaning there will be some unavoidable latency for realtime setups.
- Compression artifacting can cause movement data to be scrambled a bit. Linear Interpolated CRTs are applied to compensate for the scrambling, but does make the movement much slower. A dedicated sector has been set to control the intensity of the smoothing/interpolation, but it is recommended to keep the smoothing at maximum (Which is 0 for that channel) for most situations. 
- If quicker movement is needed, it is recommended to do it in small bursts, quickly returning the smoothing back to maximum when you can. This channel controls the movement for all lights. It is currently not possible to control the speed of individual light/sectors with this system.
- The system currently supports a little over 35 unique fixtures (sectors). Support for more is planned for the future.

### Software
- Recommended software to use this system with is SoundSwitch, which allows a DJ to play pre-created sync'd light shows in realtime with whatever track is playing as well as other DJ friendly light show options. You can get SoundSwitch here! (https://www.soundswitch.com/)

- The grid itself can be purchased at my gumroad, which will also help directly support the development of this project <3! Which can be purchased here! () OSC output is also included with this grid system for when VRChat officially supports it via Udon. You can get it here!: https://gumroad.com/l/xYaPu

<p align="center">
  <img src="Documentation/VRSl-Kradt-ComeWithMe-RecDemo-Short.gif">
</p>
  </a>

### Channel List Per Sector (1-13)
Here are the channels and what they represent for each Sector (set of 13):

1. Pan (Left/Right Rotation)
2. Fine-Pan
3. Tilt (Up/Down Rotation)
4. Fine-Tilt
5. SpotLight Radius/Cone Width (Will show up as "Motor Speed" in some software)
6. Intensity
7. Strobe (0-009 is off, 010-255 is slow to fast)
8. Red Intensity
9. Green Intensity
10. Blue Intensity
11. Unused (Labled as "White Intensity" in some software)
12. GOBO selection (1 through 6; Labled as "Programmes" in some software)
13. GOBO Spin Speed (Work-In-Progress; Labled as "Speed/Sensativity");


### Tips
- There are a limited number of available sectors, so try to be effecient with the number of unique fixtures you set up. You can have an unlimted number of fixtures reading from the same sector!
- Movers can have their pan/tilt inverted! This allows for mirrored movement between multiple movers that share the same sector!
- Video Tutorials/tips for this system will be released soon, including how to integrate with SoundSwitch!
- Use uDesktopDuplication (by hecomi) to see your screen appear in realtime in your unity editor! You can use this to test your lights in realtime with the grid as well as create stage lighting scenes and shows! Get it here!: https://github.com/hecomi/uDesktopDuplication/releases/tag/v1.6.0

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


## Audio Reaction via AudioLink

A varation of these shaders support AudioLink by llealloo.

As described by llealloo, "AudioLink is a system that analyzes the frequencies of in-world audio and exposes the amplitude data to VRChat Udon, world shaders, and avatar shaders."

These shaders will have their intensity's react to the audio at different frequencies. They use the shader implementation of AudioLink for minimal overhead. There is also basic "RAW" Udon support that is enabled alongside AudioLink which inclues GPU Instanced properties and mover target following. 

An example scene is included that show the different light types reacting to the different frequency bands of audio.

You can get Audiolink as well as learn more about it here!: (https://github.com/llealloo/vrc-udon-audio-link)

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


## VRSL Card-Based System (UdonSharp + Animations)

<p align="center">
  <img src="Documentation/VRSL-CardSystemShowcase.gif">
</p>
  </a>
A custom stage lighting system that is designed around controlling the lights within VR itself in a manner that combines elements of a real stage lighting hardware and card games. The standard "RAW" perks such as batching and GPU instancing are used here as well.


### Cards
The card based system uses positionally synced orbs with cards in them to control groups of lights at a time by putting the orbs in special "Lighting Zones". These "Lighting Zones" will read information from the card and tell whatever lights it has control over what corresponding animations to play. There are 4 types of cards, with each type controlling a different aspect of the lights.

- Pan/Tilt Cards (Movers Only)
- GOBO Cards (SpotLights Only)
- Color Cards
- Intensity Cards

- Pan/Tilt Cards control how mover fixtures move around. They are yellow and only "Mover" type fixtures will respond to them.
- GOBO Cards control what GOBO/Cookie is being projected by the spotlight. They are green and only "Spotlight" type fixtures will respond to them.
- Color Cards control what colors the lights are displaying. They are blue and all lights support them.
- Intensity Cards control the intenisty/flashing of the lights. They are red and all lights support them.

The cards contain the title of animation, an icon describing what it does, and the number of measures/bars the animation will last.
There are plans for more card types and animations in the future.

### Universal Metronome
The animations are controlled by a "Universal Metronome" or "BPMCounter" which counts a standard 4 beats at the desired tempo. Every 1st beat, a network event is launched to ensure everyone is in sync. The metronome controls the speed of the animations, which will allow the animations to always keep a constant rhthmic time as well as be in sync with everyone in the instance without heavy network load. (This system may be adjusted to accomoadate for the networking update in the future). Which animations are playing are controlled by which ever card is placed into the box.

### Sliders
Other aspects of the lights such as the base brightness and radius of the spotlights are controlled with global physical sliders that are also positionally synced. 

### Example

An example scene is included with 4 "Lighting Zones" and a handful of cards. Further documentation/instructions can be found here: (https://docs.google.com/document/d/1iwmyHbZ442YsSOQxok_zZtQNxBHFgy6Tu0USKSNC9DE/edit?usp=sharing). The documentation is also included in the example scene.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

## "RAW" Input Via UdonSharp

This is the bare bones setup for these shaders. They contain a light weight version of the shaders that are GPU instanced through a compatible Udon Script. This setup is for people who want to create their own methods of controlling the lights via Udon and serves as the basis for the 3 other systems.
The basic features it comes with include:

- GPU instanced properties/variables and methods to update them.
- Target Following for movers.
- Custom Inspectors.

Future systems will also derive from this set of scripts.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

## Local UI Panel

A UI panel that can control the intensity of the different lighting shaders is also included. Plop this panel in you world to allow users to locally control the brightness of each aspect of the lights, or all lights at once. It also includes a slider for bloom intensity and a pre-made post processing volume for it. This panel is included as a prefab and is also featured in each example scene.

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

## Support

If you'd like to support the project, you can do so via patreon, where you can also get some VRSL exclusives!~
https://www.patreon.com/ac_chosen
You can also join the official VRSL discord here!: 
https://discord.gg/zPktZAe48r
