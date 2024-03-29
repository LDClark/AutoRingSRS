using EsapiEssentials.Plugin;
using AutoRingUAB;
using VMS.TPS.Common.Model.API;

[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script : ScriptBase
    {
        public override void Run(PluginScriptContext context)
        {
            var esapiService = new EsapiService(context);

            using (var ui = new UiRunner())
            {
                ui.Run(() =>
                {
                    var window = new MainWindow();
                    var dialogService = new DialogService(window);
                    var viewModel = new MainViewModel(esapiService, dialogService);
                    window.DataContext = viewModel;
                    window.ShowDialog();
                });
            }
        }
    }
}

