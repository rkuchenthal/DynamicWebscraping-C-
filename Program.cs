using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace DynamicWebScrape
{
    public class Globals
    {
        public static String newPrice;
        public static String NewPrice { get { return newPrice; } set { newPrice = value; } }

        public static int itemsCounter;
        public static int ItemsCounter { get { return itemsCounter; } set { itemsCounter = value; } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            List<Item> items = new List<Item>();
            items.Add(new Item { Id = 0, ItemPrice = 0, ItemName = "2 in. x 4 in. x 96 in. Prime Whitewood Stud", ItemURL = "2-in-x-4-in-x-96-in-Prime-Whitewood-Stud-058449/312528776", InternetNum = 312528776 });
            items.Add(new Item { Id = 1, ItemPrice = 0, ItemName = "2-1/16 in. x 250 ft. Paper Drywall Joint Tape", ItemURL = "USG-Sheetrock-Brand-2-1-16-in-x-250-ft-Paper-Drywall-Joint-Tape-382175/100321613", InternetNum = 100321613 });
            items.Add(new Item { Id = 2, ItemPrice = 0, ItemName = "1-1/4 in. x 8 ft. Quicksilver Metal Corner Bead", ItemURL = "ClarkDietrich-1-1-4-in-x-8-ft-Quicksilver-Metal-Corner-Bead-741339/203171469", InternetNum = 203171469 });

            //Scrape(items);
            Globals.ItemsCounter = 0;
            foreach (Item item in items)
            {
                Scrape(item.ItemURL);
                //Console.WriteLine(item.InternetNum);
                PriceUpdate(Globals.NewPrice, items, Globals.ItemsCounter);
                Globals.ItemsCounter += 1;
            }

            //testing by printing all prices out of items list
            foreach(Item item in items)
            {
                Console.WriteLine(item.ItemPrice);
            }
        }

        public static void Scrape(String itemURL)
        {
            ChromeOptions options = new ChromeOptions();

            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                
                driver.Navigate().GoToUrl("https://www.homedepot.com/p/" + itemURL);
                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Id("standard-price")));

                Globals.newPrice = firstResult.GetAttribute("textContent");
                Console.WriteLine(Globals.newPrice);
                 
            }
        }
        public static void PriceUpdate(String newPrice, List<Item> items, int key)
        {
            //remove the $ symbol from the price
            newPrice = newPrice.Replace("$", string.Empty);

            //turn newPrice into a decimal
            decimal newDPrice = decimal.Parse(newPrice);

            //insert deciaml price into the list for the specified item
            items[key].ItemPrice = newDPrice;
            
        }
    }



}