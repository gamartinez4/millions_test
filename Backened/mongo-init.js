// ---------------------------------------------------------------- //
// MongoDB initialization script for the Million Real Estate project //
// ---------------------------------------------------------------- //

// Switch to the 'million_db' database. It will be created if it doesn't exist.
db = db.getSiblingDB('million_db');

// Drop existing collections to start fresh
db.owners.drop();
db.properties.drop();

print("--- Creating collections and indexes ---");

// Create collections explicitly to set up schemas or indexes if needed in the future
db.createCollection("owners");
db.createCollection("properties");

print("Collections 'owners' and 'properties' created.");

// ---------------------------------------------------------------- //
//                           Seed Data                              //
// ---------------------------------------------------------------- //

print("--- Inserting seed data ---");

// 1. Insert an Owner
const ownerResult = db.owners.insertOne({
    _id: ObjectId("60d5ec49e0f3e2a3b4c5d6e1"), // Using a fixed ObjectId for predictability
    Name: "John Doe",
    Address: "123 Main St, Anytown, USA",
    Photo: "https://example.com/photos/john_doe.jpg",
    Birthday: new ISODate("1980-01-15T00:00:00Z")
});

if (!ownerResult.insertedId) {
    print("Error: Owner insertion failed.");
} else {
    print("Successfully inserted 1 owner.");
}

const ownerId = ownerResult.insertedId;

// 2. Insert Properties, referencing the Owner
const propertiesResult = db.properties.insertMany([
    {
        Name: "Casa de Playa",
        Address: "Calle 123, Playa del Sol",
        Price: 150000.00,
        Year: 2010,
        OwnerId: ownerId,
        PropertyImages: [
            {
                _id: new ObjectId(),
                File: "https://example.com/images/playa1.jpg",
                Enabled: true
            }
        ],
        PropertyTraces: []
    },
    {
        Name: "Apartamento en la Ciudad",
        Address: "Avenida 45, Centro Urbano",
        Price: 250000.00,
        Year: 2015,
        OwnerId: ownerId,
        PropertyImages: [
            {
                _id: new ObjectId(),
                File: "https://example.com/images/ciudad1.jpg",
                Enabled: true
            },
            {
                _id: new ObjectId(),
                File: "https://example.com/images/ciudad2.jpg",
                Enabled: false
            }
        ],
        PropertyTraces: [
            {
                _id: new ObjectId(),
                DateSale: new ISODate("2018-06-20T00:00:00Z"),
                Name: "Apartamento Antiguo",
                Value: 220000.00,
                Tax: 22000.00
            }
        ]
    },
    {
        Name: "Casa de Campo",
        Address: "Vereda 7, Montaña Alta",
        Price: 350000.00,
        Year: 2020,
        OwnerId: ownerId,
        PropertyImages: [],
        PropertyTraces: []
    }
]);

if (propertiesResult.insertedCount !== 3) {
    print(`Error: Only inserted ${propertiesResult.insertedCount} of 3 properties.`);
} else {
    print(`Successfully inserted ${propertiesResult.insertedCount} properties.`);
}


print("--- Database initialization complete ---"); 