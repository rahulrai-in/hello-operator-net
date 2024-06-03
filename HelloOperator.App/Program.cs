using Figgle;

var count = 1;
var messageToPrint = FiggleFonts.Standard.Render(Environment.GetEnvironmentVariable("MESSAGE") ?? "No Message");

while (true)
{
    Console.WriteLine($"Message {count++} at {DateTimeOffset.Now}");
    Console.WriteLine(messageToPrint);
    await Task.Delay(TimeSpan.FromSeconds(30));
}