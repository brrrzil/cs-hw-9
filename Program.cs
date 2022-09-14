// !! Для корректной работы в папке bin\Debug должны быть созданы папки \photo, \video, \audio и \other !!

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// Не забудьте вписать токен
var botClient = new TelegramBotClient(" ");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.FirstName}");

Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message.Photo != null)
    {
        var fileId = update.Message.Photo.Last().FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFilePath = $@"..\photo\{update.Message.Photo.Last().FileUniqueId}.jpg";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Фотография успешно загружена на сервер",
            cancellationToken: cancellationToken);
        return;
    }

    if (update.Message.Document != null)
    {
        var fileId = update.Message.Document.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFilePath = $@"..\photo\{update.Message.Document.FileName}";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Документ успешно загружен на сервер",
            cancellationToken: cancellationToken);
        return;
    }

    if (update.Message.Audio != null)
    {
        var fileId = update.Message.Audio.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFilePath = $@"..\audio\{update.Message.Audio.FileName}";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Аудиофайл успешно загружен на сервер",
            cancellationToken: cancellationToken);
    }

    if (update.Message.Voice != null)
    {
        var fileId = update.Message.Voice.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFileFolder = $@"..\audio\{update.Message.Voice.FileUniqueId}.mp3";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFileFolder);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Заметка успешно сохранена на сревер",
            cancellationToken: cancellationToken);
    }

    if (update.Message.Video != null)
    {
        var fileId = update.Message.Video.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFileFolder = $@"..\video\{update.Message.Video.FileName}";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFileFolder);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Видео успешно загружено на сервер",
            cancellationToken: cancellationToken);
    }

    if (update.Message.VideoNote != null)
    {
        var fileId = update.Message.VideoNote.FileId;
        var fileInfo = await botClient.GetFileAsync(fileId);
        var filePath = fileInfo.FilePath;

        string destinationFileFolder = $@"..\video\{update.Message.VideoNote.FileUniqueId}.mp4";
        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFileFolder);
        await botClient.DownloadFileAsync(filePath, fileStream);
        fileStream.Close();
        await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Видеозаметка успешно загружена на сервер",
            cancellationToken: cancellationToken);
    }

    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;    

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    // Клавиатура для скачивания
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] {"Фото", "Видео", "Аудио"},
        })
    { ResizeKeyboard = true };

    Message keyboardMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Выберите файл для скачивания",
        replyMarkup: replyKeyboardMarkup,
        cancellationToken: cancellationToken);

    if (message.Text.Contains("Фото"))
    {
        String path = ".." + @"\" + "photo" + @"\";
        var dir = Directory.GetFiles(path);
        foreach (var f in dir)
        {
            Message downloadMessage = await botClient.SendTextMessageAsync(chatId: chatId, text: $@"{f.Substring(9)}", cancellationToken: cancellationToken);
        }
    }    
    
    if (message.Text.Contains("Видео"))
    {
        String path = ".." + @"\" + "video" + @"\";
        var dir = Directory.GetFiles(path);
        foreach (var f in dir)
        {
            Message downloadMessage = await botClient.SendTextMessageAsync(chatId: chatId, text: $@"{f.Substring(9)}", cancellationToken: cancellationToken);
        }
    }

    if (message.Text.Contains("Аудио"))
    {
        String path = ".." + @"\" + "audio" + @"\";
        var dir = Directory.GetFiles(path);
        foreach (var f in dir)
        {
            Message downloadMessage = await botClient.SendTextMessageAsync(chatId: chatId, text: $@"{f.Substring(9)}", cancellationToken: cancellationToken);
        }
    }

    // Ниже тестовый код из уроков (кроме последней строчки). Не удалял, чтобы вы видели, что я это проходил)

    #region Повторение за пользователем
    // Echo received message text
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "You said:\n" + messageText,
    //    cancellationToken: cancellationToken);
    #endregion

    #region Ответ на сообщение пользователя с параметрами 

    //Message message2 = await botClient.SendTextMessageAsync(
    //chatId: chatId,
    //text: "Trying *all the parameters* of `sendMessage` method",
    //parseMode: ParseMode.MarkdownV2,
    //disableNotification: true,
    //replyToMessageId: update.Message.MessageId,
    //replyMarkup: new InlineKeyboardMarkup(
    //    InlineKeyboardButton.WithUrl(
    //        "Check sendMessage method",
    //        "https://core.telegram.org/bots/api#sendmessage")),
    //cancellationToken: cancellationToken);
    #endregion

    #region Вывод в консоль информации о сообщении
    //Console.WriteLine(
    //$"{message2.From.FirstName} sent message {message2.MessageId} " +
    //$"to chat {message2.Chat.Id} at {message2.Date.ToLocalTime()}. " +
    //$"It is a reply to message {message2.ReplyToMessage.MessageId} " +
    //$"and has {message2.Entities.Length} message entities."
    //);
    #endregion

    #region Отправка картинки
    //message = await botClient.SendPhotoAsync(
    //chatId: chatId,
    //photo: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
    //caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
    //parseMode: ParseMode.Html,
    //cancellationToken: cancellationToken);
    #endregion

    #region Отправка стикеров двумя способами
    ////по ссылке
    //message = await botclient.sendstickerasync(
    //    chatid: chatid,
    //    sticker: "https://github.com/telegrambots/book/raw/master/src/docs/sticker-fred.webp",
    //    cancellationtoken: cancellationtoken);

    ////взяв fileid первого сообщения
    //message message2 = await botclient.sendstickerasync(
    //    chatid: chatid,
    //    sticker: message.sticker.fileid,
    //    cancellationtoken: cancellationtoken);
    #endregion

    #region Отправка MP3 файла
    //message = await botClient.SendAudioAsync(
    //    chatId: chatId,
    //    audio: "https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3",
    ///*
    //performer: "Joel Thomas Hunger",
    //title: "Fun Guitar and Ukulele",
    //duration: 91, // in seconds
    //*/
    //cancellationToken: cancellationToken
    //);
    ////Console.WriteLine(message.Audio.FileId); //Узнать FileID
    #endregion

    #region Отправка голосового сообщения
    //Message message1;
    //using (var stream = System.IO.File.OpenRead(@"D:\Unity\SkillBox\HomeWork\C#\HW8\1\src_docs_voice-nfl_commentary.ogg"))
    //{
    //    message1 = await botClient.SendVoiceAsync(
    //        chatId: chatId,
    //        voice: stream,
    //        duration: 36,
    //        cancellationToken: cancellationToken);
    //}
    #endregion

    #region Отправка видео
    //message = await botClient.SendVideoAsync(
    //    chatId: chatId,
    //    video: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4",
    //    thumb: "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg",
    //    supportsStreaming: true,
    ////    replyToMessageId: update.Message.MessageId,
    //    cancellationToken: cancellationToken
    //);
    #endregion

    #region Отправка видео-заметки
    //Message message1;
    //using (var stream = System.IO.File.OpenRead(@"D:\Unity\SkillBox\HomeWork\C#\HW8\1\video-waves.mp4"))
    //{
    //    message1 = await botClient.SendVideoNoteAsync(
    //        chatId: chatId,
    //        videoNote: stream,
    //        duration: 47,
    //        length: 360,
    //        cancellationToken: cancellationToken);
    //}
    #endregion

    #region Отправка альбома фотографий
    //Message[] messages = await botClient.SendMediaGroupAsync(
    //    chatId: chatId,
    //    media: new IAlbumInputMedia[]
    //    {
    //        new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"),
    //        new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg")
    //    },
    //    cancellationToken: cancellationToken);
    #endregion

    #region Отправка документа
    //Message message1 = await botClient.SendDocumentAsync(
    //    chatId: chatId,
    //    document: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
    //    caption: "<b>ara bird. </b><i>Sourse: </i><a href=\"https://pixabay.com\">Pixabay</a>",
    //    parseMode: ParseMode.Html,
    //    cancellationToken: cancellationToken
    //    );

    #endregion

    #region Отправка анимации
    //Message message1 = await botClient.SendAnimationAsync(
    //    chatId: "@testbotmega",
    //    animation: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-waves.mp4",
    //    caption: "Waves",
    //    cancellationToken: cancellationToken);
    #endregion

    #region Отправка опроса
    //Message pollMessage = await botClient.SendPollAsync(
    //    chatId: "@testbotmega",
    //    question: "Ты пидор?",
    //    options: new[]
    //    {
    //        "Да",
    //        "Нет"
    //    },
    //    cancellationToken: cancellationToken);
    #endregion

    #region Остановка опроса
    //Poll poll = await botClient.StopPollAsync(
    //    chatId: pollMessage.Chat.Id,
    //    messageId: pollMessage.MessageId,
    //    cancellationToken: cancellationToken);
    #endregion

    #region Отправка контакта
    //Message message1 = await botClient.SendContactAsync(
    //    chatId: chatId,
    //    phoneNumber: "+79329360174",
    //    firstName: "Hideo",
    //    lastName: "Kojima",
    //    cancellationToken: cancellationToken);
    #endregion

    #region Клавиатура
    //    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    //    {
    //    new KeyboardButton[] {"3", "☎️"},
    //    })
    //    { ResizeKeyboard = true };

    //    Message message1 = await botClient.SendTextMessageAsync(
    //        chatId: chatId,
    //        text: ".",
    //        replyMarkup: replyKeyboardMarkup,
    //        cancellationToken: cancellationToken);
    #endregion

    #region Специальные кнопки
    //ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    //{
    //    KeyboardButton.WithRequestLocation("Share Location"),//Поделиться локацией
    //    KeyboardButton.WithRequestContact("Share Contact"),//Поделиться контактом
    //    KeyboardButton.WithRequestPoll("Share Request Poll")//Создать опрос
    //})
    //{ ResizeKeyboard = true };//Подогнать клавиатуру по высоте

    //sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "Who or Where are you?",
    //    replyMarkup: replyKeyboardMarkup,
    //    cancellationToken: cancellationToken);
    #endregion

    #region Удалить клавиатуру
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //chatId: chatId,
    //text: "Removing keyboard",
    //replyMarkup: new ReplyKeyboardRemove(),
    //cancellationToken: cancellationToken);
    #endregion

    #region Inline Keyboards
    #region Callback buttons
    //InlineKeyboardMarkup inlineKeyboard = new(new[]
    //{
    //    // first row
    //    new []
    //    {
    //        InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
    //        InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
    //    },
    //    // second row
    //    new []
    //    {
    //        InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
    //        InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
    //    },
    //});

    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: "A message with an inline keyboard markup",
    //    replyMarkup: inlineKeyboard,
    //    cancellationToken: cancellationToken);
    #endregion
    #region URL buttons
    //    InlineKeyboardMarkup inlineKeyboard = new(new[]
    //    {
    //            InlineKeyboardButton.WithUrl(
    //                text: "Link to the Repository",
    //                url: "https://github.com/TelegramBots/Telegram.Bot"
    //            )
    //        }
    //);

    //    Message sentMessage = await botClient.SendTextMessageAsync(
    //        chatId: chatId,
    //        text: "A message with an inline keyboard markup",
    //        replyMarkup: inlineKeyboard,
    //        cancellationToken: cancellationToken);
    #endregion
    #region Switch to inline buttons
    //    InlineKeyboardMarkup inlineKeyboard = new(new[]
    //    {
    //        InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"),
    //        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"),
    //    }
    //);

    //    Message sentMessage = await botClient.SendTextMessageAsync(
    //        chatId: chatId,
    //        text: "A message with an inline keyboard markup",
    //        replyMarkup: inlineKeyboard,
    //        cancellationToken: cancellationToken);
    #endregion
    #endregion
}
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
