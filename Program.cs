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

    //this class just houses the global variables are needed to track the items list and get the new price assigned 
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
            
            //Create the items list for all products
            List<Item> items = new List<Item>();

            // Defining a string to turn to characters
            string scentence = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ$!@#%^&*(),./'"; 
            // Turns String to a list of characters
            char[] charArr = scentence.ToCharArray();
            
            ReadCSVFile(items);

            Globals.ItemsCounter = 0;
            foreach (Item item in items)
            {
                //test, assigned $0 to global so i could test price updates function
                //Globals.newPrice = "0";

                //webScraping
                Scrape(item.ItemURL);

                //test print to check that actual price got assigned to variable
                //Console.WriteLine(Globals.newPrice);

                //send the info to priceupdate to have the price parsed and added to the list value
                PriceUpdate(Globals.newPrice, items, Globals.ItemsCounter, charArr);
                //counter to keep track of all items in the list
                Globals.ItemsCounter += 1;
            }

            //testing by printing all prices out of items list
            foreach(Item item in items)
            {

                Console.WriteLine(item.ItemName + " = $" + item.ItemPrice);
            }
        }


        public static void Scrape(String itemURL)
        {
            ChromeOptions options = new ChromeOptions();

            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                
                driver.Navigate().GoToUrl("https://www.homedepot.com/p" + itemURL);
                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Id("standard-price")));
                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Name("price")));

                //test printing/assigning
                //Console.WriteLine(firstResult.GetAttribute("textContent"));
                //String price = firstResult.GetAttribute("textContent");
                //String price1 = firstResult.Text;

                //assigning the price to the global variable
                //Globals.newPrice = firstResult.GetAttribute("textContent");
                Globals.newPrice = firstResult.Text;

                //Had to ad this loop because for some unknown reason Globals.newPrice
                //was not being assigned the firstResult.text everytime
                while (Globals.newPrice.Equals("") || Globals.newPrice == null)
                {
                    Globals.newPrice = firstResult.Text;
                }

                //test printing
                Console.WriteLine(Globals.newPrice);
                //Console.WriteLine(price);

            }
        }


        //parse the $ from string price, turn string to decimal, then update price on correct list item.
        public static void PriceUpdate(String newPrice, List<Item> items, int key, char[] chars)
        {

           
            //int index = newPrice.LastIndexOf(" "); // Character to remove, space
            //if (index > 0)
            //{
            //    newPrice = newPrice.Substring(0, index); // This will remove all text after character space
            //}
                    

            int index1 = newPrice.IndexOf("\r\n"); // Character to remove, "\r\n"
            if (index1 > 0)
            {
                newPrice = newPrice.Substring(0, index1); // This will remove all text after character "\r\n"
            }
                    
           

            //remove the $ and any other non numerical symbol from the price
            foreach (var character in chars)
            {
                if (newPrice.Contains(character))
                {
                    string repChar = character.ToString();
                    newPrice = newPrice.Replace(repChar, string.Empty);
                }
            }
            newPrice = newPrice.Replace("\r\n90", string.Empty);

            //turn newPrice into a decimal and move decimal 2 spots to the left
            double newDPrice = double.Parse(newPrice) / 100;


            //insert deciaml price into the list for the specified item
            items[key].ItemPrice = newDPrice;
            //test printing
            Console.WriteLine(newPrice);
        }


        static void ReadCSVFile(List<Item> items)
        {
            var lines = File.ReadAllLines("Materials.csv");
            var list = new List<Materials>();
            foreach (var line in lines)
            {
                //returns a string array of all values on each line
                var matValues = line.Split(',');

                //convert all integers from strings.
                int matQuantity = StringToInt(matValues[1]);
                //int matNPI = StringToInt(matValues[2]);
                int matInternetNum = StringToInt(matValues[4]);

                //insert correct values in matierals
                var item = new Item()
                {
                    ItemName = matValues[0],
                    ItemQuant = matQuantity,
                    ItemNPI = matValues[2],
                    ItemModel = matValues[3],
                    InternetNum = matInternetNum,
                    ItemURL = matValues[5],
                    ItemPrice = 0
                };

                items.Add(item);
            }
            items.ForEach(x => Console.WriteLine($"{x.ItemName}\t{x.ItemQuant}\t{x.ItemNPI}\t{x.ItemModel}\t{x.InternetNum}\t{x.ItemURL}\t"));
        }

        public static int StringToInt(string x)
        {
            //turn string into integer.
            return(int.Parse(x));
        }
    }



}