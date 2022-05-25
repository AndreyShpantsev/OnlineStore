using DataAccessLogic;
using DataAccessLogic.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechSaleTelegramBot;

namespace WebApplicationTechSale.HelperServices
{
    public class BotHostService : BackgroundService, IBotHost
    {
        private readonly IBot telegramBot;
        private readonly IServiceScopeFactory scopeFactory;

        public BotHostService(IBot bot, IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            telegramBot = bot;
            Register(telegramBot);
        }

        public void Register(IBot bot)
        {
            bot.Host = this;
        }

        public async Task RespondToMessage(string chatId, string userName)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(chatId))
            {
                throw new Exception("Пользователь не определен");
            }

            User existingUser = await context.Users.FirstOrDefaultAsync(user =>
            user.TelegramUsername == userName);
            if (existingUser == null)
            {
                await telegramBot.SendMessage("Пользователь не найден в базе данных сайта", chatId);
                return;
            }
            if (!string.IsNullOrWhiteSpace(existingUser.TelegramChatId))
            {
                await telegramBot.SendMessage("Вы уже подписались на уведомления", chatId);
                return;
            }
            existingUser.TelegramChatId = chatId;
            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
            await telegramBot.SendMessage("Теперь вы подписаны на уведомления", chatId);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
