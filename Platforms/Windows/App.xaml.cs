using Microsoft.UI.Xaml;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace JCA.Mobile.WinUI;

public partial class App : MauiWinUIApplication
{
	public App()
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}