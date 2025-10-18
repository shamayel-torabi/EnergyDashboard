CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE meters (
    meter_id integer NOT NULL,
    serial_number character varying(50) NULL,
    name character varying(100) NULL,
    station_id integer NULL,
    station_name character varying(100) NULL,
    active boolean NOT NULL DEFAULT FALSE,
    dismount boolean NOT NULL DEFAULT FALSE,
    start_operation_date timestamp without time zone NOT NULL,
    end_operation_date timestamp without time zone NOT NULL,
    created timestamp without time zone NOT NULL,
    last_modified timestamp without time zone NULL,
    CONSTRAINT pk_meters PRIMARY KEY (meter_id)
);

CREATE TABLE meter_energys (
    meter_energy_id uuid NOT NULL,
    meter_id integer NOT NULL,
    record_date timestamp with time zone NOT NULL,
    tool_type_id integer NULL,
    energy_active_export numeric(18,0) NOT NULL,
    energy_active_import numeric(18,0) NOT NULL,
    energy_reactive_export numeric(18,0) NOT NULL,
    energy_reactive_import numeric(18,0) NOT NULL,
    created timestamp without time zone NOT NULL,
    last_modified timestamp without time zone NULL,
    CONSTRAINT pk_meter_energys PRIMARY KEY (meter_energy_id),
    CONSTRAINT fk_meter_energys_meters_meter_id FOREIGN KEY (meter_id) REFERENCES meters (meter_id) ON DELETE CASCADE
);

CREATE INDEX ix_meter_energys_meter_id ON meter_energys (meter_id);

CREATE INDEX ix_meter_energys_record_date ON meter_energys (record_date);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20220731072307_UpdaateAppDb', '6.0.7');

COMMIT;

START TRANSACTION;

ALTER TABLE meters ALTER COLUMN start_operation_date TYPE timestamp with time zone;

ALTER TABLE meters ALTER COLUMN end_operation_date TYPE timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20220801035810_UpdateNewAppDb', '6.0.7');

COMMIT;

