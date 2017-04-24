using System;

#if __ANDROID__
namespace SkiaSharp.Views.Android
#elif __TVOS__
namespace SkiaSharp.Views.tvOS
#elif __IOS__
namespace SkiaSharp.Views.iOS
#elif __DESKTOP__ || __WPF__
namespace SkiaSharp.Views.Desktop
#elif WINDOWS_UWP
namespace SkiaSharp.Views.UWP
#elif __MACOS__
namespace SkiaSharp.Views.Mac
#endif
{
    public class SKPaintGLSurfaceEventArgs : EventArgs
	{
		public SKPaintGLSurfaceEventArgs(SKSurface surface, GRBackendRenderTargetDesc renderTarget)
		{
			Surface = surface;
			RenderTarget = renderTarget;
		}

		public SKSurface Surface { get; private set; }

		public GRBackendRenderTargetDesc RenderTarget { get; private set; }
	}
}
