using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperStubs.Tests.Models
{
    public class ComplexPerson : SimplePerson
    {
        public House House { get; set; }
    }

    public class House
    {
        public Address Address { get; set; }
        public byte Floors { get; set; }
        public byte Bathrooms { get; set; }
        public decimal SquareFeet { get; set; }
    }
}
