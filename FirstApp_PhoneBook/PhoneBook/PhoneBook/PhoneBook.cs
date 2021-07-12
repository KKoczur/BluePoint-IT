using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace PhoneBook
{
    class PhoneBook
    {
        private readonly Dictionary<string, Contact> _contactMap  = new Dictionary<string, Contact>();

        public bool AddContact(Contact contact)
        {
            if (contact == null)
            {
                Console.WriteLine("Contact not exists.");
                return false;
            }
            else
            {
                if (_contactMap.ContainsKey(contact.Name))
                {
                    Console.WriteLine($"Contact {contact} already exists.");
                    return false;
                }
                else
                {
                    if (_contactMap.TryAdd(contact.Name, contact))
                    {
                        Console.WriteLine($"Added contact: {contact}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Contact: {contact} cannot be added");
                        return false;
                    }

                }
            }
        }

        public void DisplayAllContacts()
        {
            Console.WriteLine("All contacts:");

            DisplayContactsDetails(_contactMap.Values);
        }

        public void FindContactByPhoneNumber(string phoneNumber)
        {
           Contact[] foundContacts = _contactMap.Values.Where(c => c.PhoneNumber == phoneNumber).ToArray();

            if (foundContacts.Length > 0)
            {
                Console.WriteLine("Contacts found:");
                DisplayContactsDetails(foundContacts);
            }
            else
            {
                Console.WriteLine("Contact not found");
            }
        }

        public void FindContactByName (string name)
        {
            if (_contactMap.ContainsKey(name))
            {

                Console.WriteLine("Contact found:");
                Console.WriteLine(_contactMap[name].ToString());
            }
            else
            {
                Console.WriteLine("Contact not found");
             }
        }

        
        static void DisplayContactsDetails(IEnumerable<Contact> contacts)
        {
            foreach (Contact contact in contacts)
            {
                Console.WriteLine(contact.ToString());
            }
        }
    }
}
