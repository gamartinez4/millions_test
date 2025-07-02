namespace Million.Domain.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
    
        public string File { get; set; }
        public bool Enabled { get; set; }

        public PropertyImage(int propertyId, string file, bool enabled)
        {
            if (propertyId <= 0)
            {
                throw new ArgumentException("Property ID must be greater than zero.", nameof(propertyId));
            }
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException("File path cannot be empty.", nameof(file));
            }

            PropertyId = propertyId;
            File = file;
            Enabled = enabled;
        }
    }
} 