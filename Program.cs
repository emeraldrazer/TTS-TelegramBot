#pragma warning disable
using System;
using System.Speech.Synthesis;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Requests;
using System.Diagnostics;

namespace Text_To_Speech
{
    class Program
    {
        private static TelegramBotClient? client;
        private static CancellationTokenSource cts;
        static void Main(string[] args)
        {
            DotNetEnv.Env.Load();

            string? token = Environment.GetEnvironmentVariable("BOT_TOKEN");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Error: BOT_TOKEN environment variable is not set.");
                return;
            }

            try
            {
                client = new TelegramBotClient(token);
                cts = new();

                ReceiverOptions receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = []
                };

                client.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cts.Token
                );

                Console.WriteLine("TTS Bot started successfully.");

                Process.GetCurrentProcess().WaitForExit();
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to start the bot. {ex.Message}");
            }
        }

        static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message != null)
                {

                    Message message = update.Message;

                    switch (message.Text)
                    {
                        case "/voices":
                            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
                            {
                                string msgToSend = string.Empty;
                                foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
                                {
                                    VoiceInfo info = voice.VoiceInfo;
                                    msgToSend += $"Name: {info.Name}\nCulture: {info.Culture}\nGender: {info.Gender}\nAge: {info.Age}\n\n";
                                }

                                await client.SendTextMessageAsync(message.Chat.Id, msgToSend, cancellationToken: cts.Token);
                                return;
                            }
                    }


                    string filePath = "output.wav";

                    using (SpeechSynthesizer synth = new())
                    {
                        synth.SetOutputToWaveFile(filePath);
                        synth.Speak(message.Text);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var cts = new CancellationTokenSource();
                        await client.SendDocumentAsync(
                            chatId: message.Chat.Id,
                            document: new Telegram.Bot.Types.InputFileStream(fileStream, filePath),
                            cancellationToken: cts.Token
                        );
                    }

                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Main.HandleUpdateAsync() Error -> {ex.Message}");
            }
        }

        static async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: Failed to start the bot. {exception.Message}");
        }
    }
}