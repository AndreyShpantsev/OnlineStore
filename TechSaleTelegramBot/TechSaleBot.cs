using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TechSaleTelegramBot
{
    public class TechSaleBot : IBot
    {
        public IBotHost Host { get; set; }

        private readonly TelegramBotClient telegramBot;

        public TechSaleBot(string token)
        {
            telegramBot = new TelegramBotClient(token);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message
                }
            };

            telegramBot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions);
        }

        public async Task SendMessage(string msg, string chatId)
        {
            await telegramBot.SendTextMessageAsync(chatId, msg);
        }

        async Task HandleUpdateAsync(
            ITelegramBotClient botClient, 
            Update update, 
            CancellationToken cancellationToken
        )
        {
            if (update.Message!.Type != MessageType.Text)
                return;

            await Host.RespondToMessage(update.Message.Chat.Id.ToString(), update.Message.From.Username);
        }

        Task HandleErrorAsync(
            ITelegramBotClient botClient, 
            Exception exception, 
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }
    }
}