using System.Collections.Generic;

namespace WebApplicationTechSale.HelperServices
{
    public static class RedirectionMessageProvider
    {
        public static List<string> AntiquesAcceptedMessages()
        {
            return new List<string>()
            {
                "Модерация прошла успешно",
                "Антиквариат опубликован и теперь виден всем пользователям.",
                "Сейчас вы будете перенаправлены на страницу с антиквариатами"
            };
        }

        public static List<string> AntiquesRejectedMessages()
        {
            return new List<string>()
            {
                "Вы отклонили публикацию антиквариата",
                "Пользователь получит уведомление и сможет отредактировать данные для повторной публикации.",
                "Сейчас вы будете перенаправлены на страницу c антиквариатами",
            };
        }

        public static List<string> AntiquesCreatedMessages()
        {
            return new List<string>()
            {
                "Антиквариат успешно создан",
                "Сейчас антиквариат будет отправлен на модерацию. Модератор примет решение о публикации. Проверяйте статус антиквариата в личном кабинете",
                "Если вы подписаны на нашего Telegram-бота, то будете уведомлены о публикации",
                "Сейчас вы будете перенаправлены на главную страницу"
            };
        }

        public static List<string> AntiquesUpdatedMessages()
        {
            return new List<string>()
            {
                "Данные обновлены",
                "Антиквариат повторно отправлен на модерацию. Проверяйте статус антиквариата в личном кабинете",
                "Если вы подписаны на нашего Telegram-бота, то будете уведомлены о публикации",
                "Сейчас вы будете перенаправлены на главную страницу"
            };
        }

        public static List<string> AccountCreatedMessages()
        {
            return new List<string>()
            {
                "Регистрация прошла успешно",
                "Сейчас вы будете перенаправлены на главную страницу нашего сайта"
            };
        }

        public static List<string> OrderCreateMessage()
        {
            return new List<string>()
            {
                "Поздравляем с покупкой!!!",
                "Сейчас вы будете перенаправлены на страницу со списком Ваших покупок"
            };
        }

        public static List<string> AccountUpdatedMessages()
        {
            return new List<string>()
            {
                "Изменения сохранены",
                "Сейчас вы будете перенаправлены в личный кабинет"
            };
        }
    }
}