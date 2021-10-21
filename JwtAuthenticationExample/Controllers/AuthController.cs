using JwtAuthenticationExample.DTOs;
using JwtAuthenticationExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("signup")]
        public ActionResult<string> Register([FromBody] UserRegistrationRequest userRegistration)
        {
            // TODO:
            // Lasciamo un attimo in sospeso, andiamo a definire una classe
            // che descriva i dati che manderà l'utente in fase di registrazione
            // Dobbiamo creare una classe, il cui unico scopo è quello di fare
            // da contenitore di dati in fase di scampio tra server e client
            // e/o viceversa, le classi di questo tipo non vengono considerate
            // dei modelli, ma sono dei DTO (Data transfer Object)

            // Model Validation dotnet (validazione "automatizzata" degli oggetti che arrivano
            // nei controller)
            if (_service.Register(userRegistration.Username, userRegistration.Password))
            {
                return Ok("Registration completed");
            }
            return BadRequest("User already exists");
        }

        [HttpPost("login")] // Riutilizzo lo stesso DTO per non creare altre classi, ma converrebbe
        public ActionResult<string> Login([FromBody] UserRegistrationRequest user)
        {
            // Visto che nel service viene effettuare throw di un'eccezione
            // qualora il login non risulta essere stato effettuato con successo
            // usiamo direttamente un try/catch
            try
            {
                // Se va tutto a buon fine verra restituito il token (TODO)
                return Ok(_service.Login(user.Username, user.Password));
            }
            catch (Exception e)
            {
                // Altrimenti verrà mandato un BadRequest col messaggio dell'eccezione
                return BadRequest(e.Message);
            }
        }
    }
}
