using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneBook
{
    class PhoneBook
    {
        private readonly List<Contact> _contactList  = new List<Contact>();

        public bool AddContact(Contact contact)
        {
            if (contact == null)
            {
                Console.WriteLine("Contact not exists.");
                return false;
            }
            else
            {
                if (contact.Name.Length < 3)
                {
                    Console.WriteLine("Username should have at least 3 characters");
                    return false;

                }

                if (contact.PhoneNumber.Length < 9)
                {
                    Console.WriteLine("Phone number should have at least 9 characters");
                    return false;
                }

                if (_contactList.Any(c => c.Name == contact.Name))
                {
                    Console.WriteLine($"Contact {contact} already exists.");
                    return false;
                }
                else
                {
                    _contactList.Add(contact);
                    
                        Console.WriteLine($"Added contact: {contact}");
                        return true;
                }
            }
        }

        public void DisplayAllContacts()
        {
            Console.WriteLine("All contacts:");

            DisplayContactsDetails(_contactList);
        }

        public Contact[] FindContactByName(string name)
        {
            Contact[] foundContacts = _contactList.Where(c => c.Name == name).ToArray();
            DisplaySearchResult(foundContacts);

            return foundContacts;
        }

        public Contact[] FindContactByPhoneNumber(string phoneNumber)
        {
           Contact[] foundContacts = _contactList.Where(c => c.PhoneNumber == phoneNumber).ToArray();

            DisplaySearchResult(foundContacts);

            return foundContacts;
        }

        public void DeleteContactByNumber(string phoneNumber)
        {
            Contact[] foundContacts = FindContactByPhoneNumber(phoneNumber);

            if (foundContacts.Length > 0)
            {
                foreach (var contact in foundContacts)
                {
                    _contactList.Remove(contact);
                    Console.WriteLine($"Removed contact: {contact}");
                }
            }
            else
            {
                Console.WriteLine("Contact not found");
            }
        }

        void DisplaySearchResult(Contact[] foundContacts)
        {
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

        void DisplayContactsDetails(IEnumerable<Contact> contacts)
        {
            foreach (Contact contact in contacts)
            {
                Console.WriteLine(contact.ToString());
            }
        }
    }
}
