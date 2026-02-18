CREATE DATABASE events;

CREATE TABLE public."events"
(
    "id" UUID PRIMARY KEY,
    "name" VARCHAR(50) NOT NULL,
    "description" VARCHAR(200) NOT NULL,
    "location" VARCHAR(100) NOT NULL,
    "start_time" TIMESTAMP WITH TIME ZONE NOT NULL,
    "start_time_offset" INTERVAL NOT NULL,
    "end_time" TIMESTAMP WITH TIME ZONE NOT NULL,
    "end_time_offset" INTERVAL NOT NULL
)