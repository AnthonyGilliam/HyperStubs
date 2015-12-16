using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperStubs.Tests.Models
{
    public class SimplePerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string NickName { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
