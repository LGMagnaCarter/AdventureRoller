namespace AdventureRoller.Services
{
    using AdventureRoller.DatabaseContext;
    using AdventureRoller.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CharacterAttributeService : ICharacterAttributeService
    {
        private AdventurerollerdbContext DbContext { get; }

        public CharacterAttributeService(AdventurerollerdbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Response AddCharacterAttribute(Guid characterId, int level, string attribute, string value)
        {
            string error = ValidateAdd(characterId, attribute);

            if (!string.IsNullOrEmpty(error))
            {
                return new StringResponse(false, error);
            }

            DbContext.CharacterAttributes.Add(new CharacterAttributes
            {
                CharacterId = characterId,
                CharacterLevel = level,
                Name = attribute,
                Value = value
            });

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                DbContext.RejectChanges();
                return new Response(false, "Error committing to database!");
            }

            return Response.SuccessfulResponse;
        }

        public Response AddCharacterAttributes(Guid characterId, int level, Dictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                string error = ValidateAdd(characterId, attribute.Key);

                if (!string.IsNullOrEmpty(error))
                {
                    return new StringResponse(false, error);
                }

                DbContext.CharacterAttributes.Add(new CharacterAttributes
                {
                    CharacterId = characterId,
                    CharacterLevel = level,
                    Name = attribute.Key,
                    Value = attribute.Value
                });
            }
            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                DbContext.RejectChanges();
                return new Response(false, "Error committing to database!");
            }

            return Response.SuccessfulResponse;
        }

        public StringResponse GetCharacterAttribute(Guid characterId, int level, string attribute)
        {
            var caList = DbContext.CharacterAttributes.AsQueryable().Where(ca => ca.CharacterId == characterId && ca.Name.StartsWith(attribute));

            var error = ValidateOnlyOne(caList, attribute);

            if (!string.IsNullOrEmpty(error))
            {
                return new StringResponse(false, error);
            }

            return new StringResponse(true, caList.First().Value);
        }

        public Response UpdateCharacterAttributes(Guid characterId, int level, Dictionary<string, string> attributes)
        {
            foreach (var attribute in attributes)
            {
                var caList = DbContext.CharacterAttributes.AsQueryable().Where(ca => ca.CharacterId == characterId && ca.CharacterLevel == level && ca.Name.StartsWith(attribute.Key));

                var error = ValidateOnlyOne(caList, attribute.Key);

                if (!string.IsNullOrEmpty(error))
                {
                    return new StringResponse(false, error);
                }

                caList.First().Value = attribute.Value;

            }

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                DbContext.RejectChanges();
                return new Response(false, "Error committing to database");
            }

            return Response.SuccessfulResponse;
        }

        public Response DeleteCharacterAttributes(Guid characterId, int? level = null)
        {
            try
            {
                var caList = DbContext.CharacterAttributes.AsQueryable().Where(ca => ca.CharacterId == characterId && (!level.HasValue || ca.CharacterLevel == level));

                foreach (var ca in caList)
                {
                    DbContext.Remove(ca);
                }

                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                DbContext.RejectChanges();
                return new Response(false, "Error removing character attributes from database");
            }

            return Response.SuccessfulResponse;
        }

        #region Validation
        private string ValidateOnlyOne(IQueryable<CharacterAttributes> caList, string attribute)
        {
            if (!caList.Any())
            {
                return $"No Attribute starts with {attribute}";
            }
            if (caList.Count() > 1)
            {
                return $"Conflict: {attribute} could refer to {string.Join(",", caList.Select(a => a.Name).ToList())}";
            }

            return string.Empty;
        }

        private string ValidateAdd(Guid characterId, string attribute)
        {
            var caList = DbContext.CharacterAttributes.AsQueryable().Where(ca => ca.CharacterId == characterId && (ca.Name.StartsWith(attribute) || attribute.StartsWith(ca.Name)));

            if (caList.Count() != 0)
            {
                return $"Conflict: {attribute} could refer to {string.Join(",", caList.Select(a => a.Name).ToList())}";
            }

            return string.Empty;
        }
        #endregion
    }
}
