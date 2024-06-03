using System;

namespace VisualProgrammingFinalProject
{
    internal class StudentViewModel
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public int? ParentID { get; set; }
        public string ParentName { get; set; }
        public int? ClassID { get; set; }
        public string ClassName { get; set; }
        public decimal? Balance { get; set; }
    }
}