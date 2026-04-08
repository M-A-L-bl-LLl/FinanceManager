namespace FinanceManager.UI.ViewModels;

public class MessageDialogViewModel
{
    public string Title { get; init; } = "";
    public string Message { get; init; } = "";
    public string Icon { get; init; } = "InformationOutline";
    public string IconColor { get; init; } = "#7C4DFF";
    public string ConfirmText { get; init; } = "ОК";
    public string? CancelText { get; init; }
    public bool ShowCancel => CancelText is not null;
}
