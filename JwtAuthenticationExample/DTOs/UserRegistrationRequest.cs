using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.DTOs
{
    // Questa classe rappresenta un DTO per la fase di registrazione
    // dell'utente
    public class UserRegistrationRequest
    {
        // Sono i due dati che il client manda al server in fase di registrazione
        // di un nuovo Utente
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
