# VR Stage Lighting - Changelog
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



