using me.bellacall.Core.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Учетные данные аккаунта
    /// </summary>
    public class AuthModel
    {
        [Log, Required, StringLength(256)]
        [EmailAddress]
        public virtual string Email { get; set; }

        [Required, StringLength(32)]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        public long? Company_Id { get; set; }
    }
}
