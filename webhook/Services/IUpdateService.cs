using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace webhook.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
        Task Respond(Update update);
    }
}
