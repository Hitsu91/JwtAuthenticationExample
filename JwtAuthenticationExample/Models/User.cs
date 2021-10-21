using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Models
{
    // Questo modello deve rappresentare i dati dell'utente che verranno
    // utilizzati nel nostro sistema
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        // Nel DB salviamo solo il valore della password Hashed
        // Un valore a cui applico l'hash, non può essere più ricavato
        // dato valore A => B, da B non posso risalire ad A
        // ma solo rippassando A allo stesso algoritmo di Hash, ottengo sempre B
        public byte[] PasswordHash { get; set; }

        // Il salt serve ad aggiungere un valore che non c'entra in alcun modo
        // con la password dell'utente, in modo che se due utenti hanno
        // la stessa password, non avranno lo stesso valore nella password 
        // hashed
        // Ad esempio se l'utente si registra con password 1234
        // a quel valore aggiungeremo il "sale" Es: 1234ciccioplutopaperino&5%3
        public byte[] PasswordSalt { get; set; }
        // Quando l'utente effettua il login, noi andiamo a prendere la sua password
        // andiamo a recuperare il suo sale, li combiniamo insieme
        // applichiamo lo stesso algoritmo di Hash e vediamo se corrisponde
        // esattamente al valore PasswordHash salvato nel DB

    }
}
