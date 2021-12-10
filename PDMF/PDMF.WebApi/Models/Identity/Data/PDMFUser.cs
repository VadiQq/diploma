using System;
using Microsoft.AspNetCore.Identity;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Models.Identity.Data
{
    public class PDMFUser : IdentityUser
    {
        public DateTime CreateDate { get; set; }
        public UserStatus State { get; set; }
        public string SpareEmail { get; set; }
    }
}