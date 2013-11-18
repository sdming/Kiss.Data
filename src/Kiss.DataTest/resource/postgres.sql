CREATE TABLE ttable
(
  pk bigserial NOT NULL,
  cbool boolean,
  cint integer,
  cfloat double precision,
  cnumeric numeric(10,4),
  cstring character varying(100),
  cdatetime timestamp without time zone,
  cguid uuid,
  cbytes bit varying(1000),
  CONSTRAINT ttable_pkey PRIMARY KEY (pk)
);