CREATE TABLE terranova.payment_status (
  id char(26) PRIMARY KEY,
  name varchar(50) NOT NULL UNIQUE,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);