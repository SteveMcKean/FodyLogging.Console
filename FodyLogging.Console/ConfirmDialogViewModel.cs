using Prism.Commands;

namespace FodyLogging.Console;

/// <summary>
/// ViewModel for a confirmation dialog.
/// </summary>
public partial class ConfirmDialogViewModel : DialogViewModel
{
    public bool Confirmed { get; set; }
    public DelegateCommand ConfirmCommand { get; private set; }
    public DelegateCommand CancelCommand { get; private set; }

    public ConfirmDialogViewModel()
    {
        InitializeCommands();
    }

    /// <summary>
    /// Initializes the commands for the dialog.
    /// </summary>
    private void InitializeCommands()
    {
        ConfirmCommand = new DelegateCommand(Confirm);
        CancelCommand = new DelegateCommand(Cancel);
    }

    /// <summary>
    /// Confirms the dialog and closes it.
    /// </summary>
    protected virtual void Confirm()
    {
        Confirmed = true;
        Close();
    }

    /// <summary>
    /// Cancels the dialog and closes it.
    /// </summary>
    protected virtual void Cancel()
    {
        Confirmed = false;
        Close();
    }
}