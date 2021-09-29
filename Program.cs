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
            
            //this extracts all contents from the CSV file
            ReadCSVFile(items);

            Globals.ItemsCounter = 0;
            foreach (Item item in items)
            {
                //Test: assigned $0 to global so i could test price updates function
                //Globals.newPrice = "0";

                //webScraping
                Scrape(item.ItemURL);

                //Test: print to check that actual price got assigned to variable
                //Console.WriteLine(Globals.newPrice);

                //send the info to PriceUpdate to have the price parsed and added to the list value
                PriceUpdate(Globals.newPrice, items, Globals.ItemsCounter, charArr);

                //counter to keep track of all items in the list
                Globals.ItemsCounter += 1;
            }

            //Test: by printing all prices out of items list we ensure everything was properly scraped then formatted
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

                //search the web page for the div with the name = "price", and put all child elements into firstResult
                IWebElement firstResult = wait.Until(ExpectedConditions.ElementExists(By.Name("price")));

                //Test: printing/assigning
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

                //Test: printing
                Console.WriteLine(Globals.newPrice);
                //Console.WriteLine(price);

            }
        }


        //parse the $ from string price, turn string to decimal, then update price on correct list item.
        public static void PriceUpdate(String newPrice, List<Item> items, int key, char[] chars)
        {

            // Character to remove, "\r\n" and all following char's
            int index1 = newPrice.IndexOf("\r\n"); 
            if (index1 > 0)
            {
                // This will remove all text after character "\r\n"
                newPrice = newPrice.Substring(0, index1); 
            }

            //remove the $ and any other non numerical symbols left over from the Substing above.
            foreach (var character in chars)
            {
                if (newPrice.Contains(character))
                {
                    //converting the searched char into a string
                    string repChar = character.ToString();
                    //replacing char with empty
                    newPrice = newPrice.Replace(repChar, string.Empty);
                }
            }
            newPrice = newPrice.Replace("\r\n90", string.Empty);

            //turn newPrice into a decimal and move decimal 2 spots to the left
            double newDPrice = double.Parse(newPrice) / 100;


            //insert deciaml price into the list for the specified item
            items[key].ItemPrice = newDPrice;
            //TEST: printing
            Console.WriteLine(newPrice);
        }


        static void ReadCSVFile(List<Item> items)
        {
            //open csv to read from
            //TODO: research whether or not File. auto closes the file to improve speed.
            var lines = File.ReadAllLines("Materials.csv");
            var list = new List<Item>();
            foreach (var line in lines)
            {
                //returns a string array of all values on each line
                var matValues = line.Split(',');

                //convert all integers from strings.
                int matQuantity = StringToInt(matValues[1]);
                //TODO: once we fill in all NPI #'s we will switch this back to integers.
                //int matNPI = StringToInt(matValues[2]);
                int matInternetNum = StringToInt(matValues[4]);

                //insert correct values into Item objects
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
                //add item into the main items list
                items.Add(item);
            }
            //TEST: print all items in items to show they all were transferred over from the csv properly
            items.ForEach(x => Console.WriteLine($"{x.ItemName}\t{x.ItemQuant}\t{x.ItemNPI}\t{x.ItemModel}\t{x.InternetNum}\t{x.ItemURL}\t"));
        }

        public static int StringToInt(string x)
        {
            //turn string into integer.
            return(int.Parse(x));
        }
    }



}