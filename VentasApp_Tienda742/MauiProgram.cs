using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

namespace VentasApp_Tienda742;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		QuestPDF.Settings.License = LicenseType.Community;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
