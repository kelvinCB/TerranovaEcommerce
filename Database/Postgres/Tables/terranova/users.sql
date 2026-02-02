CREATE TABLE terranova.users (
  id char(26) PRIMARY KEY,
  first_name varchar(50) NOT NULL,
  last_name varchar(50) NOT NULL,
  phone_number varchar(20),
  date timestamptz,
  gender char(1),
  password_hash varchar(225) NOT NULL,
  is_active boolean NOT NULL DEFAULT TRUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  email_address varchar(50) NOT NULL UNIQUE,
  is_deleted boolean NOT NULL DEFAULT FALSE
);