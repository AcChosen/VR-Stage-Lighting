These are the prefabs for the AudioLink variants of VRSL and a short description of each. These prefabs support AudioLink including bands and delay.

Fixtures that have the word "Static" will be the most performant version of that fixture. This is because there are no update loops running on the script. The difference between the variants that have "Static" and the ones that don't will be explained below.

Audio-UnClean (ToHearAudioProperly) - A copy of the audio that audiolink is reacting to that is meant to be audible to the player.

VRSL-AudioLinkControllerWithSmoothing - A special version of the AudioLink control panel with an extra panel that allows to control the interpolated smoothing on the VRSL Edition of Audiolink.

AudioLink - VRSL Edition - A special version of AudioLink that uses the interpolated version of the AudioLink shader. The interpolation will work for both the lights and any other AudioLink compatible shaders/objects.

VRSL-AudioLink-Static-DiscoBall - The VRSL Discoball with AudioLink support on the intensity. AudioLink is off by default.

VRSL-AudioLink-Static-Blinder - The VRSL Blinder with AudioLink support on intensity. Has support for Color Chord as well as Texture Color Sampling.

VRSL-AudioLink-Static-LightBar - The VRSL Light Bar with AudioLink support on intensity. Has support for Color Chord as well as Texture Color Sampling.

VRSL-AudioLink-Static-ParLight - The VRSL Par Light with AudioLink support on intensity. Has support for Color Chord as well as Texture Color Sampling.

VRSL-AudioLink-Static-SpotLight - The VRSL Spot Light with AudioLink support on intensity. Has support for Color Chord as well as Texture Color Sampling. This is the "Static" variant. This means that it will not have the ability to follow a "target". The Pan/Tilt of this light can still be controlled with the appropriate variables though. Animations will not work very well with this version of the fixture.

VRSL-AudioLink-Mover-SpotLight - The VRSL Spot Light with AudioLink support on intensity. Has support for Color Chord as well as Texture Color Sampling. This is the "Mover" variant.This means that it will have the ability to follow a "target" by having a target transform variable and enabling "Follow Target". Use this if you want to animate the fixture. 




