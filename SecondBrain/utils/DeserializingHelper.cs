using System;
using System.Collections.Generic;
using SecondBrain.notes;
using Newtonsoft.Json.Linq;

namespace SecondBrain.utils{

    public static class DeserializingHelper {

        public static Category<Note> DeserializeCategory(JObject json)
        {
            if (!CheckForRequiredKeys(new[] { "CategoryType", "Name", "Values", "SubCategories" }, json))
            {
                throw new Exception("Missing at least one key");
            }

            var categoryType = json["CategoryType"].ToString();

            if (categoryType != "Note")
            {
                throw new Exception("Trying to deserialize category with invalid type. Expected: 'Note'");
            }

            var category = new Category<Note>(json["Name"].ToString());

            foreach (JObject prop in json["Values"])
            {
                category.Values.Add(DeserializeNote(prop));
            }

            foreach (JObject prop in json["SubCategories"])
            {
                category.SubCategories.Add(DeserializeCategory(prop));
            }

            return category;
        }

        private static Note DeserializeNote(JObject json)
        {
            if (!CheckForRequiredKeys(new[] { "NoteType", "Name", "Tags", "TimeCreated" }, json))
            {
                throw new Exception("Missing at least one key");
            }

            var parsedType = NoteType.TryParse(json["NoteType"].ToString(), out NoteType noteType);

            if (!parsedType)
            {
                throw new Exception("Could not find note type");
            }

            Note note = null;

            switch (noteType)
            {
                case NoteType.Plaintext:
                    if (!CheckForRequiredKeys(new[] { "PathToContent" }, json))
                    {
                        throw new Exception("Missing at least one key");
                    }

                    note = new PlaintextNote(
                            json["Name"].ToString(),
                            ((JArray) json["Tags"]).ToObject<List<string>>(),
                            DateTime.Parse(json["TimeCreated"].ToString()),
                            json["PathToContent"].ToString());
                    break;
                case NoteType.Contact:
                    note = new ContactNote(
                            DateTime.Parse(json["TimeCreated"].ToString()),
                            ((JArray) json["Tags"]).ToObject<List<string>>(),
                            json["Number"].ToString(),
                            json["FirstName"].ToString(),
                            json["LastName"].ToString());
                    break;
            }

            if (note == null)
            {
                throw new Exception("Invalid note type");
            }

            return note;
        }

        private static bool CheckForRequiredKeys(string[] requiredKeys, JObject json)
        {
            foreach (var requiredKey in requiredKeys)
            {
                if (!json.ContainsKey(requiredKey))
                {
                    return false;
                }
            }

            return true;
        }
    }
}