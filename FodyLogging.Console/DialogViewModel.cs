using System.Threading.Tasks;

namespace FodyLogging.Console;

public class DialogViewModel: ViewModelBase
{
    protected TaskCompletionSource<bool> CloseTask = new();
    
    
    public bool IsDialogOpen { get; set; }


    public async Task WaitAsync()
    {
        await CloseTask.Task;
    }

    public void Show()
    {
        if(CloseTask.Task.IsCompleted)
            CloseTask = new TaskCompletionSource<bool>();
        
        IsDialogOpen = true;
    }
    
    public void Close()
    {
        IsDialogOpen = false;   
        CloseTask.TrySetResult(true);
    }
    
}