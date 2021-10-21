using JwtAuthenticationExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Services
{
    public interface ICharacterService
    {
        List<Character> Characters();

        Character AddCharacter(Character newCharacter);
    }
}
