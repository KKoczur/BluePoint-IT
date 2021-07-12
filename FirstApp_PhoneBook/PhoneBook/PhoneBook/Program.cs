﻿using System;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

namespace PhoneBook
{
    class Program
    {
        static void Main(string[] args)
        {
            var phoneBook = new PhoneBook();
            phoneBook.AddContact(new Contact("Bill", "123456789"));
            phoneBook.AddContact(new Contact("Tom", "987654321"));
            Console.WriteLine();


            DisplayMenuOptions();
            MenuOption menuOption = GetSelectedMenuOption();

            while (menuOption != MenuOption.Exit)
            {
                switch (menuOption)
                {
                    case MenuOption.Add:

                        Console.WriteLine("Set name of contact");
                        string userName = Console.ReadLine();
                        Console.WriteLine("Set phone number of contact");
                        string phoneNumber = Console.ReadLine();

                        if (!phoneBook.AddContact(new Contact(userName, phoneNumber)))
                        {
                            Console.WriteLine("User has not been added");
                        }
                        break;


                    case MenuOption.FindByNumber:

                        Console.WriteLine("Set phone number of contact");
                        string userPhoneNumber = Console.ReadLine();
                        phoneBook.FindContactByPhoneNumber(userPhoneNumber);

                        break;

                    case MenuOption.DisplayAll:
                        phoneBook.DisplayAllContacts();
                        break;

                    case MenuOption.FindByName:

                        Console.WriteLine("Set name of contact");
                        string inputName = Console.ReadLine();
                        phoneBook.FindContactByName(inputName);

                        break;

                    case MenuOption.Remove:

                        Console.WriteLine("Set phone number of contact to be deleted");
                        string userToBeDeletedPhoneNumber = Console.ReadLine();
                        phoneBook.DeleteContactByNumber(userToBeDeletedPhoneNumber);

                        break;

                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }

                DisplayMenuOptions();
                menuOption = GetSelectedMenuOption();
            }

        }

        private static MenuOption GetSelectedMenuOption()
        {
            string userInput = Console.ReadLine();
            MenuOption menuOption = MenuOption.Unknown;
            if (int.TryParse(userInput, out int intUserInput))
            {
                menuOption = (MenuOption)intUserInput;
            }

            return menuOption;
        }

        static void DisplayMenuOptions()
        {
            Console.WriteLine();
            Console.WriteLine(" *** Menu ***");
            Console.WriteLine("1 -> Add contact");
            Console.WriteLine("2 -> Find a contact by phone number");
            Console.WriteLine("3 -> Display all contacts");
            Console.WriteLine("4 -> Find a contact by name");
            Console.WriteLine("5 -> Delete contact");
            Console.WriteLine("6 -> Exit");
            Console.WriteLine(" *** Menu ***");
            Console.WriteLine();
        }
    }
}
