# About the Oculus XR Plugin

The Oculus XR Plugin enables you to build applications for a variety of Oculus devices including the Rift, Rift S, Quest 2, Quest Pro, and Quest 3.

## Supported XR plugin subsystems

### Display 

The display subsystem provides stereo rendering support for the XR Plugin. It supports the following graphics APIs:

* Windows
    * DX11
* Android
    * OpenGL ES 3.0
    * Vulkan

### Input 

The input subsystem provides controller support, haptics, and tracking for the controllers and HMD.

## XR Management support

Integration with XR Management isn't required to use the Oculus XR Plugin, but it provides for a simpler and easier way of using this and other Providers within Unity. The Oculus XR Plugin package ships with built-in XR Management support. For more information, see [XR Management Documention](https://docs.unity3d.com/Packages/com.unity.xr.management@latest)

The Oculus XR Plugin integration with XR Management provides the following functionality:

* **Runtime Settings** - Configure runtime settings such as rendering modes, depth buffer sharing, Dash support, etc.
* **Lifecycle Management** - The Oculus XR Plugin ships with a default XR Plugin loader implementation that handles subsystem lifecycle such as application initialization, shutdown, pausing, and resuming.

### Windows standalone settings

* **Stereo Rendering Mode** - You can select *Multi Pass* or *Single Pass Instanced* stereo rendering mode.
    * *Multi Pass* - Unity renders each eye independently by making two passes across the scene graph. Each pass has its own eye matrices and render target. Unity draws everything twice, which includes setting the graphics state for each pass. This is a slow and simple rendering method which doesn't require any special modification to shaders.
    * *Single Pass Instanced* - Unity uses a texture array with two slices, and uses instanced draw calls (converting non-instanced draws call to instanced versions when necessary) to direct rendering to the appropriate texture slice.  Custom shaders need to be modified for rendering in this mode.  Use Unity's XR shader macros to simplify authoring custom shaders. 
* **Shared Depth Buffer** - Enable or disable support for using a shared depth buffer. This allows Unity and Oculus to use a common depth buffer, which enables Oculus to composite the Oculus Dash and other utilities over the Unity application.
* **Dash Support** - Enable or disable Dash support. This inintializes the Oculus Plugin with Dash support, which enables the Oculus Dash to composite over the Unity application.

### Android settings

* **Stereo Rendering Mode** - You can select *Multi Pass* or *Multiview* stereo rendering mode.
    * *Multi Pass* - Unity renders each eye independently by making two passes across the scene graph. Each pass has its own eye matrices and render target. Unity draws everything twice, which includes setting the graphics state for each pass. This is a slow and simple rendering method which doesn't require any special modification to shaders.
    * *Multiview* - Multiview is essentially the same as the *Single Pass Instanced* option described above, except the graphics driver does the draw call conversion, requiring less work from the Unity engine. As with *Single Pass Instanced*, shaders need to be authored to enable Multiview.  Using Unity's XR shader macros will simplify custom shader development.
* **Low Overhead Mode** - If enabled, the GLES graphics driver will bypass validation code, potentially running faster. Disable this if you experience graphics instabilities. GLES only.
* **Optimize Buffer Discards** - If enabled, the depth buffer contents will be discarded rather than resolved and the MSAA color buffer will be resolved rather than stored after rendering. This is a performance optimization that can possibly break rendering effects that sample from the depth buffer, such as camera stacking. Vulkan only.
* **Phase Sync** - This enables a latency optimization technique which can reduce simulation latency by several ms, depending on application workload. Note: this is always enabled when using the Oculus OpenXR Runtime.
* **Symmetric Projection** - If enabled, this allows the application to render with symmetric projection matrices. This can improve GPU performance when using multiview due to more common workloads between the left and right eye. Supported when using Vulkan and Multiview.
* **Subsampled Layout** - If enabled, the eye textures will use a subsampled layout. When using FFR, the subsampled layout will improve app GPU performance and reduce FFR-related visual artifacts. However, this feature will slightly increase the GPU cost of Timewarp. Therefore, we only recommend enabling it if the app is using FFR level 2 or higher, in which case, the app GPU performance improvement should outweigh the extra Timewarp cost. Vulkan only.
* **Foveated Rendering Method** - Choose which foveated rendering method is used when foveation is enabled.
    * *Fixed Foveated Rendering* - Foveates the image based on a fixed pattern.
    * *Eye Tracked Foveated Rendering* - Foveates the image using eye tracking. Only supported on Quest Pro with proper permissions and when using Vulkan, Multiview, and ARM64.
* **Enable TrackingOrigin Stage Mode** - When enabled, if the Tracking Origin Mode is set to Floor, the tracking origin won't change with a system recenter.
* **Depth Submission** - Enables support for submitting the depth buffer on mobile. This enables depth testing between layers on Oculus mobile platforms.
* **System Splash Screen** - You can add a PNG file under the Assets folder as the system splash screen image. If set, the OS will display the system splash screen as a high quality compositor layer as soon as the app is starting to launch until the app submits the first frame.
* **Late Latching** - This feature reduces tracked rendering latency by updating head and controller poses as late as possible before rendering. Vulkan only.
* **Late Latching Debug Mode** - Enables a debug mode for Late Latching which will print information about the Late Latching system as well as any errors. This can be used to verify that Late Latching is performing correctly. Debug mode is only active in Development builds.
* **Application SpaceWarp** - Enables support for a frame synthesis technology to allow your application to render at half frame rate, while still delivering a smooth experience. Note that this currently requires a custom version of the URP provided by Oculus in order to work, and should not be enabled if you aren't using that customized Oculus URP package.

## Technical details

### Fixed-Foveated Rendering (FFR)

Support for [fixed-foveated rendering](https://developer.oculus.com/documentation/quest/latest/concepts/mobile-ffr/) to provide better performance for [pixel-fill limited](https://en.wikipedia.org/wiki/Fillrate) applications. Controlling the level of foveation is made available through APIs in the Oculus XR Plugin.

FFR works best when rendering directly into the *eye textures* using the [foward rendering mode](https://docs.unity3d.com/Manual/RenderTech-ForwardRendering.html).  [*Deferred rendering* mode](https://docs.unity3d.com/Manual/RenderTech-DeferredShading.html), which is characterized by rendering into an intermediate render texture, is not recommended for use with FFR. This situation arises often when using the default *Universal Rendering Pipeline*, which includes a blit operation by default at the end of the frame. 
