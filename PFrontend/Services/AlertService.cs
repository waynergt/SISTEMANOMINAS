namespace PFrontend.Services
{
    public class AlertService
    {
        public event Func<string, AlertType, Task>? OnShow;
        public event Func<Task>? OnHide;

        public async Task ShowAlert(string message, AlertType type = AlertType.Info)
        {
            if (OnShow != null)
                await OnShow.Invoke(message, type);
        }

        public async Task HideAlert()
        {
            if (OnHide != null)
                await OnHide.Invoke();
        }

        public enum AlertType { Success, Error, Warning, Info }
    }
}