using BejaiaCovid.Commands;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;

namespace BejaiaCovid
{
    class Program
    {
        private const string UserName = "YOUR_USERNAME_HERE";
        private const string Password = "YOUR_PASSWORD_HERE";
        
        private static IInstaApi _instaApi;

        private static List<string> _messagesIds = new();

        public const string Prefix = "!";

        public static Command[] Commands = new Command[]
        {
            new HelpCommand(),
            new SummaryDataCommand(),
            new DayDataCommand(),
            new ByAgeDataCommand(),
            new BejaiaDataCommand(),
            new BejaiaDayDataCommand()
        };

        static void Main(string[] args)
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
            Console.ReadKey();
        }

        public static async Task MainAsync()
        {
            try
            {
                
                Console.WriteLine("Bot instagram BéjaïaCovid");

                var userSession = new UserSessionData
                {
                    UserName = UserName,
                    Password = Password
                };
                
                

                var delay = RequestDelay.FromSeconds(2, 2);
                
                _instaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.Exceptions)) 
                    .SetRequestDelay(delay)
                    .Build();
                

                if (!_instaApi.IsUserAuthenticated)
                {
                    
                    Console.WriteLine($"Logging in as {userSession.UserName}");
                    delay.Disable();
                    var logInResult = await _instaApi.LoginAsync();
                    delay.Enable();
                    if (!logInResult.Succeeded)
                    {
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                        return;
                    }
                }

                while (true)
                {
                    await Task.Delay(300);
                    try
                    {
                        var pendingDirect = await _instaApi.MessagingProcessor
                            .GetPendingDirectAsync(PaginationParameters.Empty);
                        if (pendingDirect.Succeeded && pendingDirect.Value.Inbox.Threads is not null)
                        {
                            foreach (var thread in pendingDirect.Value.Inbox.Threads)
                            {
                                Task.Run(() =>
                                {
                                    RunThread(thread, userSession);
                                });
                            }
                        }
                        var inbox = (await _instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.Empty));
                        if (!inbox.Succeeded)
                            continue;

                        foreach (var thread in inbox.Value.Inbox.Threads)
                        {
                            Task.Run(() =>
                            {
                                RunThread(thread, userSession);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1);
        }
        
        private static async void RunThread(InstaDirectInboxThread thread, UserSessionData userSession)
        {
            var item = thread.Items.Last();
            if (!_messagesIds.Contains(item.ItemId))
            {
                _messagesIds.Add(item.ItemId);
                if (_instaApi.UserProcessor.GetFullUserInfoAsync(item.UserId).Result.Value.UserDetail.Username != userSession.UserName && item.Text.StartsWith(Prefix) && item.Text.Length > Prefix.Length)
                {
                    var args = item.Text.Substring(Prefix.Length).Split(" ");
                    foreach (var command in Commands)
                    {
                        if (command.Name == args[0])
                        {
                            var result = await command.Execute(_instaApi, args);
                            await _instaApi.MessagingProcessor.SendDirectTextAsync(null,
                                thread.ThreadId.ToString(), result);
                            break;
                        }
                    }
                                    
                }
            }
        }
    }

    
}