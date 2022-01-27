using System.Text.Json;
using BejaiaCovid.Commands;
using BejaiaCovid.Models;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using VectSharp;
using VectSharp.Raster;

namespace BejaiaCovid
{
    class Program
    {
        private const string UserName = "YOUR_USERNAME";
        private const string Password = "YOUR_PASSWORD";

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
            new BejaiaDayDataCommand(),
            new WilayaDataCommand(),
            new WilayaDayDataCommand()
        };

        static void Main(string[] args)
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static DateTime last;
        private static readonly FontFamily GowunBold = new FontFamily(File.OpenRead(@"./Assets/GowunBatang-Bold.ttf"));

        private static readonly FontFamily GowunRegular =
            new FontFamily(File.OpenRead(@"./Assets/GowunBatang-Regular.ttf"));

        public static async Task AutoPost()
        {
            if (File.Exists("./last.json"))
            {
                last = JsonSerializer.Deserialize<DateTime>(await File.ReadAllTextAsync("./last.json"));
            }
            else
            {
                last = new DateTime(2010, 01, 01);
            }

            while (true)
            {
                using var client = new HttpClient();
                var data = await client.GetAsync("https://api.corona-dz.live/country/summary");
                var json = await data.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SummaryElement>(json, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result.Latest.UpdatedAt.ToLongDateString() != last.ToLongDateString())
                {
                    last = result.Latest.UpdatedAt;
                    await File.WriteAllTextAsync("./last.json", JsonSerializer.Serialize(last));

                    var data2 = await client.GetAsync("https://api.corona-dz.live/province/6/latest");
                    var json2 = await data2.Content.ReadAsStringAsync();
                    var result2 = JsonSerializer.Deserialize<ProvinceElement[]>(json2, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    })?[0];

                    Document doc = new Document();
                    doc.Pages.Add(new Page(1024, 1024));
                    doc.Pages.Last().Background = Colour.FromRgb(27, 33, 43);
                    Graphics gpr = doc.Pages.Last().Graphics;
                    WriteTextCentered(ref gpr,
                        $"Bilan national {result.Latest.Date.ToString("dd/MM/yyyy")}",
                        70, 75, GowunBold, Colours.White);

                    WriteTextCentered(ref gpr,
                        $"Jour n°{result.Days}",
                        140, 60, GowunRegular, Colours.LightGray);

                    WriteTextCentered(ref gpr,
                        $"Nouveaux cas : {result.Latest.NewConfirmed}",
                        250, 75, GowunRegular, Colours.Orange);
                    WriteTextCentered(ref gpr,
                        $"Guérisons : {result.Latest.NewRecovered}",
                        350, 75, GowunRegular, Colours.GreenYellow);
                    WriteTextCentered(ref gpr,
                        $"Décès : {result.Latest.NewDeaths}",
                        450, 75, GowunRegular, Colours.Red);

                    WriteTextCentered(ref gpr,
                        $"Bilan régional - Bejaïa",
                        600, 75, GowunBold, Colours.White);

                    WriteTextCentered(ref gpr,
                        $"Nouveaux cas : {result2.Data[0].NewConfirmed}",
                        700, 75, GowunRegular, Colours.Orange);
                    WriteTextCentered(ref gpr,
                        $"Décès : {result2.Data[0].NewDeaths}",
                        800, 75, GowunRegular, Colours.Red);

                    doc.Pages.Last().SaveAsPNG("./result.png");

                    using (Image image = Image.Load("./result.png"))
                    {
                        image.Save("./result.jpeg", new JpegEncoder());
                    }

                    var postResult = await _instaApi.MediaProcessor.UploadPhotoAsync(
                        (a) => { Console.WriteLine(a.UploadState.ToString()); }, new InstaImageUpload("./result.jpeg"),
                        "Bilan covid-19 du " + result.Latest.Date.ToString("dd/MM/yyyy"));
                    Console.WriteLine(postResult.Succeeded ? "Yay" : "Aïe : " + postResult.Info.Message);
                }


                await Task.Delay(300000);
            }
        }

        private static void WriteTextCentered(ref Graphics gpr, string text, double y,
            double fontSize, FontFamily family, Colour colour)
        {
            var font = new Font(
                family,
                fontSize);
            var x = 512 - (gpr.MeasureText(text, font).Width / 2);
            gpr.FillText(new VectSharp.Point(x, y), text,
                font, colour, TextBaselines.Middle);
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

#pragma warning disable CS4014
                AutoPost();
#pragma warning restore CS4014


                while (true)
                {
                    await Task.Delay(2500);
                    try
                    {
                        var pendingDirect = await _instaApi.MessagingProcessor
                            .GetPendingDirectAsync(PaginationParameters.Empty);
                        if (pendingDirect.Succeeded && pendingDirect.Value.Inbox.Threads is not null)
                        {
                            foreach (var thread in pendingDirect.Value.Inbox.Threads)
                            {
                                Task.Run(() => { RunThread(thread, userSession); });
                            }
                        }

                        var inbox =
                            (await _instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.Empty));
                        if (!inbox.Succeeded)
                            continue;

                        foreach (var thread in inbox.Value.Inbox.Threads)
                        {
                            Task.Run(() => { RunThread(thread, userSession); });
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
            try
            {
                var item = thread.Items.Last();
                if (!_messagesIds.Contains(item.ItemId))
                {
                    _messagesIds.Add(item.ItemId);
                    if (_instaApi.UserProcessor.GetFullUserInfoAsync(item.UserId).Result.Value.UserDetail.Username !=
                        userSession.UserName && item.Text.StartsWith(Prefix) && item.Text.Length > Prefix.Length)
                    {
                        var args = item.Text.Substring(Prefix.Length).Split(" ");
                        foreach (var command in Commands)
                        {
                            if (command.Name == args[0])
                            {
                                var result = await command.Execute(_instaApi, args);
                                await _instaApi.MessagingProcessor.SendDirectTextAsync(null,
                                    thread.ThreadId.ToString(), result);
                                return;
                            }
                        }

                        await _instaApi.MessagingProcessor.SendDirectTextAsync(null,
                            thread.ThreadId.ToString(), "Commande inconnue");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}