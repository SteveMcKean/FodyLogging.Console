using System.Threading.Tasks;
using Prism.Dialogs;

namespace FodyLogging.Console.Services;

public static class DialogService
{
    public static async Task ShowDialog<THost, TDialogViewModel>(THost host, TDialogViewModel viewModel)
        where TDialogViewModel : DialogViewModel
        where THost : IDialogProvider
    {
        host.Dialog = viewModel;
        viewModel.Show();

        await viewModel.WaitAsync();
    }
}