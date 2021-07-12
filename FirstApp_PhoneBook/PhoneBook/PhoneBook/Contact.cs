using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneBook
{
    class Contact
    {
        public Contact(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public override string ToString() => $"Name: {Name}, Phone: {PhoneNumber}";

    }
}
