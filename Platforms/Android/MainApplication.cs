using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace JCA.Mobile;

public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, jni.JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}