# Event Application

# Requirements

* Create an ASP.NET Core application.
* It should have a relational database to persist event registrations.
* There are two kinds of users
    1. An event creator who has to login
    2. An event participant, who can register for the event
* The event creator
    1. Can create an event
    2. Can see all registrations for an event
* The event participant
    1. Can see all events,
    2. Choose one event and fill the registration form for it
* An event has the following fields
    1. Name
    2. Description
    3. Location
    4. Start time
    5. End time
* A registration has the following fields
    1. Name
    2. Phone number
    3. Email address