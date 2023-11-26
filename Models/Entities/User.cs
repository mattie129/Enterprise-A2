using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    
    public class User
    {
        public Guid Id { get; set; }

        private string _email;
        public string Email { get => _email;  set {
            if (IsValidEmail(value)){
                _email = value;
            }
        }}
        public string Name { get; set; }
        public List<Image>? Images { get; set; }
        
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);
        }
        
    }
}