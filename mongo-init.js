// ---------------------------------------------------------------- //
// MongoDB initialization script for the Million Real Estate project //
// ---------------------------------------------------------------- //

// Switch to the 'millionsDB' database. It will be created if it doesn't exist.
db = db.getSiblingDB('millionsDB');

// Drop existing collections to start fresh
db.owners.drop();
db.properties.drop();
db.propertyImages.drop();
db.propertyTraces.drop();

print("--- Creating collections and indexes ---");

// Create collections and insert example data
db.createCollection('owners');
db.owners.insertMany([
  {
    _id: 1,
    Name: "John Doe",
    Address: "123 Main St",
    Photo: "photo.jpg",
    Birthday: new Date("1980-01-01"),
    Username: "john",
    Password: "password"
  },
  {
    _id: 2,
    Name: "Jane Smith",
    Address: "456 Oak Ave",
    Photo: "photo2.jpg",
    Birthday: new Date("1990-05-15"),
    Username: "jane",
    Password: "password"
  }
]);

db.createCollection('properties');
db.properties.insertMany([
  {
    _id: 1,
    Name: "Casa de Playa",
    Address: "Calle 123",
    Price: 150000,
    Year: 2010,
    OwnerId: 2,
    ForSale: true
  },
  {
    _id: 2,
    Name: "Apartamento en la Ciudad",
    Address: "Avenida 45",
    Price: 250000,
    Year: 2015,
    OwnerId: 1,
    ForSale: true

  },
  {
    _id: 3,
    Name: "Casa de Campo",
    Address: "Vereda 7",
    Price: 350000,
    Year: 2020,
    OwnerId: 1,
    ForSale: true
  }

]);

db.createCollection('propertyImages');
db.propertyImages.insertMany([
  {
    _id: 1,
    PropertyId: 1,
    File: "https://arquitectopablorestrepo.com/wp-content/uploads/2024/06/Diseno-casa-campestre-La-Morada-1.jpg",
    Enabled: true
  },
  {
    _id: 2,
    PropertyId: 1,
    File: "https://images.adsttc.com/media/images/5d34/e507/284d/d109/5600/0240/large_jpg/_FI.jpg",
    Enabled: true
  },
  {
    _id: 3,
    PropertyId: 2,
    File: "https://images.adsttc.com/media/images/5caf/246b/284d/d19a/9100/0586/large_jpg/13135-JO_SALESFORCE_TOWER_13_(c)_Jason_O'Rear.jpg",
    Enabled: true
  },
  {
    _id: 4,
    PropertyId: 3,
    File: "https://www.fincasquindio.com.co/wp-content/uploads/2022/02/finca_tipica_quindio.jpg",
    Enabled: true
  }
]);

db.createCollection('propertyTraces');


print("Collections created and populated in 'millionsDB'.");
print("--- Database initialization complete ---"); 