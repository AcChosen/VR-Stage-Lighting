# VR Stage Lighting - Changelog

## 2.8.0 Change Log - May 21, 2024

- Applied a bunch of community PRs! Here's a list of them:

- Adjustments for OpenGL and SPS-I configured devices - fundale
- Fixed wrong hard-coded index in unrolled array on VRStageLighting_AudioLink_Laser.cs - KitKat4191
- Update C# scripts to support compiling outside of the UdonSharp context. - techanon & fundale
- Force RequireDepthLight and VolumetricNoise off on Android -  CompuGenius-Programs
- Added URP Shader Support - techanon

## 2.7.0 Change Log - June 2nd, 2024
- Packaged important add-ons such as video players, gpu readback, and screen space shaders into unitypackages along with main VR Stage Light package to reduce confusion and keep things organized.
- Converted shaders and video players to all run on native linear input. This removes many issues and inconsistencies with smoothing and general movement.
- Updated GPU readback to fire events on value change when past a certain threshold for float and int paramater ranges. Added ability to name data points in GPU Readback function script.

## 2.6.2 Change Log - May 21st 2024
- Added component sliders to the VRSL DMX and VRSL AudioLink scripts. These sliders replace the final intensity sliders with sliders that control the intensities of the indivdual components of a light (The volumetric, projection, and fixture mesh).
- Added "Genereate Unique VRSL Materials" menu button to the VRSL drop down menu under "Utilties". This button will search the scene for any VRSL Materials, make a copy of them, save them in your selected folder, and apply them to your VRSL meshes in your scene as well as to the Local UI Control Panel. This will allow users to seperate their VRSL materials from the prefab materials and applying it to the scene, allowing for users to make changes to their VRSL materials without updates resetting their settings. (The AudioLink component sliders are at the bottom of the script.)
- Added the "VRSL GI" toggle prefab toggle to Control Panel. This will allow one to spawn the VRSL GI variants of the standard VRSL prefabs. (VRSL GI 1.2+ Only).

## 2.6.1 Change Log - April 6th 2024
- Added global DMX strobe toggle on Local Control Panel asset. Toggling this will disable the strobe channel for the player locally. Useful for those that want to enjoy the light show but can't handle the strobing.
- Adjusted the volumetrics 3D noise to scale properly with the mesh. Scaling a spotlight or strobe up should no longer result in wonky noise tiling.
- Fixed build errors with the DMX patch exporter scripts (again).

## 2.6.0 Change Log - March 25th 2024
- Created Fixture Definition Files and added Fixture Type properties to DMX fixtures. Fixture Definition files are used to create arbitrary data to describe DMX fixture channels. They are mostly cosmetic and used for the DMX patch data exporter.
- Added the ability to save and load DMX Patch Data per scene. This is useful for restoring DMX patch settings to scenes due to corruption.
- Added the ability to export the patch data as a JSON, MVR (My Virtual Rig), or PDF Patch List. 
- Added VRSL Manager Window Settings file. This file will start to hold per project data about the VRSL Control Panel. Right now, it holds references to alternate meshes for spotlight and washlight volumetrics.
-Added the ability to switch the volumetric meshes in a scene to lower poly versions on the fly.
-Improved the spin timer shader to have more consistent and smooth DMX gobo spinning.

## 2.5.1 Change Log - February 29th 2024

- Fixed inspector related bugs and improved inspector stability on all VRSL component scripts.
- Added prefab load toggles for VRSL GI prefabs into Control Panel (VRSL GI 1.2.0+)
- Increased AudioLink Delay range from 30 to 127

## 2.5.0 Change Log - February 9th, 2024
- Removed the USharp Video and uDesktopDuplication DMX screen prefabs and placed them in their own packages along with new VideoTXL and ProTV DMX screen prefabs.
- Added ability to add and remove DMX Screen prefab packages. TekOSC is the only DMX reader prefab that now comes pre-installed with VRSL, removing USharp Video as a requirement for VRSL.
- Reduced the precision of majority of the DMX Render Textures to 16-Bit SFLOAT instead of 32-Bit SFLOAT to reduce VRAM usage.
- Auto set all DMX Render Textures to have a period of 0.0 to reduce editor related issues.
- Removed the bloom post processing volume prefab from the VRSL-LocalUIControlPanel prefab.
-  VRSL-LocalUIControlPanel will now hide the bloom slider if it cannot find the bloom post processing volume prefab or it's "Bloom Animator" variable is null.
- Fixed bug where you could not edit multiple VRSL scripts at once.

## 2.4.5 Change Log
- Re-Added Texture Sampling features for the AudioLink Discoball.
- Added the "Singal Detection System" for interpolation CRT shaders. This system will allow users to disable VRSL DMX if pixels that are not related to the VRSL DMX Grid node are detected (in cases of the video player freezing or disconnecting or an invalid video format was inputted.) This system is disabled by default but can be enabled in the inspector of any of the interpolation CRTs.

## 2.4.4 Change Log
- Added per fixture toggle to disable black-to-white conversion when texture sampling mode is enabled on audiolink fixtures.

## 2.4.3 Change Log
- Added Global Intensity Blend property to all shaders.
- Removed all `.Blend`and `.Blend1` files

## 2.4.2 Change Log
- Changed Movement smoothing algorithm to be framerate-independent.
- Added smoothing multiplier that works in VRChat only to better match smoothing values in Editor. 
- Added extra strobe render textures to reduce the delay between GI and VRSL strobing updates.
- Made version numbers consistent across all editor scripts.
- Fixed package format. (Again)

## 2.4.1 Change Log
- Added ability to switch between dynamic and static strobing for DMX fixtures in the settings of the Strobe Timings CRT. This will switch strobe channels from the smooth variable speed strobing channel to a 3 speed consistent strobe channel. The 3 speed strobing will be slightly more performant and should also appear more consistent at lower frame rates.
- Added the ability to quickly make changes to any of the CRT materials for the DMX and AudioLink Custom Render Textures inside of the VRSL Control Panel. It is listed under options and allows users to view and edit the materials that the custom render textures use. This will allow for adjusting global dmx settings such as range of smoothing for movement on moving heads as well as the maximum and minmum strobe speeds.
- Moved some files around in preperation for the VRSL GI release.

# NOTE: This version of VRSL has changed ALOT of the backend (under-the-hood). If you are production sensitive project, please continue to use [v2.2](https://github.com/AcChosen/VR-Stage-Lighting/releases/tag/v2.2.0) until you feel your project is ready to update to 2.4. If not, please update to this version as soon as you can!~

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



