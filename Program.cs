using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Reflection;
 
namespace DynamicWebScrape
{
    class Program
    {
        static void Main(string[] args)
        {
            Scrape();
            Console.ReadLine();
        }

        public static void Scrape()
        {
            ChromeOptions options = new ChromeOptions();
            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl("https://www.homedepot.com/p/2-in-x-4-in-x-96-in-Prime-Whitewood-Stud-058449/312528776");
                //driver.FindElement(By.ClassName("price-format__large price-format__main-price"))/*.Click()*/;
                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Id("standard-price")));
                Console.WriteLine(firstResult.GetAttribute("textContent"));
            }
        }
    }
}