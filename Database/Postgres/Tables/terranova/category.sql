CREATE TABLE terranova.category (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);