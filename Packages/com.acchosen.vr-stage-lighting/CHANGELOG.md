# VR Stage Lighting - Changelog

## v2.3.0 - February 15th, 2023
### New Features
- All DMX Grid textures are now global textures set with VRCShader.SetGlobalTexture(), which gives all shaders in a scene/instance access to all 4 DMX Grid textures at all times. This allows for custom VRSL shaders to be created without having to manage a bunch of render textures in the material's inspector. This also allows avatar shaders to have a wider range of channels to read from and allows a much more performant way of getting the textures to the avatars without relying on a grabpass.
- Avatar testing scenes have been created to test your VRSL DMX compatible avatar using pre-recorded video of arbitrary 5-channel dmx data. This should make it easier to test things without having to run a full DMX setup.

### Changes and Bugfixes
- The UsharpVideo DMX reader used a version of standard shader on the reading screen that was slightly affected by enviormental lighting, which sometimes affected the data the camera was trying to read. This has been fixed by replacing the shader with a properly unlit one.




## v2.2.0 - November 30th, 2022
### New Features
- Created this changelog!
- VR Stage Lighting is now a VPM package to be used with the VRChat creator companion. This is now the officially supported way of installing VR Stage Lighting.
- Added scene based depth light dependency toggle in control panel. Allows VR Stage Lighting shaders to become "Quest Compatible" by removing the reliance on the depth texture to function. This feature is also functional with PC.
- Added scene based 3D noise toggle in control panel. Allows ability to toggle the 3D and 2D noise features of volumetric shaders to reduce overhead on the fragment shaders. Useful for improving performance on Quest.
- Added support for AudioLink Theme Colors on all audiolink compatible lights.
- Added color chord support for audiolink disco ball.
- Added VRSL Stencil Masks. These 3D meshs will block or "mask" any and all VRSL projection and volumetric shaders. This is useful for preventing VRSL lights from leaking into adjacent rooms and hallways.

### Changes and Bugfixes
- This version of VRSL now requires AudioLink 0.3.0 or newer.
- Fixed the audiolink washlight volumetric from clipping into the fixture.
- Added a proper spawn button for the "VRSL Edition" of the AudioLink Prefab.



