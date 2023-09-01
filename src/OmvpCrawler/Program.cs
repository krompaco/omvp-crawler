using OmvpCrawler;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Spectre.Console;

var chromeOptions = new ChromeOptions();

// In case you want to hide the browser window
////chromeOptions.AddArguments("--headless");

// In case you want to test against local apps with dodgy SSL certs
////chromeOptions.AcceptInsecureCertificates = true;

const string TestUrl = "https://world.optimizely.com/community/omvp/members/";

var webDriver = new ChromeDriver(chromeOptions)
{
    Url = TestUrl,
};

WebDriverTasks.WaitForReadyState(webDriver);

// Code to read entire DOM HTML after JS complete
var htmlAsString = ((IJavaScriptExecutor)webDriver).ExecuteScript("return document.documentElement.outerHTML;").ToString();
var filePath = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now:yyyyMMddHHmmss}-{FilePaths.GetSafeFileName(TestUrl)}-browser.html");
using (var outputFile = new StreamWriter(filePath))
{
    outputFile.WriteLine("<!DOCTYPE html>");
    outputFile.Write(htmlAsString);
}

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"[green]{filePath}[/]");

var profileUrls = webDriver.FindElements(By.CssSelector(@".datatablecontainerblock .table-responsive a.cta[href^=""/System/Users-and-profiles/Community-Profile-Card/""]"))
    .Select(x => x.GetAttribute("href"))
    .ToList();

var grid = new Grid();
grid.AddColumn();
grid.AddRow(new Text($"Profile URLs found: {profileUrls.Count}", new Style(Color.Blue, Color.Black)).LeftJustified());

foreach (var url in profileUrls)
{
    grid.AddRow(url);
}

AnsiConsole.WriteLine();
AnsiConsole.Write(grid);
AnsiConsole.WriteLine();

AnsiConsole.MarkupLine("[blue]First blog post URL[/]");
var firstBlogPostUrls = new List<Uri>();

foreach (var url in profileUrls)
{
    webDriver.Navigate().GoToUrl(url);
    WebDriverTasks.WaitForReadyState(webDriver);

    var firstBlogPost = webDriver.FindElements(By.CssSelector(@".tab-pane .blog-list-title a")).FirstOrDefault();

    if (firstBlogPost != null)
    {
        var firstBlogPostUrl = new Uri(firstBlogPost.GetAttribute("href"));

        if (!firstBlogPostUrl.Host.Equals("world.optimizely.com"))
        {
            AnsiConsole.MarkupLine($"[green]{firstBlogPostUrl}[/]");
            firstBlogPostUrls.Add(firstBlogPostUrl);
        }
    }
}

var sitesToAdd = new List<string>();

foreach (var url in firstBlogPostUrls.OrderBy(x => x.Host))
{
    var siteToAdd = "https://" + url.Host + (url.Host.Equals("jcpretorius .com") ? "/blog/" : string.Empty);

    if (!sitesToAdd.Contains(siteToAdd))
    {
        sitesToAdd.Add(siteToAdd);
    }
}

grid = new Grid();
grid.AddColumn();
grid.AddRow(new Text($"Blog post URLs found: {firstBlogPostUrls.Count} / Unique: {sitesToAdd.Count}", new Style(Color.Yellow, Color.Black)).LeftJustified());

foreach (var siteToAdd in sitesToAdd)
{
    grid.AddRow(siteToAdd);
}

AnsiConsole.WriteLine();
AnsiConsole.Write(grid);
AnsiConsole.WriteLine();
