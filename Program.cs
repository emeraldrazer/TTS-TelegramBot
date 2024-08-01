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
        private static SpeechSynthesizer speech;
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
                speech = new SpeechSynthesizer();

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

                    if (message.Text.StartsWith("/setvoice"))
                    {
                        try
                        {
                            string wantedVoice = message.Text.Substring(10);

                            speech.SelectVoice(wantedVoice);
                            await client.SendTextMessageAsync(message.Chat.Id, $"Successfully changed voice to '{wantedVoice}'", cancellationToken: cts.Token);

                        }
                        catch (System.Exception ex)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Error setting desired voice", cancellationToken: cts.Token);
                        }

                        return;
                    }

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
                                break;
                            }

                        default:
                            string filePath = "output.wav";

                            speech.SetOutputToWaveFile(filePath);
                            speech.Speak(message.Text);
                            speech.SetOutputToDefaultAudioDevice();

                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                var cts = new CancellationTokenSource();
                                await client.SendDocumentAsync(
                                    chatId: message.Chat.Id,
                                    document: new Telegram.Bot.Types.InputFileStream(fileStream, filePath),
                                    cancellationToken: cts.Token
                                );
                            }
                            break;
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