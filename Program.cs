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

    //this class just houses the global variables needed to track the items list and get the new price assigned 
    public class Globals
    {
        public static String newPrice;
        //public static String NewPrice { get; set; }

        public static int itemsCounter;
        public static int ItemsCounter { get { return itemsCounter; } set { itemsCounter = value; } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            List<Item> items = new List<Item>();
            items.Add(new Item { Id = 0, ItemPrice = 0, ItemName = "2 in. x 4 in. x 96 in. Prime Whitewood Stud", ItemURL = "2-in-x-4-in-x-96-in-Prime-Whitewood-Stud-058449/312528776", InternetNum = 312528776 });
            //items.Add(new Item { Id = 1, ItemPrice = 0, ItemName = "2-1/16 in. x 250 ft. Paper Drywall Joint Tape", ItemURL = "USG-Sheetrock-Brand-2-1-16-in-x-250-ft-Paper-Drywall-Joint-Tape-382175/100321613", InternetNum = 100321613 });
            //items.Add(new Item { Id = 2, ItemPrice = 0, ItemName = "1-1/4 in. x 8 ft. Quicksilver Metal Corner Bead", ItemURL = "ClarkDietrich-1-1-4-in-x-8-ft-Quicksilver-Metal-Corner-Bead-741339/203171469", InternetNum = 203171469 });

            //Scrape(items);
            Globals.ItemsCounter = 0;
            foreach (Item item in items)
            {
                //assigned $0 to global so i could test price updates function
                Globals.newPrice = "$0";

                //webScraping
                Scrape(item.ItemURL);

                //test print to check that actual price got assigned to variable
                Console.WriteLine(Globals.newPrice);

                //send the info to priceupdate to have the price parsed and added to the list value
                PriceUpdate(Globals.newPrice, items, Globals.ItemsCounter);
                //counter to keep track of all items in the list
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

                //test printing
                Console.WriteLine(firstResult.GetAttribute("textContent"));
                //String price = firstResult.GetAttribute("textContent");
                String price1 = firstResult.Text;

                //assigning the price to the global variable
                Globals.newPrice = firstResult.GetAttribute("textContent");

                //test printing
                Console.WriteLine(Globals.newPrice);
                //Console.WriteLine(price);
                 
            }
        }

        //TODO make decimal 0.00 format
        //parse the $ from string price, turn string to decimal, then update price on correct list item.
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