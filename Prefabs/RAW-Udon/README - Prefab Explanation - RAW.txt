These are the prefabs for the RAW variants of VRSL and a short description of each. These prefabs are the stripped down versions of the fixtures with only basic Udon suppport.

Fixtures that have the word "Static" will be the most performant version of that fixture. This is because there are no update loops running on the script. The difference between the variants that have "Static" and the ones that don't will be explained below.

VRSL-RAW-Static-DiscoBall - The VRSL Discoball.

VRSL-RAW-Static-Blinder - The VRSL Blinder.

VRSL-RAW-Static-LightBar - The VRSL Light Bar. Has support for Texture Color Sampling.

VRSL-RAW-Static-ParLight - The VRSL Par Light. Has support for Texture Color Sampling.

VRSL-RAW-Static-SpotLight - The VRSL Spot Light. Has support for Texture Color Sampling. This is the "Static" variant. This means that it will not have the ability to follow a "target". The Pan/Tilt of this light can still be controlled with the appropriate variables though. Animations will not work very well with this version of the fixture.

VRSL-RAW-Mover-SpotLight - The VRSL Spot Light. Has support for Texture Color Sampling. This is the "Mover" variant. This means that it will have the ability to follow a "target" by having a target transform variable and enabling "Follow Target". Use this if you want to animate the fixture. 

