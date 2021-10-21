using JwtAuthenticationExample.Data;
using JwtAuthenticationExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationExample.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly DataContext _ctx;

        public CharacterService(DataContext ctx)
        {
            _ctx = ctx;
        }

        public Character AddCharacter(Character newCharacter)
        {
            var createdChar = _ctx.Characters.Add(newCharacter);
            _ctx.SaveChanges();
            return createdChar.Entity;
        }

        public List<Character> Characters()
        {
            return _ctx.Characters.ToList();
        }
    }
}
