export class Owner {
    constructor(id, name, address, photo, birthday, username, properties = []) {
        this.id = id
        this.name = name
        this.address = address
        this.photo = photo
        this.birthday = birthday
        this.username = username
        this.properties = properties
    }
}
