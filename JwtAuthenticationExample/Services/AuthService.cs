using JwtAuthenticationExample.Data;
using JwtAuthenticationExample.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _ctx;

        public AuthService(DataContext ctx)
        {
            _ctx = ctx;
        }

        // Al login con successo il metodo restituirà il
        // JWT => una stringa che corrisponde al Token
        // che l'utente dovrà rimandare ogni volta
        // per risultare autorizzato nelle varie parti protette della nostra API 
        public string Login(string username, string password)
        {
            var user = _ctx.Users.FirstOrDefault(user => user.Username == username);

            if (user is null)
            {
                // Se non esiste alcun utente con quell'username
                // giustamente il login non è avvenuto con successo
                throw new Exception("User not found");
                // Per praticità uso un'eccezione generica, ma si potrebbe
                // pensare di adottare un'eccezione custom più descrittiva
            }

            // Faccio un check sulla password inserita dall'utente
            // parangonando il valora hashed con quello salvato in DB
            // per mezzo del salt sempre salvato su DB
            if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                // Ci affidiamo ad un metodo il cui unico compito è di generare il token
                // a partire dell'utente che ha effettuato con successo
                // il Login
                return CreateToken(user);
            }

            throw new Exception("Bad credentials");
        }

        private string CreateToken(User user)
        {
            // Andiamo a definire le parti del payload, nello specifico
            // le parti per andare ad identificare l'identità dell'utente
            // Questa lista contiene le info dell'utente
            var claims = new List<Claim>
            {
                // Nel nostro sistema il modo in cui identifichiamo univocamente
                // l'utente è per mezzo del suo ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Conviene esternalizzare la chiave segreta
            var secret = "Super secret very very long long men pikachu";
            // Questo secret è importante per validare il token dell'utente
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Questa volta a differenza di priva, gli passo io il salt
            // in modo da applicare l'hash utilizzando quella che ho nel DB
            // corrispondente all'utente che si sta autenticando
            using (var hms = new HMACSHA512(passwordSalt))
            {
                // vado ad applicare l'hash alla password inserita dall'utente
                var hash = hms.ComputeHash(Encoding.UTF8.GetBytes(password));

                // devo vedere se il contenuto è identico all'hash
                // che è stato salvato nel DB
                return hash.SequenceEqual(passwordHash);
                // Sfruttiamo LINQ per effuare un paragone dei vari valori
                // contenuti nei due array, invece di farlo manualmente
            }
        }

        public bool Register(string username, string password)
        {
            // 1) Controllo se risulta esserci già un utente con lo stesso nome
            // per mezzo di un metodo helper
            // Vantaggi: Metodo Register più coinciso e possibilità di modificare
            // la logica di controllo in un secondo momento senza modificare
            // il metodo Register
            if (UserExists(username))
            {
                // Restituisco false per dire che la registrazione non è
                // avvenuta con successo
                return false;
            }

            // A questo punto siamo certi che l'username sia disponibile
            // andiamo ad effettuare il salvataggio su DB, MA dobbiamo prima
            // effettuare l'Hash della password
            // Anche qui sfruttiamo un metodo Helper che ci genera la password
            // Hashed, facciamo dare anche il salt, perché è con lo stesso Salt
            // che viene applicato l'hash
            var (hash, salt) = HashPassword(password);

            // Quindi procediamo ad effettuare la persistenza con il Dbset
            // andando a creare manualmente l'oggetto di tipo User
            // sfruttando i valori hashed e il sale restituiti dal metodo
            // precedente
            _ctx.Users.Add(
                new User
                {
                    Username = username,
                    PasswordHash = hash,
                    PasswordSalt = salt
                }
            );
            // Applichiamo il salvataggio
            _ctx.SaveChanges();
            // restituiamo true in quanto la registrazione è avvenuta con successo
            return true;
        }

        // In C# abbiamo diverse possibilità per gestire un
        // potenziale output da un metodo NON SINGOLO
        // In questo per mezzo di un Tuple, restituiamo entrambi i valori
        private (byte[] passwordHash, byte[] passwordSalt) HashPassword(string password)
        {
            // Qui implementiamo tutta la parte di Hash con HMACSHA512
            // HMACSHA512 è un oggetto di una classe che implementa l'interfaccia
            // IDisposable, gli oggetti delle classi che implementano quest'ultima
            // devono essere "messi via", ci sono diversi modi per farlo
            // Possiamo usare using in modo da sbarazzarci in automatico
            // questo oggetto una volta terminato il blocco definito da using
            // altrimento dovrei farlo in modo "manuale"
            using (var hms = new HMACSHA512())
            {
                // hms.Dispose(); Non serve fare .Dispose()
                // in quanto verrà "cestinato" automaticamente una volta terminato
                // il blocco di using
                // Il sale ce lo fornisce l'oggetto hmacsha512
                // e corrisponde alla chiave
                var salt = hms.Key; // in pratica l'algoritmo utilizza la key
                // per applicare il salt in fase di hashing
                // Per l'hash dobbiamo computarla, ma dobbiamo prima di tutto
                // ricavare il buffer (byte[]) dalla stringa
                // Abbiamo la classe Encoding che gestisce le varie codifiche di un testo
                // Visto la nostra password viaggia su http, sarà per forza utf8
                var hash = hms.ComputeHash(Encoding.UTF8.GetBytes(password));
                // restituisce nell'ordine giusto i due valori attraverso
                // un Tuple
                return (hash, salt);
            }
        }

        private bool UserExists(string username)
        {
            // Se nella tabella (Con LINQ) trovo qualcuno con lo stesso username
            // allora restituisco true
            return _ctx.Users.Any(user => user.Username == username);
        }
    }
}
