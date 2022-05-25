const mainAppAddress = 'https://localhost:44338'

export const routes = {
    events: {
        href: "/Event/Events",
    },
    manageEvents: {
        href: "/Event/ManageEvents"
    },
    manageUsers: {
        href: "/Account/ManageUsers"
    },
    manageVenues: {
        href: mainAppAddress+"/Venue/ManageVenues"
    },
    venues: {
        href: mainAppAddress + "/Venue/Venues"
    },
    home: {
        href: "/"
    },
    eventsApi: {
        href: "Events"
    },
    eventApi: {
        href: "Events",
    },
    login: {
        href: "/Account/Login"
    },
    registration: {
        href: "/Account/Registration"
    },
    logout: {
        href: "/Account/Logout"
    },
    changePassword: {
        href: "/Account/ChangePassword"
    },
    changePasswordApi: {
        href: "/Users/password"
    },
    purchaseHistory: {
        href: mainAppAddress + "/Account/PurchaseHistory"
    },
    balance: {
        href: mainAppAddress + "/Account/Balance"
    },
    editAccount: {
        href: "/Account/EditContactInfo"
    },
    cart: {
        href: "/Shopping/Cart"
    },
    registrationApi: {
        href: "logins/registration"
    },
    loginApi: {
        href: "logins/login"
    },
    validateApi: {
        href: "logins/validate"
    },
    userApi: {
        href: "Users"
    },
    venuesApi: {
        href: "Venues"
    },
    layoutApi: {
        href: "Layouts"
    },
    usersApi: {
        href: "Users"
    },
    editAccountApi: {
        href: "Users"
    },
    createUser: {
        href: "/Account/Create"
    },
    createUserApi: {
        href: "Users"
    },
    deleteRoleApi: {
        href: "Roles"
    },
    addRoleApi: {
        href: "Roles"
    },
    deleteUserApi: {
        href: "Users"
    },
    refreshTokenApi: {
        href: "Logins/refresh"
    },
    createEvent: {
        href: "/Event/Create"
    },
    createEventApi: {
        href: "Events"
    },
    editEventApi: {
        href: "Events"
    },
    deleteEventApi: {
        href: "Events"
    },
    importEventsApi: {
        href: "Events/import"
    },
    editEvent: {
        href: "/Event/Edit"
    },
    detailsEvent: {
        href: mainAppAddress + "/Event/Details"
    },
    setPriceApi: {
        href: "Areas"
    },
    attachImageApi: {
        href: "Events"
    }
}
