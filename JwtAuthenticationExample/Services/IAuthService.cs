using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Services
{
    public interface IAuthService
    {
        // Per semplicità gestito in fase di registrazione solo 
        // questi due dati, ma potete usare un modello
        // dove definire le informazioni di registrazione dell'utente
        bool Register(string username, string password);

        // Per il login non serve altro. Il tipo di ritorno lo metto a string
        // Ma lo vediamo in un secondo momento a cosa può servire
        string Login(string username, string password);
    }
}
