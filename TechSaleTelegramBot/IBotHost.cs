using System.Threading.Tasks;

namespace TechSaleTelegramBot
{
    public interface IBotHost
    {
        void Register(IBot bot);
        Task RespondToMessage(string chatId, string userName);
    }
}