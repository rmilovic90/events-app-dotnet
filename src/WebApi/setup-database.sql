CREATE TABLE events
(
    "id" UUID CONSTRAINT pk_events PRIMARY KEY,
    "name" VARCHAR(50) NOT NULL,
    "description" VARCHAR(200) NOT NULL,
    "location" VARCHAR(100) NOT NULL,
    "start_time" TIMESTAMP NOT NULL,
    "start_time_offset" INTERVAL NOT NULL,
    "end_time" TIMESTAMP NOT NULL,
    "end_time_offset" INTERVAL NOT NULL
);

CREATE TABLE registrations
(
    "id" UUID CONSTRAINT pk_registrations PRIMARY KEY,
    "event_id" UUID NOT NULL CONSTRAINT fk_registrations_events REFERENCES events(id),
    "name" VARCHAR(100) NOT NULL,
    "phone_number" VARCHAR(16) NOT NULL,
    "email_address" VARCHAR(254) NOT NULL
);