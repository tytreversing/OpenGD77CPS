using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace DMR.Properties;

[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
[CompilerGenerated]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance;

	public static Settings Default => defaultInstance;

	static Settings()
	{
		defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
