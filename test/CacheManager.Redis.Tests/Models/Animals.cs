using System.Text.Json.Serialization;

namespace CacheManager.Redis.Tests.Models
{
    // Base class marked for System.Text.Json polymorphism: subclasses round-trip with a "$type" note.
    [JsonDerivedType(typeof(MarkedDog), "dog")]
    [JsonDerivedType(typeof(MarkedCat), "cat")]
    public class MarkedAnimal
    {
        public string Name { get; set; } = string.Empty;
    }

    public class MarkedDog : MarkedAnimal
    {
        public string Breed { get; set; } = string.Empty;
    }

    public class MarkedCat : MarkedAnimal
    {
        public bool Indoor { get; set; }
    }

    // Base class without the attribute: subclasses read back as the base class.
    public class PlainAnimal
    {
        public string Name { get; set; } = string.Empty;
    }

    public class PlainDog : PlainAnimal
    {
        public string Breed { get; set; } = string.Empty;
    }
}
