using Telegram.Bot;

namespace webhook.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}