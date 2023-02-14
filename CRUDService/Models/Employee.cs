﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDService.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Age { get; set; }
        public int IsActive { get; set; }
        public string? PathImage { get; set; }
    }
}
