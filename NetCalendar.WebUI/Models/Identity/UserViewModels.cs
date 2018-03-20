using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetCalendar.WebUI.Models.Identity
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsManager { get; set; }
    }

    //public class CreateEmplModel
    //{
    //    [Required]
    //    public string Name { get; set; }

    //    [Required]
    //    public string Email { get; set; }

    //    [Required]
    //    public string Password { get; set; }
    //}

    public class AddEmplModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Password { get; set; }
    }



    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}