using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace OmvpCrawler;

public static class WebDriverTasks
{
    public static void WaitForReadyState(ChromeDriver webDriver)
    {
        new WebDriverWait(webDriver, TimeSpan.FromSeconds(60)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState;").Equals("complete"));
    }
}