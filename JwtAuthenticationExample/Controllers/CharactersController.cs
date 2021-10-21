using JwtAuthenticationExample.Models;
using JwtAuthenticationExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JwtAuthenticationExample.Controllers
{
    // Voglio proteggere solo questa parte di API

    [Authorize] // solo coloro che mostrano di essere autenticati posso usare i metodi 
    // di questo controller
    [Route("[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _service;

        public CharactersController(ICharacterService service)
        {
            _service = service;
        }

        [HttpGet]
        public IEnumerable<Character> Get()
        {
            return _service.Characters();
        }

    }
}
