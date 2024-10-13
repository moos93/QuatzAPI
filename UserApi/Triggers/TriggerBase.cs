using UserApi.Models;

namespace UserApi.Triggers
{
    public abstract class TriggerBase
    {
        public abstract Task ExecuteAsync(User user);
    }
}
