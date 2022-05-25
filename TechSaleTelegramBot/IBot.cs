using System.Threading.Tasks;

namespace TechSaleTelegramBot
{
    public interface IBot
    {
        IBotHost Host { get; set; }
        Task SendMessage(string msg, string chatId);
    }
}